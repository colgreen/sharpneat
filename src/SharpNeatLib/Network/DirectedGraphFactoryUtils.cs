using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpNeat.Network
{
    public static class DirectedGraphFactoryUtils
    {
        /// <summary>
        /// Determine the set of node IDs, order them (thus assigning each node ID an index),
        /// and build a dictionary of indexes keyed by ID.
        /// </summary>
        public static Func<int,int> CompileNodeIdMap(
            HashSet<int> hiddenNodeIdSet,
            int inputOutputCount)
        {
            // Extract hidden node IDs into an array, sorted by ID.
            int[] hiddenNodeIdArr = hiddenNodeIdSet.ToArray();
            Array.Sort(hiddenNodeIdArr);

            // Build dictionary of hidden node new ID/index keyed by old ID.
            // Note. the new IDs start immediately after the last input/output node ID (defined by inputOutputCount).
            var hiddenNodeIdxById = new Dictionary<int,int>(hiddenNodeIdArr.Length);
            for(int i=0, newId=inputOutputCount; i<hiddenNodeIdArr.Length; i++, newId++) {
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
    }
}
