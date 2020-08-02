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
using System.Diagnostics;
using Redzen;
using Redzen.Sorting;

namespace SharpNeat.Network.Acyclic
{
    internal static class DirectedGraphAcyclicBuilderUtils
    {
        #region Public Static Methods

        public static DirectedGraphAcyclic CreateAcyclicDirectedGraph(
            DirectedGraph digraph,
            GraphDepthInfo depthInfo,
            out int[] newIdByOldId,
            out int[] connectionIndexMap)
        {
            // Timsort working arrays. We only need the variable slot to pass as reference, timsort will allocate them if 
            // necessary and return them, but here we just discard those arrays. To re-use the arrays call the method overload
            // that accepts the two arrays.
            int[] timsortWorkArr = null;
            int[] timsortWorkVArr = null;

            return CreateAcyclicDirectedGraph(
                digraph, depthInfo,
                out newIdByOldId,
                out connectionIndexMap,
                ref timsortWorkArr,
                ref timsortWorkVArr);
        }

        public static DirectedGraphAcyclic CreateAcyclicDirectedGraph(
            DirectedGraph digraph,
            GraphDepthInfo depthInfo,
            out int[] newIdByOldId,
            out int[] connectionIndexMap,
            ref int[] timsortWorkArr,
            ref int[] timsortWorkVArr)
        {
            int inputCount = digraph.InputCount;
            int outputCount = digraph.OutputCount;

            // Assert that all input nodes are at depth zero.
            // Any input node with a non-zero depth must have an input connection, and this is not supported.
            Debug.Assert(SpanUtils.Equals(depthInfo._nodeDepthArr.AsSpan(0, inputCount), 0));

            // Compile a mapping from current node IDs to new IDs (based on node depth in the graph).
            newIdByOldId = CompileNodeIdMap(depthInfo, digraph.TotalNodeCount, inputCount, ref timsortWorkArr, ref timsortWorkVArr);

            // Map the connection node IDs.
            ConnectionIdArrays connIdArrays = digraph.ConnectionIdArrays;
            MapIds(connIdArrays, newIdByOldId);

            // Init connection index map.
            int connCount = connIdArrays.Length;
            connectionIndexMap = new int[connCount];
            for(int i=0; i < connCount; i++) {
                connectionIndexMap[i] = i;
            }

            // Sort the connections based on sourceID, TargetId; this will arrange the connections based on the depth 
            // of the source nodes.
            // Note. This sort routine will also sort a secondary array, i.e. keep the items in both arrays aligned;
            // here we use this to create connectionIndexMap.
            ConnectionSorter<int>.Sort(connIdArrays, connectionIndexMap);

            // Make a copy of the sub-range of newIdMap that represents the output nodes.
            // This is required later to be able to locate the output nodes now that they have been sorted by depth.
            int[] outputNodeIdxArr = new int[outputCount];
            Array.Copy(newIdByOldId, inputCount, outputNodeIdxArr, 0, outputCount);

            // Create an array of LayerInfo(s).
            // Each LayerInfo contains the index + 1 of both the last node and last connection in that layer.
            //
            // The array is in order of depth, from layer zero (inputs nodes) to the last layer (usually output nodes,
            // but not necessarily if there is a dead end pathway with a high number of hops).
            //
            // Note. There is guaranteed to be at least one connection with a source at a given depth level, this is
            // because for there to be a layer N there must necessarily be a connection from a node in layer N-1 
            // to a node in layer N.
            int netDepth = depthInfo._networkDepth;
            LayerInfo[] layerInfoArr = new LayerInfo[netDepth];

            // Note. Scanning over nodes can start at inputCount instead of zero, because all nodes prior to that index
            // are input nodes and are therefore at depth zero. (input nodes are never the target of a connection,
            // therefore are always guaranteed to be at the start of a connectivity graph, and thus at depth zero).
            int nodeCount = digraph.TotalNodeCount;
            int nodeIdx = inputCount;
            int connIdx = 0;

            int[] nodeDepthArr = depthInfo._nodeDepthArr;
            int[] srcIdArr = connIdArrays._sourceIdArr;

            for (int currDepth = 0; currDepth < netDepth; currDepth++)
            {
                // Scan for last node at the current depth.
                for (; nodeIdx < nodeCount && nodeDepthArr[nodeIdx] == currDepth; nodeIdx++);

                // Scan for last connection at the current depth.
                for (; connIdx < srcIdArr.Length && nodeDepthArr[srcIdArr[connIdx]] == currDepth; connIdx++);

                // Store node and connection end indexes for the layer.
                layerInfoArr[currDepth] = new LayerInfo(nodeIdx, connIdx);
            }

            // Construct and return.
            return new DirectedGraphAcyclic(
                connIdArrays,
                inputCount, outputCount, nodeCount,
                layerInfoArr,
                outputNodeIdxArr);
        }

