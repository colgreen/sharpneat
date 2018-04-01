using System;
using System.Collections.Generic;
using System.Linq;
using Redzen;
using Redzen.Sorting;
using SharpNeat.Network;

namespace SharpNeat.Neat.Genome
{
    public static class ConnectionGenesUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Create a sorted array of hidden node IDs.
        /// </summary>
        public static int[] CreateHiddenNodeIdArray(DirectedConnection[] connArr, int inputOutputCount)
        {
            // TODO / ENHANCEMENT: Set HashSet initial capacity, or use a pool of reusable HashSets?
            var idSet = new HashSet<int>();
            foreach(var conn in connArr)
            {
                // Skip input and output node IDs (these start from zero and go up to inputOutputCount-1).
                if(conn.SourceId >= inputOutputCount) {
                    idSet.Add(conn.SourceId);
                }
                if(conn.TargetId >= inputOutputCount) {
                    idSet.Add(conn.TargetId);
                }
            }

            int[] idArr = idSet.ToArray();
            Array.Sort(idArr);
            return idArr;
        }

        public static bool ValidateHiddenNodeIds(int[] hiddenNodeIdArr, DirectedConnection[] connArr, int inputOutputCount)
        {
            // Test that the IDs are sorted (required to allow for efficient searching of IDs using a binary search).
            if(!SortUtils.IsSortedAscending(hiddenNodeIdArr)) {
                return false;
            }

            // Get the set of hidden node IDs described by the connections, and test that they match the supplied hiddenNodeIdArr.
            int[] idArr = CreateHiddenNodeIdArray(connArr, inputOutputCount);
            if(!ArrayUtils.Equals(idArr, hiddenNodeIdArr)) { 
                return false;
            }
            return true;
        }

        #endregion
    }
}
