/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Redzen.Sorting;

namespace SharpNeat.Graphs;

/// <summary>
/// For building instances of <see cref="WeightedDirectedGraph{T}"/>.
/// </summary>
/// <typeparam name="T">Connection weight data type.</typeparam>
public static class WeightedDirectedGraphBuilder<T>
    where T : struct
{
    #region Public Static Methods

    /// <summary>
    /// Create a directed graph based on the provided connections (between node IDs) and a predefined set of node IDs.
    /// Clearly the set of nodeIDs could be discovered by iterating over the connections. This overload exists to allow
    /// for additional fixed node IDs to be allocated regardless of whether they are connected to or not, this is primarily
    /// to allow for the allocation of NeatGenome input and output nodes, which are defined with fixed IDs but aren't
    /// necessarily connected to.
    /// </summary>
    /// <param name="connections">The connections that define the structure and weights of the weighted directed graph.</param>
    /// <param name="inputCount">Input node count.</param>
    /// <param name="outputCount">Output node count.</param>
    /// <returns>A new instance of <see cref="WeightedDirectedGraph{T}"/>.</returns>
    public static WeightedDirectedGraph<T> Create(
        Span<WeightedDirectedConnection<T>> connections,
        int inputCount, int outputCount)
    {
        // Debug assert that the connections are sorted.
        Debug.Assert(SortUtils.IsSortedAscending(connections, WeightedDirectedConnectionComparer<T>.Default));

        // Determine the full set of hidden node IDs.
        int inputOutputCount = inputCount + outputCount;
        var hiddenNodeIdArr = GetHiddenNodeIdArray(connections, inputOutputCount);

        // Compile a mapping from current nodeIDs to new IDs (i.e. removing gaps in the ID space).
        INodeIdMap nodeIdMap = DirectedGraphBuilderUtils.CompileNodeIdMap(
            inputOutputCount, hiddenNodeIdArr);

        // Extract/copy the neat genome connectivity graph into an ConnectionIds structure.
        // Notes.
        // The array contents will be manipulated, so copying this avoids modification of the genome's
        // connection gene list.
        // The IDs are substituted for node indexes here.
        CopyAndMapIds(connections, nodeIdMap,
            out ConnectionIds connIds,
            out T[] weightArr);

        // Construct and return a new WeightedDirectedGraph.
        int totalNodeCount = inputOutputCount + hiddenNodeIdArr.Length;
        return new WeightedDirectedGraph<T>(
            inputCount, outputCount,
            totalNodeCount,
            connIds,
            weightArr);
    }

    #endregion

    #region Private Static Methods

    private static int[] GetHiddenNodeIdArray(
        Span<WeightedDirectedConnection<T>> connSpan,
        int inputOutputCount)
    {
        // Build a hash set of all hidden nodes IDs referred to by the connections.
        var hiddenNodeIdSet = new HashSet<int>();

        // Extract hidden node IDs from the connections, to build a complete set of hidden nodeIDs.
        for(int i=0; i < connSpan.Length; i++)
        {
            if(connSpan[i].SourceId >= inputOutputCount)
                hiddenNodeIdSet.Add(connSpan[i].SourceId);

            if(connSpan[i].TargetId >= inputOutputCount)
                hiddenNodeIdSet.Add(connSpan[i].TargetId);
        }

        int[] hiddenNodeIdArr = hiddenNodeIdSet.ToArray();
        Array.Sort(hiddenNodeIdArr);
        return hiddenNodeIdArr;
    }

    /// <summary>
    /// Split each IWeightedDirectedConnection in a list into an array of DirectedConnections(s), and an array of weights.
    /// Map the node IDs to indexes as we go.
    /// </summary>
    private static void CopyAndMapIds(
        Span<WeightedDirectedConnection<T>> connSpan,
        INodeIdMap nodeIdMap,
        out ConnectionIds connIds,
        out T[] weightArr)
    {
        int count = connSpan.Length;
        connIds = new ConnectionIds(count);
        var srcIds = connIds.GetSourceIdSpan();
        var tgtIds = connIds.GetTargetIdSpan();
        weightArr = new T[count];

        for(int i=0; i < count; i++)
        {
            srcIds[i] = nodeIdMap.Map(connSpan[i].SourceId);
            tgtIds[i] = nodeIdMap.Map(connSpan[i].TargetId);
            weightArr[i] = connSpan[i].Weight;
        }
    }

    #endregion
}