        #endregion

        #region Private Static Methods [Mid Level]

        private static int[] CompileNodeIdMap(
            GraphDepthInfo depthInfo,
            int nodeCount,
            int inputCount,
            ref int[] timsortWorkArr,
            ref int[] timsortWorkVArr)
        {
            // Create an array of all node IDs in the digraph.
            int[] nodeIdArr = new int[nodeCount];
            for(int i=0; i < nodeCount; i++) {
                nodeIdArr[i] = i;
            }

            // Sort nodeIdArr based on the depth of the nodes.
            // Notes. 
            // We skip the input nodes because these all have depth zero and therefore remain at fixed 
            // positions. 
            //
            // The remaining nodes (output and hidden nodes) are sorted by depth, noting that typically 
            // there will be multiple nodes at a given depth. Here we apply the TimSort algorithm; this 
            // has very good performance when there are pre-sorted sub-spans, either in the correct
            // direction or in reverse, as is typical of much real world data, and is likely the case here
            // too.
            //
            // Timsort also performs a stable sort, as it is based on a mergesort (note. at time of writing
            // Array.Sort employs introsort, which is not stable), thus avoids unnecessary shuffling of nodes
            // that are at the same depth. However the use of a stable sort is not a strict requirement here.
            //
            // Regarding timsort temporary working data. 
            // Depending on the data being sorted, timsort may use a temp array with up to N/2 elements. Here
            // we ensure that the maximum possible size is allocated, and we re-use these arrays in future 
            // calls. If instead we pass null or an array that is too short, then timsort will allocate a new
            // array internally, per sort, so we want to avoid that cost.

            // ENHANCEMENT: Modify TimSort class to accept working arrays by ref, so that if a larger array was allocated internally, we receive it back here.
            // Thus we achieve the same functionality without requiring knowledge of TimSort's internal logic.
            // Allocate new timsort working arrays, if necessary.
            int timsortWorkArrLength = nodeCount >> 1;

            if(timsortWorkArr is null || timsortWorkArr.Length < timsortWorkArrLength)
            {
                timsortWorkArr = new int[timsortWorkArrLength];
                timsortWorkVArr = new int[timsortWorkArrLength];
            }

            // Sort the node IDs by depth.
            TimSort<int,int>.Sort(depthInfo._nodeDepthArr, nodeIdArr, inputCount, nodeCount - inputCount, timsortWorkArr, timsortWorkVArr);

            // Each node is now assigned a new node ID based on its index in nodeIdArr, i.e.
            // we are re-allocating IDs based on node depth.
            // ENHANCEMENT: This mapping inversion is avoidable if the consumer of the mapping is modified to consume the 'old index to new index' mapping.
            int[] newIdByOldId = new int[nodeCount];
            for(int i=0; i < nodeCount; i++) {
                newIdByOldId[nodeIdArr[i]] = i;
            }

            return newIdByOldId;
        }

        private static void MapIds(
            in ConnectionIdArrays connIdArrays,
            int[] newIdByOldId)
        {
            int[] srcIdArr = connIdArrays._sourceIdArr;
            int[] tgtIdArr = connIdArrays._targetIdArr;

            for(int i=0; i < srcIdArr.Length; i++) 
            {
                srcIdArr[i] = newIdByOldId[srcIdArr[i]];
                tgtIdArr[i] = newIdByOldId[tgtIdArr[i]];
            }
        }

        #endregion
    }
}
