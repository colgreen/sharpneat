using System;
using System.Collections.Generic;

namespace SharpNeat.Graphs
{
    public sealed class ConnectionSorterV1
    {
        #region Public Static Methods

        // Notes.
        // Array.Sort() can sort a secondary array, but here we need to sort a third array; the below code
        // does this but performs a lot of unnecessary memory allocation and copying. As such this logic was
        // replaced (see current implementation of SharpNeat.Graphs.ConnectionSorter) with a customised sort
        // routine that is faster and more efficient w.r.t memory allocations and copying.

        public static void Sort<S>(in ConnectionIdArrays connIdArrays, S[] weightArr) where S : struct
        {
            // Init array of indexes.
            int[] srcIdArr = connIdArrays._sourceIdArr;
            int[] tgtIdArr = connIdArrays._targetIdArr;

            int[] idxArr = new int[srcIdArr.Length];
            for(int i=0; i<idxArr.Length; i++) {
                idxArr[i] = i;
            }

            // Sort the array of indexes based on the connections that each index points to.
            var comparer = new ConnectionComparer(in connIdArrays);
            Array.Sort(idxArr, comparer);

            int len = srcIdArr.Length;
            int[] srcIdArr2 = new int[len];
            int[] tgtIdArr2 = new int[len];
            S[] weightArr2 = new S[len];

            for(int i=0; i<len; i++)
            {
                int j = idxArr[i];
                srcIdArr2[i] = srcIdArr[j];
                tgtIdArr2[i] = tgtIdArr[j];
                weightArr2[i] = weightArr[j];
            }

            Array.Copy(srcIdArr2, srcIdArr, len);
            Array.Copy(tgtIdArr2, tgtIdArr, len);
            Array.Copy(weightArr2, weightArr, len);
        }

        #endregion

        #region Private Static Methods

        private sealed class ConnectionComparer : IComparer<int>
        {
            readonly int[] _srcIdArr;
            readonly int[] _tgtIdArr;

            public ConnectionComparer(in ConnectionIdArrays connIdArrays)
            {
                _srcIdArr = connIdArrays._sourceIdArr;
                _tgtIdArr = connIdArrays._targetIdArr;
            }

            public int Compare(int x, int y)
            {
                // Compare source IDs.
                int xval = _srcIdArr[x];
                int yval = _srcIdArr[y];

                if(xval < yval) {
                    return -1;
                }
                else if(xval > yval) {
                    return 1;
                }

                // Source IDs are equal; compare target IDs.
                xval = _tgtIdArr[x];
                yval = _tgtIdArr[y];

                if(xval < yval) {
                    return -1;
                }
                else if(xval > yval) {
                    return 1;
                }

                return 0;
            }
        }

        #endregion
    }
}
