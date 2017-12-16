using System;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public partial class AddNodeStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        /// <summary>
        /// Static utility methods.
        /// </summary>
        static class Utils
        {
            #region Public Static Methods

            /// <summary>
            /// Get an array of hidden node IDs in the child genome.
            /// </summary>
            public static int[] GetHiddenNodeIdArray(
                NeatGenome<T> parent,
                int addedNodeId,
                bool newInnovationIdsFlag)
            {
                int[] parentIdArr = parent.HiddenNodeIdArray;
                int childLen = parentIdArr.Length + 1;
                int[] childIdArr = new int[childLen];

                // New innovation IDs are always higher than any existing IDs, therefore adding
                // the new node ID to the end of the list will maintain sorter order.
                if(newInnovationIdsFlag)
                {
                    Array.Copy(parentIdArr, childIdArr, parentIdArr.Length);
                    childIdArr[childIdArr.Length-1] = addedNodeId;
                    return childIdArr;
                }
            
                // Determine the insertion index for the new node ID.
                int insertIdx = ~Array.BinarySearch(parentIdArr, addedNodeId);

                // Copy all IDs up to the insertion index.
                Array.Copy(parentIdArr, 0, childIdArr, 0, insertIdx);

                // Insert the added node ID.
                childIdArr[insertIdx] = addedNodeId;

                // Copy all remaining IDs after the index. 
                Array.Copy(parentIdArr, insertIdx, childIdArr, insertIdx+1, parentIdArr.Length - insertIdx);

                return childIdArr;
            }

            public static int[] CreateConnectionIndexArray(
                NeatGenome<T> parent,
                int removedConnId,
                int removedConnIdx,
                (int connIdx, int id)[] childInsertionArr,
                int parentInsertIdx1,
                int parentInsertIdx2,
                bool newInnovationIdsFlag)
            {
                // Ensure childInsertionArr is sorted by ID.
                if(childInsertionArr[0].id > childInsertionArr[1].id)
                {
                    var tmp = childInsertionArr[0];
                    childInsertionArr[0] = childInsertionArr[1];
                    childInsertionArr[1] = tmp;
                }

                // Allocate connIdxArr.
                var parentConnIdxArr = parent.ConnectionIndexArray;
                int parentLen = parentConnIdxArr.Length;
                int childLen = parentLen + 1;
                var connIdxArr = new int[childLen];

                // Get the index of the element to be removed.
                int removeIdx = ConnectionGenesUtils.BinarySearchId(parentConnIdxArr, parent.ConnectionGenes._idArr, removedConnId);

                // Get the insertion indexes for the two new connection IDs.
                // Note. New innovation IDs are always higher than any existing innovation ID, therefore we can avoid looking up the 
                // insertion indexes for new IDs.
                int insertIdx1, insertIdx2;
                if(newInnovationIdsFlag)
                {
                    insertIdx1 = parentConnIdxArr.Length;
                    insertIdx2 = parentConnIdxArr.Length;
                }
                else
                {
                    insertIdx1 = ~ConnectionGenesUtils.BinarySearchId(parentConnIdxArr, parent.ConnectionGenes._idArr, childInsertionArr[0].id);
                    insertIdx2 = ~ConnectionGenesUtils.BinarySearchId(parentConnIdxArr, parent.ConnectionGenes._idArr, childInsertionArr[1].id);
                }

                // Build an array of parent indexes to stop at when copying from the parent to the child array.
                // Note. Each index is combined with a second value; an index into newIdArr for insertions,
                // and -1 for the split index (the connection to be removed)
                (int,int)[] stopIdxArr = new []
                {
                    (removeIdx, -1),
                    (insertIdx1, 0),
                    (insertIdx2, 1)
                };

                // Sort by the first index value.
                Array.Sort(stopIdxArr, ((int,int)x, (int,int)y) => x.Item1.CompareTo(y.Item1));

                // Loop over stopIdxArr.
                int parentIdx = 0;
                int childIdx = 0;

                for(int i=0; i<stopIdxArr.Length; i++)
                {
                    int stopIdx = stopIdxArr[i].Item1;
                    int newEntryIdx = stopIdxArr[i].Item2;

                    // Copy all parent elements up to the stop index.
                    int copyLen = stopIdx - parentIdx;
                    if(copyLen > 0) {
                        Copy(parentConnIdxArr, parentIdx, connIdxArr, childIdx, copyLen, removedConnIdx, parentInsertIdx1, parentInsertIdx2);
                    }

                    // Update parentIdx, childIdx.
                    parentIdx = stopIdx;
                    childIdx += copyLen;

                    // Test what to do at the stopIdx.
                    if(-1 == newEntryIdx)
                    {   // We are at the parent connection to be skipped.
                        parentIdx++;
                        continue;
                    }

                    // We are at an insertion point in connIdxArr.
                    connIdxArr[childIdx] = childInsertionArr[newEntryIdx].connIdx;
                    childIdx++;
                }

                // Copy any remaining items.
                int len = parentConnIdxArr.Length - parentIdx;
                if (len > 0) {
                    Copy(parentConnIdxArr, parentIdx, connIdxArr, childIdx, len, removedConnIdx, parentInsertIdx1, parentInsertIdx2);
                }

                return connIdxArr;
            }

            #endregion

            #region Private Static Methods

            private static void Copy(int[] srcArr, int srcIdx, int[] tgtArr, int tgtIdx, int length, int removedConnIdx, int insertIdx1, int insertIdx2)
            {
                for(int i=0; i<length; i++) 
                {
                    int parentConnIdx = srcArr[srcIdx+i];
                    int connIdx = parentConnIdx;

                    if(parentConnIdx > removedConnIdx) connIdx--;
                    if(parentConnIdx >= insertIdx1) connIdx++;
                    if(parentConnIdx >= insertIdx2) connIdx++;

                    tgtArr[tgtIdx+i] = connIdx;
                }
            }

            #endregion
        }
    }
}
