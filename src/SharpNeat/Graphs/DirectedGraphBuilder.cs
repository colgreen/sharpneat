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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Redzen.Sorting;

namespace SharpNeat.Graphs
{
    /// <summary>
    /// Static class for building instances of <see cref="DirectedGraph"/>.
    /// </summary>
    public static class DirectedGraphBuilder
    {
        #region Public Static Methods

        /// <summary>
        /// Create a directed graph based on the provided connections (between node IDs) and a predefined set of input/output
        /// node IDs defined as being in a contiguous sequence starting at ID zero.
        /// </summary>
        /// <param name="connections">The connections that define the structure and weights of the weighted directed graph.</param>
        /// <param name="inputCount">Input node count.</param>
        /// <param name="outputCount">Output node count.</param>
        /// <returns>A new instance of <see cref="DirectedGraph"/>.</returns>
        /// <remarks>
        /// <paramref name="connections"/> is required to be sorted by sourceId, TargetId.
        /// </remarks>
        public static DirectedGraph Create(
            Span<DirectedConnection> connections,
            int inputCount, int outputCount)
        {
            // Debug assert that the connections are sorted.
            Debug.Assert(SortUtils.IsSortedAscending<DirectedConnection>(connections));

            // Determine the full set of hidden node IDs.
            int inputOutputCount = inputCount + outputCount;
            var hiddenNodeIdArr = GetHiddenNodeIdArray(connections, inputOutputCount);

            // Compile a mapping from current nodeIDs to new IDs (i.e. removing gaps in the ID space).
            INodeIdMap nodeIdMap = DirectedGraphBuilderUtils.CompileNodeIdMap(
                inputOutputCount, hiddenNodeIdArr);

            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            // Notes.
            // The array contents will be manipulated, so copying this avoids modification of the genome's
            // connection gene list.
            // The IDs are substituted for node indexes here.
            ConnectionIdArrays connIdArrays = CopyAndMapIds(connections, nodeIdMap);

            // Construct and return a new DirectedGraph.
            int totalNodeCount =  inputOutputCount + hiddenNodeIdArr.Length;
            return new DirectedGraph(inputCount, outputCount, totalNodeCount, connIdArrays);
        }

        #endregion

        #region Private Static Methods

        private static int[] GetHiddenNodeIdArray(
            Span<DirectedConnection> connSpan,
            int inputOutputCount)
        {
            // Build a hash set of all hidden nodes IDs referred to by the connections.
            var hiddenNodeIdSet = new HashSet<int>();

            // Extract hidden node IDs from the connections, to build a complete set of hidden nodeIDs.
            for(int i=0; i < connSpan.Length; i++)
            {
                if(connSpan[i].SourceId >= inputOutputCount) {
                    hiddenNodeIdSet.Add(connSpan[i].SourceId);
                }
                if(connSpan[i].TargetId >= inputOutputCount) {
                    hiddenNodeIdSet.Add(connSpan[i].TargetId);
                }
            }

            int[] hiddenNodeIdArr = hiddenNodeIdSet.ToArray();
            Array.Sort(hiddenNodeIdArr);
            return hiddenNodeIdArr;
        }

        private static ConnectionIdArrays CopyAndMapIds(
            Span<DirectedConnection> connSpan,
            INodeIdMap nodeIdMap)
        {
            int count = connSpan.Length;
            int[] srcIdArr = new int[count];
            int[] tgtIdArr = new int[count];

            for(int i=0; i < count; i++)
            {
                srcIdArr[i] = nodeIdMap.Map(connSpan[i].SourceId);
                tgtIdArr[i] = nodeIdMap.Map(connSpan[i].TargetId);
            }

            return new ConnectionIdArrays(srcIdArr, tgtIdArr);
        }

        #endregion
    }
}
