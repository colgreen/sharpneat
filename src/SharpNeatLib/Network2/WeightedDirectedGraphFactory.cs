using System;
using System.Collections.Generic;

namespace SharpNeat.Network2
{
    public static class WeightedDirectedGraphFactory<T> where T : struct
    {
        #region Public Static Methods

        /// <summary>
        /// Create a directed graph based on the provided connections (between node IDs) and a predefined set of node IDs.
        /// Clearly the set of nodeIDs could be discovered by iterating over the connections. This overload exists to allow
        /// for additional fixed node IDs to be allocated regardless of whether they are connected to or not, this is primarily
        /// to allow for the allocation of NeatGenome input and output nodes, which are defined with fixed IDs but aren't
        /// necessarily connected to.
        /// </summary>
        public static WeightedDirectedGraph<T> Create(IList<IWeightedDirectedConnection<T>> connectionList, IEnumerable<int> fixedNodeIds)
        {
            Dictionary<int,int> nodeIdxById = CompileNodeInfo(connectionList, fixedNodeIds);

            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            // Notes. 
            // The array contents will be manipulated, so copying this avoids modification of the genome's
            // connection gene list.
            // The IDs are substituted for node indexes here.
            DirectedConnection[] connArr;
            T[] weightArr;
            CopyAndMapIds(connectionList, nodeIdxById, out connArr, out weightArr);

            // Sort the connections by source then target node ID/index (i.e. secondary sort on target).
            // Note. each connection's weight must be moved/sorted to remain at the same index as its corresponding DirectedConnection. 
            Sort(connArr, weightArr);

            // Construct and return a new DirectedGraph.
            return new WeightedDirectedGraph<T>(connArr, nodeIdxById.Count, weightArr);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Determine the set of node IDs, order them (thus assigning each node ID an index),
        /// and build a dictionary of indexes keyed by ID.
        /// </summary>
        private static Dictionary<int,int> CompileNodeInfo(IList<IWeightedDirectedConnection<T>> connList, IEnumerable<int> fixedNodeIds)
        {
            // Build a hash set of all nodes IDs referred to by the connections.
            var nodeIdSet = new HashSet<int>();
            
            // Allocate the fixed node IDs.
            if(null != fixedNodeIds)
            {
                foreach(int id in fixedNodeIds) {
                    nodeIdSet.Add(id);
                }
            }
            
            // Extract node ID from the connections, to build a complete set of nodeIDs.
            for(int i=0; i<connList.Count; i++)
            {
                nodeIdSet.Add(connList[i].SourceId);
                nodeIdSet.Add(connList[i].TargetId);
            }

            // Extract node IDs into an array.
            int nodeCount = nodeIdSet.Count;
            var nodeIdArr = new int[nodeCount];

            int idx = 0;
            foreach(int nodeId in nodeIdSet) {
                nodeIdArr[idx++] = nodeId;
            }

            // Sort the node ID array.
            Array.Sort(nodeIdArr);

            // Build dictionary of node indexes keyed by ID.
            var nodeIdxById = new Dictionary<int,int>(nodeCount);
            for(int i=0; i<nodeCount; i++) {
                nodeIdxById.Add(nodeIdArr[i], i);
            }
            return nodeIdxById;
        }

        /// <summary>
        /// Split each IWeightedDirectedConnection in a list into an array of DirectedConnections(s), and an array of weights.
        /// Map the node IDs to indexes as we go.
        /// </summary>
        /// <param name="connectionList"></param>
        /// <param name="nodeIdMap"></param>
        /// <param name="connArr"></param>
        /// <param name="weightArr"></param>
        private static void CopyAndMapIds(
            IList<IWeightedDirectedConnection<T>> connectionList,
            IDictionary<int,int> nodeIdMap,
            out DirectedConnection[] connArr,
            out T[] weightArr)
        {
            int count = connectionList.Count;
            connArr = new DirectedConnection[connectionList.Count];
            weightArr = new T[count];

            for(int i=0; i<connectionList.Count; i++) 
            {
                connArr[i] = new DirectedConnection(nodeIdMap[connectionList[i].SourceId], nodeIdMap[connectionList[i].TargetId]);
                weightArr[i] = connectionList[i].Weight;
            }
        }

        private static void Sort(DirectedConnection[] connArr, T[] weightArr)
        {
            // Sort the connections by source then target ID (i.e. secondary sort on target).
            // Note. This overload of Aray.Sort will also sort a second array, i.e. keep the 
            // items in both arrays aligned.
            Array.Sort(connArr, weightArr, DirectedConnectionComparer.__Instance);
        }

        #endregion

        #region Inner Class

        class DirectedConnectionComparer : IComparer<DirectedConnection>
        {
            // Pre-built re-usable instance.
            public static readonly DirectedConnectionComparer __Instance = new DirectedConnectionComparer();

            public int Compare(DirectedConnection x, DirectedConnection y)
            {
                // Compare source IDs.
                if (x.SourceId < y.SourceId) {
                    return -1;
                }
                if (x.SourceId > y.SourceId) {
                    return 1;
                }

                // Source IDs are equal; compare target IDs.
                if (x.TargetId < y.TargetId) {
                    return -1;
                }
                if (x.TargetId > y.TargetId) {
                    return 1;
                }
                return 0;
            }
        }

        #endregion
    }
}
