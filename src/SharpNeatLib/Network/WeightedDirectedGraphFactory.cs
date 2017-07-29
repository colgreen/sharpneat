using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpNeat.Network2
{
    public static class WeightedDirectedGraphFactory<T> 
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
        public static WeightedDirectedGraph<T> Create(IList<IWeightedDirectedConnection<T>> connectionList, int inputCount, int outputCount)
        {
            // Debug assert that the connections are sorted.
            Debug.Assert(DirectedConnectionUtils.IsSorted(connectionList));

            // Compile a mapping from current nodeIDs to new IDs (i.e. removing gaps in the ID space).
            int inputOutputCount = inputCount + outputCount;
            int totalNodeCount;
            Func<int,int> nodeIdMapFn = CompileNodeIdMap(connectionList, inputOutputCount, out totalNodeCount);

            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            // Notes. 
            // The array contents will be manipulated, so copying this avoids modification of the genome's
            // connection gene list.
            // The IDs are substituted for node indexes here.
            DirectedConnection[] connArr;
            T[] weightArr;
            CopyAndMapIds(connectionList, nodeIdMapFn, out connArr, out weightArr);

            // Construct and return a new WeightedDirectedGraph.
            return new WeightedDirectedGraph<T>(connArr, inputCount, outputCount, totalNodeCount, weightArr);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Determine the set of node IDs, order them (thus assigning each node ID an index),
        /// and build a dictionary of indexes keyed by ID.
        /// </summary>
        private static Func<int,int> CompileNodeIdMap(
            IList<IWeightedDirectedConnection<T>> connList,
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
            Func<int,int> nodeIdMap,
            out DirectedConnection[] connArr,
            out T[] weightArr)
        {
            int count = connectionList.Count;
            connArr = new DirectedConnection[connectionList.Count];
            weightArr = new T[count];

            for(int i=0; i<connectionList.Count; i++) 
            {
                connArr[i] = new DirectedConnection(
                                    nodeIdMap(connectionList[i].SourceId),
                                    nodeIdMap(connectionList[i].TargetId));

                weightArr[i] = connectionList[i].Weight;
            }
        }

        #endregion
    }
}
