using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Redzen;
using Redzen.Sorting;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.Neat.Genome
{
    internal static class NeatGenomeAssertions<T>
        where T : struct
    {
        #region Public Static Methods

        public static void Assert(            
            MetaNeatGenome<T> metaNeatGenome,
            int id,
            int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            INodeIdMap nodeIndexByIdMap,
            DirectedGraph digraph,
            int[] connectionIndexMap)
        {
            // Mandatory ref tests.
            Debug.Assert(null != metaNeatGenome);
            Debug.Assert(null != connGenes);
            Debug.Assert(null != hiddenNodeIdArr);
            Debug.Assert(null != nodeIndexByIdMap);
            Debug.Assert(null != digraph);

            // Acyclic graph checks.
            if(metaNeatGenome.IsAcyclic) {
                AssertAcyclicGraph(metaNeatGenome, digraph, connectionIndexMap);
            }

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
            MetaNeatGenome<T> metaNeatGenome,
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
            ConnectionGenes<T> connGenes,
            DirectedGraph digraph,
            INodeIdMap nodeIndexByIdMap,
            int[] connectionIndexMap)
        {
            // Connection counts.
            Debug.Assert(connGenes._connArr.Length == digraph.ConnectionIdArrays.Length);

            // Connection order.
            Debug.Assert(SortUtils.IsSortedAscending(connGenes._connArr));
            Debug.Assert(IsSortedAscending(digraph.ConnectionIdArrays));

            // Connection node ID mappings.
            DirectedConnection[] connArr = connGenes._connArr;
            int[] srcIdArr = digraph.ConnectionIdArrays._sourceIdArr;
            int[] tgtIdArr = digraph.ConnectionIdArrays._targetIdArr;

            for(int i=0; i < connGenes._connArr.Length; i++)
            {
                DirectedConnection connGene = connArr[i];

                // Determine the index of he equivalent connection in the digraph.
                int genomeConnIdx = (null == connectionIndexMap) ? i : connectionIndexMap[i];

                Debug.Assert(nodeIndexByIdMap.Map(connArr[genomeConnIdx].SourceId) == srcIdArr[i]);
                Debug.Assert(nodeIndexByIdMap.Map(connArr[genomeConnIdx].TargetId) == tgtIdArr[i]);
            }
        }

        private static void AssertAcyclicGraph(
            MetaNeatGenome<T> metaNeatGenome,
            DirectedGraph digraph,
            int[] connectionIndexMap)
        {
            Debug.Assert(digraph is AcyclicDirectedGraph);
            Debug.Assert(null != connectionIndexMap);

            // Cast to an acyclic digraph.
            var acyclicDigraph = (AcyclicDirectedGraph)digraph;

            // Layer info checks.
            LayerInfo[] layerArr = acyclicDigraph.LayerArray;
            Debug.Assert(null != layerArr && layerArr.Length > 0);

            // Layer 0 is the input layer, thus the number of nodes in this layer should be at least the number of input nodes.
            // Note. Any node with no incoming connections is assigned to layer 0, therefore there can be non input nodes in 
            // this layer too.
            Debug.Assert(layerArr[0].EndNodeIdx >= metaNeatGenome.InputNodeCount);

            // EndNodeIdx is strictly increasing, as is EndConnectionIdx.
            // Note. There is always at least one node in a layer (otherwise the layer would not exist).
            for(int i=1; i < layerArr.Length; i++) {
                Debug.Assert(layerArr[i-1].EndNodeIdx < layerArr[i].EndNodeIdx);
            }

            // EndConnectionIdx is strictly increasing, except for the last layer which has no connections by definition (if 
            // there was a connection it would result in one more layer!).
            for(int i=1; i < layerArr.Length-1; i++) {
                Debug.Assert(layerArr[i-1].EndConnectionIdx < layerArr[i].EndConnectionIdx);
            }

            // The last layer has no connections, by definition.
            // Note. In principle there can be a single layer, i.e. a bunch of nodes with no connections between them;
            // it's nonsensical, but it's not disallowed.
            int lastLayerIdx = layerArr.Length-1;
            if(lastLayerIdx > 0) { 
                Debug.Assert(layerArr[lastLayerIdx].EndConnectionIdx == layerArr[lastLayerIdx-1].EndConnectionIdx);
            }

            // Reconstruct a GraphDepthInfo from the layer info.
            GraphDepthInfo depthInfo = BuildGraphDepthInfo(layerArr, digraph.TotalNodeCount);
            int[] nodeDepthArr = depthInfo._nodeDepthArr;

            // Connection tests.
            int connIdx = 0;
            int[] srcIdArr = digraph.ConnectionIdArrays._sourceIdArr;
            int[] tgtIdArr = digraph.ConnectionIdArrays._targetIdArr;

            // Loop the layer infos.
            for(int layerIdx=0; layerIdx < layerArr.Length; layerIdx++)
            {
                LayerInfo layerInfo = layerArr[layerIdx];

                // EndNodeIdx should never be negative or higher than the total node count.
                Debug.Assert(layerInfo.EndNodeIdx >=0 && layerInfo.EndNodeIdx <= digraph.TotalNodeCount);

                // The connections in this layer should all have a source node in this layer, and a target node in a layer
                // with a higher layer index.
                for(; connIdx < layerInfo.EndConnectionIdx; connIdx++)
                {                    
                    int srcId = srcIdArr[connIdx];
                    int tgtId = tgtIdArr[connIdx];

                    Debug.Assert(nodeDepthArr[srcId] == layerIdx);
                    Debug.Assert(nodeDepthArr[tgtId] > layerIdx);
                }
            }
        }

        #endregion

        #region Private Static Methods

        private static GraphDepthInfo BuildGraphDepthInfo(LayerInfo[] layerArr, int totalNodeCount)
        {
            int[] nodeDepthArr = new int[totalNodeCount];

            for(int layerIdx=0, nodeIdx = 0; layerIdx < layerArr.Length; layerIdx++)
            {
                int endNodeIdx = layerArr[layerIdx].EndNodeIdx;

                for(; nodeIdx < endNodeIdx; nodeIdx++) {
                    nodeDepthArr[nodeIdx] = layerIdx;
                }
            }

            return new GraphDepthInfo(layerArr.Length, nodeDepthArr);
        }

        #endregion

        #region Private Static Methods [ConnectionIdArrays Sort Order]

        private static bool IsSortedAscending(
            ConnectionIdArrays connIdArrays)
        {
            if(0 == connIdArrays.Length) {
                return true;
            }

            int[] srcIdArr = connIdArrays._sourceIdArr;
            int[] tgtIdArr = connIdArrays._targetIdArr;

            for(int i=0; i < srcIdArr.Length -1 ; i++)
            {
                if(Compare(srcIdArr[0], tgtIdArr[0], srcIdArr[i+1], tgtIdArr[i+1]) > 0) {
                    return false;
                }
            }

            return true;
        }

        private static int Compare(int srcIdA, int tgtIdA, int srcIdB, int tgtIdB)
        {
            if(srcIdA < srcIdB) { return -1; }
            if(srcIdA > srcIdB) { return 1; }

            if(tgtIdA < tgtIdB) { return -1; }
            if(tgtIdA > tgtIdB) { return 1; }
            return 0;
        }

        #endregion
    }
}
