using System;
using System.Collections.Generic;
using System.Linq;
using Redzen;
using SharpNeat.Network;

namespace SharpNeat.Neat.Genome
{
    public static class ConnectionGenesUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Tests if the array of connection gene indexes describe the connection genes in innovation ID sort order.
        /// </summary>
        /// <param name="connIdxArr">The array of connection gene indexes to test.</param>
        /// <param name="idArr">The connection gene ID array.</param>
        /// <returns>True if the array is sorted, otherwise false.</returns>
        public static bool IsSorted(int[] connIdxArr, int[] idArr)
        {
            if(connIdxArr.Length == 0) {
                return true;
            }

            for(int i=1; i < connIdxArr.Length; i++)
            {
                if(idArr[connIdxArr[i-1]] > idArr[connIdxArr[i]]) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Search for the index of a connection gene with the given innovation ID.
        /// </summary>
        /// <param name="connIdxArr">An array of indexes into the connection genes, sorted by connection gene innovation ID.</param>
        /// <param name="idArr">The connection gene ID array.</param>
        /// <param name="id">The innovation ID to search for.</param>
        /// <returns>An array index if the item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than id or, if there is no larger element, the bitwise
        /// complement of connArr.Length.</returns>
        public static int BinarySearchId(int[] connIdxArr, int[] idArr, int id)
        {
            return SearchUtils.BinarySearch(connIdxArr, id, (int x, int _id) => idArr[x].CompareTo(_id));
        }

        /// <summary>
        /// Create an array of indexes into a connection gene array, sorted by connection gene innovation IDs.
        /// </summary>
        /// <param name="connGenes">Connection genes structure.</param>
        /// <returns>A new array of connection gene indexes.</returns>
        public static int[] CreateConnectionIndexArray<T>(ConnectionGenes<T> connGenes)
            where T : struct
        {
            var connArr = connGenes._connArr;
            var idArr = connGenes._idArr;


            int[] connIdxArr = new int[connArr.Length];
            for(int i=0; i < connArr.Length; i++) {
                connIdxArr[i] = i;
            }
            Array.Sort(connIdxArr, (int x, int y) => idArr[x].CompareTo(idArr[y]));
            return connIdxArr;
        }

        /// <summary>
        /// Validation of innovation IDs in a connection genes structure.
        /// </summary>
        /// <typeparam name="T">Connection weight type.</typeparam>
        /// <param name="connGenes">The connection genes structure to test.</param>
        /// <param name="inputCount">The number of input nodes. In NEAT this number is relevant to the innovation ID tests.</param>
        /// <param name="outputCount">The number of output nodes. In NEAT this number is relevant to the innovation ID tests.</param>
        /// <returns>True if the test passes successfully; otherwise false.</returns>
        public static bool ValidateInnovationIds<T>(ConnectionGenes<T> connGenes, int inputCount, int outputCount)
            where T : struct
        {
            // Test for duplicate IDs.
            HashSet<int> connIdSet = new HashSet<int>();
            HashSet<int> nodeIdSet = new HashSet<int>();

            var connArr = connGenes._connArr;
            int[] idArr = connGenes._idArr;

            for(int i=0; i < connArr.Length; i++)
            {
                if(connIdSet.Contains(idArr[i])) {
                    return false;
                }
                connIdSet.Add(idArr[i]);

                // Build the set of all node IDs.
                nodeIdSet.Add(connArr[i].SourceId);
                nodeIdSet.Add(connArr[i].TargetId);

                // Test for input-output connections with invalid innovation IDs, and/or non-input-output connections with 
                // an input-output innovation ID.
                // Notes. Innovation IDs for IO connections are defined by a fixed scheme; failure to adhere to this scheme
                // will result in errors in logic such as the add/delete connection mutations, sexual reproduction and decoding
                // to a functional neural network.
                var dc = new DirectedConnection(connArr[i].SourceId, connArr[i].TargetId);

                if(IOConnectionUtils.TryGetInputOutputConnectionId(dc, inputCount, outputCount, out int expectedId))
                {
                    // We have an IO connection; confirm that the actual innovation ID matches the expected ID.
                    if(idArr[i] != expectedId) {
                        return false;
                    }
                }
                else
                {
                    // Not an IO connection; confirm that the innovation ID isn't one reserved for IO connections.
                    if(IOConnectionUtils.IsInputOutputInnovationId(idArr[i], inputCount, outputCount)) {
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
    }
}
