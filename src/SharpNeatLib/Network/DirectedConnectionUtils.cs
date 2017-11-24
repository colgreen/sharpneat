using System.Collections.Generic;
using Redzen;

namespace SharpNeat.Network
{
    public static class DirectedConnectionUtils
    {
        public static bool IsSorted(IList<DirectedConnection> connList)
        {
            if(connList.Count == 0) {
                return true;
            }

            DirectedConnection prev = connList[0];
            for(int i=1; i < connList.Count; i++)
            {
                DirectedConnection curr = connList[i];
                if(ConnectionCompareFunctions.Compare(prev, curr) > 0) {
                    return false;
                }
                prev = curr;
            }
            return true;
        }

        public static bool IsSorted<T>(IList<WeightedDirectedConnection<T>> connList)
            where T : struct
        {
            if(connList.Count == 0) {
                return true;
            }

            var prev = connList[0];
            for(int i=1; i < connList.Count; i++)
            {
                var curr = connList[i];
                if(ConnectionCompareFunctions.Compare(prev, curr) > 0) {
                    return false;
                }
                prev = curr;
            }
            return true;
        }

        /// <summary>
        /// Get the index of the first connection with the given source node ID.
        /// </summary>
        /// <param name="connArr">The array of connections to search; these must be sorted by source node ID.</param>
        /// <param name="srcNodeId">The source node ID to search for.</param>
        /// <returns>The index of the first connection with the given source node index.</returns>
        /// <remarks>
        /// If srcNodeId is not found and is less than one or more elements in array, the negative number returned is
        /// the bitwise complement of the index of the first connection that is larger than srcNodeId.
        /// If value is not found and value is greater than all connections in array, the negative number returned is the
        /// bitwise complement of the index of the last element plus 1. 
        /// </remarks>
        public static int GetConnectionIndexBySourceNodeId(DirectedConnection[] connArr, int srcNodeId)
        {
            // Search for a connection with the given source node ID.
            int connIdx = SearchUtils.BinarySearch(connArr, srcNodeId,
                (DirectedConnection conn, int nodeId) => conn.SourceId.CompareTo(nodeId));

            // Test for no match, i.e. no connections with the given source node ID.
            if(connIdx < 0) {   
                return connIdx;
            }

            // Note. if there are multiple connections with the given source ID then BinarySearch() will 
            // return the index of one of them, but makes no guarantee regarding which one. As such we scan
            // in reverse for the first connection.
            for(; connIdx > 0 && connArr[connIdx-1].SourceId == srcNodeId; connIdx--);
            
            return connIdx;
        }
    }
}
