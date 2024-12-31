﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace SharpNeat.Neat.Genome;

/// <summary>
/// Static utility methods related to the building of new instances of <see cref="NeatGenome{T}"/>.
/// </summary>
internal static class NeatGenomeBuilderUtils
{
    /// <summary>
    /// Create a digraph from the a set of connection genes.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="metaNeatGenome">Genome metadata.</param>
    /// <param name="connGenes">Connection genes.</param>
    /// <param name="nodeIndexByIdMap">A mapping from node IDs to node indexes.</param>
    /// <returns>A new instance of <see cref="DirectedGraph"/>.</returns>
    public static DirectedGraph CreateDirectedGraph<TScalar>(
        MetaNeatGenome<TScalar> metaNeatGenome,
        ConnectionGenes<TScalar> connGenes,
        INodeIdMap nodeIndexByIdMap)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Extract/copy the neat genome connectivity graph into a ConnectionIds structure.
        // Notes.
        // The array contents will be manipulated, so copying this avoids modification of the genome's
        // connection gene list.
        // The IDs are substituted for node indexes here.
        CopyAndMapIds(
            connGenes._connArr,
            nodeIndexByIdMap,
            out ConnectionIds connIds);

        // Construct a new DirectedGraph.
        var digraph = new DirectedGraph(
            metaNeatGenome.InputNodeCount,
            metaNeatGenome.OutputNodeCount,
            nodeIndexByIdMap.Count,
            connIds);

        return digraph;
    }

    #region Private Static Methods

    private static void CopyAndMapIds(
        DirectedConnection[] connArr,
        INodeIdMap nodeIdMap,
        out ConnectionIds connIds)
    {
        int count = connArr.Length;
        connIds = new ConnectionIds(count);
        var srcIds = connIds.GetSourceIdSpan();
        var tgtIds = connIds.GetTargetIdSpan();

        for(int i=0; i < count; i++)
        {
            srcIds[i] = nodeIdMap.Map(connArr[i].SourceId);
            tgtIds[i] = nodeIdMap.Map(connArr[i].TargetId);
        }
    }

    #endregion
}
