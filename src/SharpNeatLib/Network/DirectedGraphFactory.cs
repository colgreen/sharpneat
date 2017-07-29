using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpNeat.Network2
{
    public static class DirectedGraphFactory
    {
        #region Public Static Methods

        /// <summary>
        /// Create a directed graph based on the provided connections (between node IDs) and a predefined set of input/output 
        /// node IDs defined as being in a contiguous sequence starting at ID zero.
        /// </summary>
        /// <remarks>
        /// connectionList is assumed to be sorted by sourceID, TargetID.
        /// </remarks>
        public static DirectedGraph Create(IList<IDirectedConnection> connectionList, int inputCount, int outputCount)
        {
            // Debug assert that the connections are sorted.
            Debug.Assert(DirectedConnectionUtils.IsSorted(connectionList));

            // Build map from old IDs to new IDs (i.e. removing gaps in the ID space).
            int inputOutputCount = inputCount + outputCount;
            int totalNodeCount;
            Func<int,int> nodeIdMapFn = CompileNodeInfo(connectionList, inputOutputCount, out totalNodeCount);

            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            // Notes. 
            // The array contents will be manipulated, so copying this avoids modification of the genome's
            // connection gene list.
            // The IDs are substituted for node indexes here.
            DirectedConnection[] connArr = CopyAndMapIds(connectionList, nodeIdMapFn);

            // Construct and return a new DirectedGraph.
            return new DirectedGraph(connArr, inputCount, outputCount, totalNodeCount);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Determine the set of node IDs, order them (thus assigning each node ID an index),
        /// and build a dictionary of indexes keyed by ID.
        /// </summary>
        private static Func<int,int> CompileNodeInfo(
            IList<IDirectedConnection> connList,
            int inputOutputCount,
            out int totalNodeCount)
        {
            // Build a hash set of all hidden nodes IDs referred to by the connections.
            var hiddenNodeIdSet = new HashSet<int>();
            
            // Extract hidden node IDs from the connections, to build a complete set of hidden nodeIDs.
            for(int i=0; i<connList.Count; i++)
            {
                if(connList[i].SourceId >= inputOutputCount) { 
                    hiddenNodeIdSet.Add(connList[i].SourceId); 
                }
                if(connList[i].TargetId >= inputOutputCount) { 
                    hiddenNodeIdSet.Add(connList[i].TargetId); 
                }
            }

            // Set total node count output parameter.
            totalNodeCount = inputOutputCount + hiddenNodeIdSet.Count;

            // Extract hidden node IDs into an array.
            int hiddenNodeCount = hiddenNodeIdSet.Count;
            var hiddenNodeIdArr = new int[hiddenNodeCount];

            int idx = 0;
            foreach(int nodeId in hiddenNodeIdSet) {
                hiddenNodeIdArr[idx++] = nodeId;
            }

            // Sort the hidden node ID array.
            Array.Sort(hiddenNodeIdArr);

            // Build dictionary of hidden node new ID/index keyed by old ID.
            // Note. the new IDs start immediately after the last input/output node ID (defined by inputOutputCount).
            var hiddenNodeIdxById = new Dictionary<int,int>(hiddenNodeCount);
            for(int i=0, newId=inputOutputCount; i<hiddenNodeCount; i++, newId++) {
                hiddenNodeIdxById.Add(hiddenNodeIdArr[i], newId);
            }

            // Return a mapping function.
            // Note. this captures hiddenNodeIdxById in a closure. 
            Func<int,int> nodeIdxByIdFn = (int id) => {     
                    // Input/output node IDs are fixed.
                    if(id < inputOutputCount) {
                        return id;
                    }
                    // Hidden nodes have mappings stored in a dictionary.
                    return hiddenNodeIdxById[id]; 
                };

            return nodeIdxByIdFn;
        }

        private static DirectedConnection[] CopyAndMapIds(
            IList<IDirectedConnection> connectionList,
            Func<int,int> nodeIdMap)
        {
            var arr = new DirectedConnection[connectionList.Count];
            for(int i=0; i<connectionList.Count; i++) {
                arr[i] = new DirectedConnection(
                                nodeIdMap(connectionList[i].SourceId),
                                nodeIdMap(connectionList[i].TargetId));
            }
            return arr;
        }

        #endregion
    }
}
