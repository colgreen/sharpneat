using System;
using Redzen;
using SharpNeat.Network;

namespace SharpNeat.Neat.Genome
{
    public static class ConnectionGeneUtils
    {
        #region Public Static Methods

        public static void Sort<T>(ConnectionGene<T>[] connArr)
            where T : struct
        {
            Array.Sort(connArr, CompareBySourceThenTarget);
        }

        /// <summary>
        /// Tests if he array of connection genes is sorted by sourceId then targetId.
        /// </summary>
        /// <typeparam name="T">Connection weight type.</typeparam>
        /// <param name="connArr">The connection gene array to test.</param>
        /// <returns>True if the array is sorted, otherwise false.</returns>
        public static bool IsSorted<T>(ConnectionGene<T>[] connArr)
            where T : struct
        {
            if(connArr.Length == 0) {
                return true;
            }

            var prev = connArr[0];
            for(int i=1; i < connArr.Length; i++)
            {
                var curr = connArr[i];
                if(CompareBySourceThenTarget(prev, curr) > 0) {
                    return false;
                }
                prev = curr;
            }
            return true;
        }

        /// <summary>
        /// Searches an entire one-dimensional sorted list for a specific item, using the provided comparison function.
        /// </summary>
        /// <typeparam name="T">Connection weight type.</typeparam>
        /// <param name="connArr">The sorted array of connection genes to search.</param>
        /// <param name="conn">The connection to search for, i.e. combination of source and target node IDs.</param>
        /// <returns>The zero-based index of an item in the list, if item is found; otherwise, a negative number that is the 
        /// bitwise complement of the index of the next element that is larger than item or, if there is no larger element,
        /// the bitwise complement of list.Count.</returns>
        public static int BinarySearch<T>(ConnectionGene<T>[] connArr, DirectedConnection conn)
            where T : struct
        {
            return SearchUtils.BinarySearch(connArr, conn,  (ConnectionGene<T> x, DirectedConnection y) => 
                {   
                    // Compare source IDs.
                    if (x.SourceId < y.SourceId) { return -1; }
                    if (x.SourceId > y.SourceId) { return 1; }

                    // Source IDs are equal; compare target IDs.
                    if (x.TargetId < y.TargetId) { return -1; }
                    if (x.TargetId > y.TargetId) { return 1; }
                    return 0;
                });
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
        public static int GetConnectionIndexBySourceNodeId<T>(ConnectionGene<T>[] connArr, int srcNodeId)
            where T : struct
        {
            // Search for a connection with the given source node ID.
            int connIdx = SearchUtils.BinarySearch(connArr, srcNodeId,
                (ConnectionGene<T> conn, int nodeId) => conn.SourceId.CompareTo(nodeId));

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



        #endregion

        #region Private Static Methods

        private static int CompareBySourceThenTarget<T>(ConnectionGene<T> x, ConnectionGene<T> y)
            where T : struct
        {
            // Compare source IDs.
            if (x.SourceId < y.SourceId) { return -1; }
            if (x.SourceId > y.SourceId) { return 1; }

            // Source IDs are equal; compare target IDs.
            if (x.TargetId < y.TargetId) { return -1; }
            if (x.TargetId > y.TargetId) { return 1; }
            return 0;
        }

        #endregion
    }
}
