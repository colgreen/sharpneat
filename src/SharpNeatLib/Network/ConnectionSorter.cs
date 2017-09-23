using System;

namespace SharpNeat.Network
{
    public class ConnectionSorter<T>
        where T : struct
    {
        #region Public Static Methods

        // TODO: Replace this naive sort with a more efficient approach.
        // Notes.
        // Array.Sort() can sort a secondary array, but here we need to sort a third array, the below code
        // does this but performs a lot of unnecessary memory allocation and copying. What is needed is a
        // sort implementation that allows sorting of N secondary arrays.
        public static void Sort<S>(ConnectionIdArrays connIdArrays, S[] weightArr) where S : struct
        {
            // Init array of indexes.
            int[] srcIdArr = connIdArrays._sourceIdArr;
            int[] tgtIdArr = connIdArrays._targetIdArr;

            int[] idxArr = new int[srcIdArr.Length];
            for(int i=0; i<idxArr.Length; i++) {
                idxArr[i] = i;
            }

            // Sort the array of indexes based on the connections that each index points to.
            var comparer = new ConnectionComparer(connIdArrays);
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
    }
}
