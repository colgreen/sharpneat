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

namespace SharpNeat.Network
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
        /// <remarks>
        /// connectionList is required to be sorted by sourceID, TargetID.
        /// </remarks>
        public static DirectedGraph Create(
            IList<DirectedConnection> connectionList,
            int inputCount, int outputCount)
        {
            // Debug assert that the connections are sorted.
            Debug.Assert(SortUtils.IsSortedAscending(connectionList));

            // Determine the full set of hidden node IDs.
            int inputOutputCount = inputCount + outputCount;
            var hiddenNodeIdArr = GetHiddenNodeIdArray(connectionList, inputOutputCount);

            // Compile a mapping from current nodeIDs to new IDs (i.e. removing gaps in the ID space).
            INodeIdMap nodeIdMap = DirectedGraphBuilderUtils.CompileNodeIdMap(
                inputOutputCount, hiddenNodeIdArr);

            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            // Notes. 
            // The array contents will be manipulated, so copying this avoids modification of the genome's
            // connection gene list.
            // The IDs are substituted for node indexes here.
            ConnectionIdArrays connIdArrays = CopyAndMapIds(connectionList, nodeIdMap);

            // Construct and return a new DirectedGraph.
            int totalNodeCount =  inputOutputCount + hiddenNodeIdArr.Length;
            return new DirectedGraph(connIdArrays, inputCount, outputCount, totalNodeCount);
        }

        #endregion

        #region Private Static Methods

        private static int[] GetHiddenNodeIdArray(
            IList<DirectedConnection> connectionList,
            int inputOutputCount)
        {
            // Build a hash set of all hidden nodes IDs referred to by the connections.
            var hiddenNodeIdSet = new HashSet<int>();
            
            // Extract hidden node IDs from the connections, to build a complete set of hidden nodeIDs.
            for(int i=0; i<connectionList.Count; i++)
            {
                if(connectionList[i].SourceId >= inputOutputCount) { 
                    hiddenNodeIdSet.Add(connectionList[i].SourceId); 
                }
                if(connectionList[i].TargetId >= inputOutputCount) { 
                    hiddenNodeIdSet.Add(connectionList[i].TargetId); 
                }
            }

            int[] hiddenNodeIdArr = hiddenNodeIdSet.ToArray();
            Array.Sort(hiddenNodeIdArr);
            return hiddenNodeIdArr;
        }

        private static ConnectionIdArrays CopyAndMapIds(
            IList<DirectedConnection> connectionList,
            INodeIdMap nodeIdMap)
        {
            int count = connectionList.Count;
            int [] srcIdArr = new int[count];
            int [] tgtIdArr = new int[count];

            for(int i=0; i<connectionList.Count; i++) 
            {
                srcIdArr[i] = nodeIdMap.Map(connectionList[i].SourceId);
                tgtIdArr[i] = nodeIdMap.Map(connectionList[i].TargetId);    
            }            

            return new ConnectionIdArrays(srcIdArr, tgtIdArr);
        }

        #endregion
    }
}
