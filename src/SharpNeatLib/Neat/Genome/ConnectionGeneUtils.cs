using System;
using System.Collections.Generic;
using System.Linq;
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

            for(int i=1; i < connArr.Length; i++)
            {
                if(CompareBySourceThenTarget(connArr[i-1], connArr[i]) > 0) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tests if he array of connection gene indexes is describes the connection genes in innovation ID sort order.
        /// </summary>
        /// <typeparam name="T">Connection weight type.</typeparam>
        /// <param name="connIdxArr">The array of connection gene indexes to test.</param>
        /// <param name="connArr">The connection gene array.</param>
        /// <returns>True if the array is sorted, otherwise false.</returns>
        public static bool IsSorted<T>(int[] connIdxArr, ConnectionGene<T>[] connArr)
            where T : struct
        {
            if(connIdxArr.Length == 0) {
                return true;
            }

            for(int i=1; i < connIdxArr.Length; i++)
            {
                if(connArr[connIdxArr[i-1]].Id > connArr[connIdxArr[i]].Id) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Search a list of connection genes sorted by sourceId,targetId, for a given directed connection.
        /// </summary>
        /// <typeparam name="T">Connection weight type.</typeparam>
        /// <param name="connArr">The sorted array of connection genes to search.</param>
        /// <param name="conn">The connection to search for, i.e. combination of source and target node IDs.</param>
        /// <returns>An array index if the item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than conn or, if there is no larger element, the bitwise
        /// complement of connArr.Length.</returns>
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
        /// Search for the index of a connection gene with the given innovation ID.
        /// </summary>
        /// <typeparam name="T">Connection weight type.</typeparam>
        /// <param name="connArr"></param>
        /// <param name="connIdxArr">An array of indexes into _connectionGeneArr, sorted by connection gene innovation ID.</param>
        /// <param name="id">The innovation ID to search for.</param>
        /// <returns>An array index if the item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than id or, if there is no larger element, the bitwise
        /// complement of connArr.Length.</returns>
        public static int BinarySearchId<T>(int[] connIdxArr, ConnectionGene<T>[] connArr, int id)
            where T : struct
        {
            return SearchUtils.BinarySearch(connIdxArr, id, (int x, int _id) => connArr[x].Id.CompareTo(_id));
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

        /// <summary>
        /// Create an array of indexes into a connection gene array, sorted by connection gene innovation IDs.
        /// </summary>
        /// <param name="connArr"></param>
        /// <returns></returns>
        public static int[] CreateConnectionIndexArray<T>(ConnectionGene<T>[] connArr)
            where T : struct
        {
            int[] connIdxArr = new int[connArr.Length];
            for(int i=0; i < connArr.Length; i++) {
                connIdxArr[i] = i;
            }
            Array.Sort(connIdxArr, (int x, int y) => connArr[x].Id.CompareTo(connArr[y].Id));
            return connIdxArr;
        }

        public static bool ValidateInnovationIds<T>(ConnectionGene<T>[] connArr, int inputCount, int outputCount)
            where T : struct
        {
            // Test for duplicate IDs.
            HashSet<int> connIdSet = new HashSet<int>();
            HashSet<int> nodeIdSet = new HashSet<int>();

            for(int i=0; i < connArr.Length; i++)
            {
                if(connIdSet.Contains(connArr[i].Id)) {
                    return false;
                }
                connIdSet.Add(connArr[i].Id);

                // Build the set of all node IDs.
                nodeIdSet.Add(connArr[i].SourceId);
                nodeIdSet.Add(connArr[i].TargetId);


                // Test for input-output connections with invalid innovation IDs, and/or
                // non-input-output connections with an input-output innovation ID.
                // Notes. Innovation IDs for IO connections are defined by a fixed scheme; failure to adhere to this scheme
                // will result in errors in logic such as add/delete connection mutations, sexual reproduction and decoding to
                // a functional neural network.

                var dc = new DirectedConnection(connArr[i].SourceId, connArr[i].TargetId);

                if(IOConnectionUtils.TryGetInputOutputConnectionId(dc, inputCount, outputCount, out int expectedId))
                {
                    // We have an IO connection; confirm that the actual innovation ID matches the expected ID.
                    if(connArr[i].Id != expectedId) {
                        return false;
                    }
                }
                else
                {
                    // Not an IO connection; confirm that the innovation ID isn't one reserved for IO connections.
                    if(IOConnectionUtils.IsInputOutputInnovationId(connArr[i].Id, inputCount, outputCount)) {
                        return false;
                    }
                }
            }

            // Test for innovation ID overlap between connections a nodes.
            if(connIdSet.Intersect(nodeIdSet).Count() != 0) {
                return false;
            }

            // All tests passed OK.
            return true;
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
