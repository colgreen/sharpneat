using System;
using System.Collections.Generic;
using System.Linq;
using Redzen;
using Redzen.Sorting;
using SharpNeat.Network;

namespace SharpNeat.Neat.Genome
{
    /// <summary>
    /// Static utility methods related to connection genes.
    /// </summary>
    public static class ConnectionGenesUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Create a sorted array of hidden node IDs.
        /// </summary>
        public static int[] CreateHiddenNodeIdArray(
            DirectedConnection[] connArr, 
            int inputOutputCount,
            HashSet<int> workingIdSet)
        {
            workingIdSet.Clear();

            foreach(var conn in connArr)
            {
                // Skip input and output node IDs (these start from zero and go up to inputOutputCount-1).
                if(conn.SourceId >= inputOutputCount) {
                    workingIdSet.Add(conn.SourceId);
                }
                if(conn.TargetId >= inputOutputCount) {
                    workingIdSet.Add(conn.TargetId);
                }
            }

            int[] idArr = workingIdSet.ToArray();
            Array.Sort(idArr);
            return idArr;
        }

        public static bool ValidateHiddenNodeIds(
            int[] hiddenNodeIdArr,
            DirectedConnection[] connArr,
            int inputOutputCount)
        {
            // Test that the IDs are sorted (required to allow for efficient searching of IDs using a binary search).
            if(!SortUtils.IsSortedAscending(hiddenNodeIdArr)) {
                return false;
            }

            // Get the set of hidden node IDs described by the connections, and test that they match the supplied hiddenNodeIdArr.
            int[] idArr = CreateHiddenNodeIdArray(connArr, inputOutputCount, new HashSet<int>());
            if(!ArrayUtils.Equals(idArr, hiddenNodeIdArr)) { 
                return false;
            }
            return true;
        }

        #endregion
    }
}
