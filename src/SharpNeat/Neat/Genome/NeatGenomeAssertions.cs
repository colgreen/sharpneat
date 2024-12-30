﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Numerics;
using Redzen.Sorting;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.Neat.Genome;

/// <summary>
/// Static methods for validating/verifying the data associated with a NEAT genome.
/// Principally for use in debug builds and debugging sessions.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
internal static class NeatGenomeAssertions<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    #region Public Static Methods

    /// <summary>
    /// Validate/verify the data associated with a NEAT genome, applying a series of debug asserts.
    /// </summary>
    /// <param name="metaNeatGenome">Genome metadata.</param>
    /// <param name="id">Genome ID.</param>
    /// <param name="birthGeneration">Genome birth generation.</param>
    /// <param name="connGenes">Connection genes data structure.</param>
    /// <param name="hiddenNodeIdArr">An array of the genome's hidden node IDs.</param>
    /// <param name="nodeIndexByIdMap">Mapping function for obtaining a node index for a given node ID.</param>
    /// <param name="digraph">The directed graph that the current genome represents.</param>
    /// <param name="connectionIndexMap">A set of connection index mappings.</param>
    public static void AssertIsValid(
        MetaNeatGenome<TScalar> metaNeatGenome,
        int id,
        int birthGeneration,
        ConnectionGenes<TScalar> connGenes,
        int[] hiddenNodeIdArr,
        INodeIdMap nodeIndexByIdMap,
        DirectedGraph digraph,
        int[]? connectionIndexMap)
    {
        // Check for mandatory object references.
        Debug.Assert(metaNeatGenome is not null);
        Debug.Assert(connGenes is not null);
        Debug.Assert(hiddenNodeIdArr is not null);
        Debug.Assert(nodeIndexByIdMap is not null);
        Debug.Assert(digraph is not null);

        // Basic check on ID and birth generation.
        Debug.Assert(id >= 0);
        Debug.Assert(birthGeneration >= 0);

        // Acyclic graph checks.
        if(metaNeatGenome.IsAcyclic)
            AssertAcyclicGraph(metaNeatGenome, digraph, connectionIndexMap);

        // Node counts.
        AssertNodeCounts(metaNeatGenome, hiddenNodeIdArr, nodeIndexByIdMap, digraph);

        // Hidden node IDs.
        Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

        // Connections.
        AssertConnections(connGenes, digraph, nodeIndexByIdMap, connectionIndexMap);
    }

    #endregion

    #region Private Static Methods

    private static void AssertNodeCounts(
        MetaNeatGenome<TScalar> metaNeatGenome,
        int[] hiddenNodeIdArr,
        INodeIdMap nodeIndexByIdMap,
        DirectedGraph digraph)
    {
        int totalNodeCount = metaNeatGenome.InputNodeCount + metaNeatGenome.OutputNodeCount + hiddenNodeIdArr.Length;

        Debug.Assert(digraph.InputCount == metaNeatGenome.InputNodeCount);
        Debug.Assert(digraph.OutputCount == metaNeatGenome.OutputNodeCount);
        Debug.Assert(digraph.TotalNodeCount == totalNodeCount);
        Debug.Assert(nodeIndexByIdMap.Count == totalNodeCount);
    }

    private static void AssertConnections(
        ConnectionGenes<TScalar> connGenes,
        DirectedGraph digraph,
        INodeIdMap nodeIndexByIdMap,
        int[]? connectionIndexMap)
    {
        // Connection counts.
        Debug.Assert(connGenes._connArr.Length == digraph.ConnectionIds.Length);

        // Connection order.
        Debug.Assert(SortUtils.IsSortedAscending<DirectedConnection>(connGenes._connArr));
        Debug.Assert(IsSortedAscending(digraph.ConnectionIds));

        // Connection node ID mappings.
        DirectedConnection[] connArr = connGenes._connArr;
        ReadOnlySpan<int> srcIds = digraph.ConnectionIds.GetSourceIdSpan();
        ReadOnlySpan<int> tgtIds = digraph.ConnectionIds.GetTargetIdSpan();

        for(int i=0; i < connGenes._connArr.Length; i++)
        {
            // Determine the index of the equivalent connection in the digraph.
            int genomeConnIdx = (connectionIndexMap is null) ? i : connectionIndexMap[i];

            Debug.Assert(nodeIndexByIdMap.Map(connArr[genomeConnIdx].SourceId) == srcIds[i]);
            Debug.Assert(nodeIndexByIdMap.Map(connArr[genomeConnIdx].TargetId) == tgtIds[i]);
        }
    }

    private static void AssertAcyclicGraph(
        MetaNeatGenome<TScalar> metaNeatGenome,
        DirectedGraph digraph,
        int[]? connectionIndexMap)
    {
        Debug.Assert(digraph is DirectedGraphAcyclic);
        Debug.Assert(connectionIndexMap is not null);

        // Cast to an acyclic digraph.
        var acyclicDigraph = (DirectedGraphAcyclic)digraph;

        // Layer info checks.
        LayerInfo[] layerArr = acyclicDigraph.LayerArray;
        Debug.Assert(layerArr is not null && layerArr.Length > 0);

        // Layer zero is the input layer, thus the number of nodes in this layer should be at least the number of input nodes.
        // Note. Any node with no incoming connections is also assigned to layer zero, therefore there can be non-input nodes in
        // this layer too.
        Debug.Assert(layerArr[0].EndNodeIdx >= metaNeatGenome.InputNodeCount);

        // EndNodeIdx is strictly increasing, as is EndConnectionIdx.
        // Note. There is always at least one node in a layer (otherwise the layer would not exist).
        for(int i=1; i < layerArr.Length; i++)
            Debug.Assert(layerArr[i-1].EndNodeIdx < layerArr[i].EndNodeIdx);

        // EndConnectionIdx is strictly increasing, except for the last layer which has no connections by definition (if
        // there was a connection it would result in one more layer!).
        for(int i=1; i < layerArr.Length-1; i++)
            Debug.Assert(layerArr[i-1].EndConnectionIdx < layerArr[i].EndConnectionIdx);

        // The last layer has no connections, by definition.
        // Note. In principle there can be a single layer, i.e. a bunch of nodes with no connections between them;
        // it's nonsensical, but it's not disallowed.
        int lastLayerIdx = layerArr.Length-1;
        if(lastLayerIdx > 0)
            Debug.Assert(layerArr[lastLayerIdx].EndConnectionIdx == layerArr[lastLayerIdx-1].EndConnectionIdx);

        // Reconstruct a GraphDepthInfo from the layer info.
        GraphDepthInfo depthInfo = BuildGraphDepthInfo(layerArr, digraph.TotalNodeCount);
        int[] nodeDepthArr = depthInfo._nodeDepthArr;

        // Connection tests.
        int connIdx = 0;
        ReadOnlySpan<int> srcIds = digraph.ConnectionIds.GetSourceIdSpan();
        ReadOnlySpan<int> tgtIds = digraph.ConnectionIds.GetTargetIdSpan();

        // Loop the layer infos.
        for(int layerIdx = 0; layerIdx < layerArr.Length; layerIdx++)
        {
            LayerInfo layerInfo = layerArr[layerIdx];

            // EndNodeIdx should never be negative or higher than the total node count.
            Debug.Assert(layerInfo.EndNodeIdx >= 0 && layerInfo.EndNodeIdx <= digraph.TotalNodeCount);

            // Loop the connections in the current layer.
            for(; connIdx < layerInfo.EndConnectionIdx; connIdx++)
            {
                int srcId = srcIds[connIdx];
                int tgtId = tgtIds[connIdx];

                // The connections in the current layer should all have a source node in this layer.
                Debug.Assert(nodeDepthArr[srcId] == layerIdx);

                // The target node should normally be in a higher layer. However, layer zero is a special case because it
                // contains not only the input nodes, but can also contain hidden nodes that are not reachable from an input node.
                //
                // Thus, the connections in layer zero should have either:
                // a) an input source node in this layer, and a target node in a layer with a higher layer index, or,
                // b) a hidden source node in this layer, and a target node that can be in any layer, including layer zero
                // if the target node is also unreachable from an input node (i.e. via another connectivity path).
                Debug.Assert(
                        (layerIdx == 0 && (srcId >= digraph.InputCount || nodeDepthArr[tgtId] > 0))
                    ||  (layerIdx > 0 && nodeDepthArr[tgtId] > layerIdx));
            }
        }
    }

    private static GraphDepthInfo BuildGraphDepthInfo(LayerInfo[] layerArr, int totalNodeCount)
    {
        int[] nodeDepthArr = new int[totalNodeCount];

        for(int layerIdx = 0, nodeIdx = 0; layerIdx < layerArr.Length; layerIdx++)
        {
            int endNodeIdx = layerArr[layerIdx].EndNodeIdx;

            for(; nodeIdx < endNodeIdx; nodeIdx++)
                nodeDepthArr[nodeIdx] = layerIdx;
        }

        return new GraphDepthInfo(layerArr.Length, nodeDepthArr);
    }

    #endregion

    #region Private Static Methods [ConnectionIds Sort Order]

    private static bool IsSortedAscending(
        in ConnectionIds connIds)
    {
        if(connIds.Length == 0)
        {
            return true;
        }

        Span<int> srcIds = connIds.GetSourceIdSpan();
        Span<int> tgtIds = connIds.GetTargetIdSpan();

        for(int i=0; i < srcIds.Length - 1; i++)
        {
            if(Compare(ref srcIds[0], ref tgtIds[0], ref srcIds[i+1], ref tgtIds[i+1]) > 0)
                return false;
        }

        return true;
    }

    private static int Compare(ref int srcIdA, ref int tgtIdA, ref int srcIdB, ref int tgtIdB)
    {
        if(srcIdA < srcIdB) return -1;
        if(srcIdA > srcIdB) return 1;

        if(tgtIdA < tgtIdB) return -1;
        if(tgtIdA > tgtIdB) return 1;
        return 0;
    }

    #endregion
}
