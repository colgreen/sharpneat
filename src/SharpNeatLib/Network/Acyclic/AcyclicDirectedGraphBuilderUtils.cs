using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Redzen;
using Redzen.Sorting;

namespace SharpNeat.Network.Acyclic
{
    internal static class AcyclicDirectedGraphBuilderUtils
    {
        #region Public Static Methods

        public static AcyclicDirectedGraph CreateAcyclicDirectedGraph(
            DirectedGraph digraph,
            GraphDepthInfo depthInfo,
            out int[] newIdByOldId,
            out int[] connectionIndexMap)
        {
            int inputCount = digraph.InputCount;
            int outputCount = digraph.OutputCount;

            // Assert that all input nodes are at depth zero.
            // Any input node with a non-zero depth must have an input connection, and this is not supported.
            Debug.Assert(ArrayUtils.Equals(depthInfo._nodeDepthArr, 0, 0, inputCount));

            // Compile a mapping from current node IDs to new IDs (based on node depth in the graph).
            newIdByOldId = CompileNodeIdMap(depthInfo, digraph.TotalNodeCount, inputCount);

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
            // Note. This overload of Array.Sort will also sort a second array, i.e. keep the items in both arrays aligned;
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
            return new AcyclicDirectedGraph(
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
            int inputCount)
        {
            // Create an array of all node IDs in the digraph.
            int[] nodeIdArr = new int[nodeCount];
            for(int i=0; i < nodeCount; i++) {
                nodeIdArr[i] = i;
            }

            // Sort nodeIdArr based on the depth of the nodes.
            // Note. We skip the input nodes because these all have depth zero and therefore remain
            // at fixed positions. The remaining nodes (output and hidden nodes) must be sorted using a
            // stable sort, hence timsort is used (which is guaranteed to be stable) rather than
            // Array.Sort() which at time of writing in implemented using introsort, which is not stable.
            //
            // TODO: Clarify why it must be a stable sort!
            //
            // TODO: Alloc reusable working arrays for use by timsort; this should improve performance 
            // by avoiding new allocs on each invocation of sort().
            TimSort<int,int>.Sort(depthInfo._nodeDepthArr, nodeIdArr, inputCount, nodeCount);

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
            ConnectionIdArrays connIdArrays,
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
