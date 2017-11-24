using System.Diagnostics;

namespace SharpNeat.Network
{
    /// <summary>
    /// A variant on ArraySortHelper in the core framework:
    /// 
    ///    https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Collections/Generic/ArraySortHelper.cs
    ///    
    /// This version is customised for sorting network connections. I.e.. sort order is based on both source and target node IDs 
    /// (which are held in separate arrays), and a separate array of weights is re-ordered to keep the weights at the same array 
    /// index as their respective source and target IDs.
    /// 
    /// This functionality can be achieved by using the various sort() methods in the core framework, but less efficiently than 
    /// by this customised class (in terms of both speed, RAM allocations and thus GC overhead).
    /// </summary>
    public class ConnectionSorter<T>
    {
        #region Statics / Consts

        // This is the threshold where Introspective sort switches to Insertion sort.
        // Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
        // Large value types may benefit from a smaller number.
        const int __introsortSizeThreshold = 17;

        #endregion
        
        #region Public Static Methods

        public static void Sort(ConnectionIdArrays connIdArrays, T[] weightArr)
        {
            Debug.Assert(connIdArrays._sourceIdArr != null);
            Debug.Assert(connIdArrays._targetIdArr != null);
            Debug.Assert(weightArr != null);
            Debug.Assert(connIdArrays._sourceIdArr.Length == connIdArrays._targetIdArr.Length);
            Debug.Assert(connIdArrays._sourceIdArr.Length == weightArr.Length);

            IntrospectiveSort(connIdArrays._sourceIdArr, connIdArrays._targetIdArr, weightArr, 0, connIdArrays._sourceIdArr.Length);
        }

        #endregion

        #region Private Static Methods [Intro Sort]

        private static void IntrospectiveSort(int[] srcIdArr, int[] tgtIdArrTKey, T[] weightArr, int left, int length)
        {
            Debug.Assert(left >= 0);
            Debug.Assert(length >= 0);
            Debug.Assert(length <= srcIdArr.Length);
            Debug.Assert(length + left <= srcIdArr.Length);

            if (length < 2)
                return;

            IntroSortInner(srcIdArr, tgtIdArrTKey, weightArr, left, length + left - 1, 2 * FloorLog2(srcIdArr.Length));
        }

        private static void IntroSortInner(int[] srcIdArr, int[] tgtIdArr, T[] weightArr, int lo, int hi, int depthLimit)
        {
            Debug.Assert(lo >= 0);
            Debug.Assert(hi < srcIdArr.Length);

            while (hi > lo)
            {
                int partitionSize = hi - lo + 1;
                if (partitionSize < __introsortSizeThreshold)
                {
                    if (partitionSize == 1) {
                        return;
                    }
                    if (partitionSize == 2) 
                    {
                        SwapIfGreaterWithItems(srcIdArr, tgtIdArr, weightArr, lo, hi);
                        return;
                    }
                    if (partitionSize == 3) 
                    {
                        SwapIfGreaterWithItems(srcIdArr, tgtIdArr, weightArr, lo, hi - 1);
                        SwapIfGreaterWithItems(srcIdArr, tgtIdArr, weightArr, lo, hi);
                        SwapIfGreaterWithItems(srcIdArr, tgtIdArr, weightArr, hi - 1, hi);
                        return;
                    }

                    InsertionSort(srcIdArr, tgtIdArr, weightArr, lo, hi);
                    return;
                }

                if (depthLimit == 0)
                {
                    Heapsort(srcIdArr, tgtIdArr, weightArr, lo, hi);
                    return;
                }
                depthLimit--;

                int p = PickPivotAndPartition(srcIdArr, tgtIdArr, weightArr, lo, hi);
                // Note we've already partitioned around the pivot and do not have to move the pivot again.
                IntroSortInner(srcIdArr, tgtIdArr, weightArr, p + 1, hi, depthLimit);
                hi = p - 1;
            }
        }

        private static int PickPivotAndPartition(int[] srcIdArr, int[] tgtIdArr, T[] weightArr, int lo, int hi)
        {   
            Debug.Assert(lo >= 0);
            Debug.Assert(hi > lo);
            Debug.Assert(hi < srcIdArr.Length);

            // Compute median-of-three.  But also partition them, since we've done the comparison.
            int middle = lo + ((hi - lo) / 2);

            // Sort lo, mid and hi appropriately, then pick mid as the pivot.
            SwapIfGreaterWithItems(srcIdArr, tgtIdArr, weightArr, lo, middle);  // swap the low with the mid point
            SwapIfGreaterWithItems(srcIdArr, tgtIdArr, weightArr, lo, hi);      // swap the low with the high
            SwapIfGreaterWithItems(srcIdArr, tgtIdArr, weightArr, middle, hi);  // swap the middle with the high

            //TKey pivot = keys[middle];
            int pivotSrcId = srcIdArr[middle];
            int pivotTgtId = tgtIdArr[middle];

            Swap(srcIdArr, tgtIdArr, weightArr, middle, hi - 1);
            int left = lo, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

            while (left < right)
            {
                while (Compare(srcIdArr[++left], tgtIdArr[left], pivotSrcId, pivotTgtId) < 0);
                while (Compare(pivotSrcId, pivotTgtId, srcIdArr[--right], tgtIdArr[right]) < 0);

                if (left >= right) {
                    break;
                }

                Swap(srcIdArr, tgtIdArr, weightArr, left, right);
            }

            // Put pivot in the right location.
            Swap(srcIdArr, tgtIdArr, weightArr, left, (hi - 1));

            Debug.Assert(left >= lo && left <= hi);
            return left;
        }

        private static void SwapIfGreaterWithItems(int[] srcIdArr, int[] tgtIdArr, T[] weightArr, int a, int b)
        {
            if (a != b && Compare(srcIdArr[a], tgtIdArr[a], srcIdArr[b], tgtIdArr[b]) > 0)
            {
                int id = srcIdArr[a];
                srcIdArr[a] = srcIdArr[b];
                srcIdArr[b] = id;

                id = tgtIdArr[a];
                tgtIdArr[a] = tgtIdArr[b];
                tgtIdArr[b] = id;

                T w = weightArr[a];
                weightArr[a] = weightArr[b];
                weightArr[b] = w;        
            }   
        }

        private static void Swap(int[] srcIdArr, int[] tgtIdArr, T[] weightArr, int i, int j)
        {
            if (i != j)
            {
                int id = srcIdArr[i];
                srcIdArr[i] = srcIdArr[j];
                srcIdArr[j] = id;

                id = tgtIdArr[i];
                tgtIdArr[i] = tgtIdArr[j];
                tgtIdArr[j] = id;

                T w = weightArr[i];
                weightArr[i] = weightArr[j];
                weightArr[j] = w;
            }
        }

        #endregion

        #region Private Static Methods [Insertion Sort]

        private static void InsertionSort(int[] srcIdArr, int[] tgtIdArr, T[] weightArr, int lo, int hi)
        {
            Debug.Assert(lo >= 0);
            Debug.Assert(hi >= lo);
            Debug.Assert(hi <= srcIdArr.Length);

            int i, j;
            int srcId;
            int tgtId;
            T weight;
            
            for (i = lo; i < hi; i++)
            {
                j = i;
                srcId = srcIdArr[i+1];
                tgtId = tgtIdArr[i+1];
                weight = weightArr[i+1];

                while (j >= lo &&  Compare(srcId, tgtId, srcIdArr[j], tgtIdArr[j]) < 0)
                {
                    srcIdArr[j + 1] = srcIdArr[j];
                    tgtIdArr[j + 1] = tgtIdArr[j];
                    weightArr[j + 1] = weightArr[j];
                    j--;
                }

                srcIdArr[j + 1] = srcId;
                tgtIdArr[j + 1] = tgtId;
                weightArr[j + 1] = weight;
            }
        }

        #endregion

        #region Private Static Methods [Heap Sort]

        private static void Heapsort(int[] srcIdArr, int[] tgtIdArr, T[] weightArr, int lo, int hi)
        {
            Debug.Assert(lo >= 0);
            Debug.Assert(hi > lo);
            Debug.Assert(hi < srcIdArr.Length);

            int n = hi - lo + 1;
            for (int i = n / 2; i >= 1; i = i - 1)
            {
                DownHeap(srcIdArr, tgtIdArr, weightArr, i, n, lo);
            }
            for (int i = n; i > 1; i = i - 1)
            {
                Swap(srcIdArr, tgtIdArr, weightArr, lo, lo + i - 1);
                DownHeap(srcIdArr, tgtIdArr, weightArr, 1, i - 1, lo);
            }
        }

        private static void DownHeap(int[] srcIdArr, int[] tgtIdArr, T[] weightArr, int i, int n, int lo)
        {
            Debug.Assert(lo >= 0);
            Debug.Assert(lo < srcIdArr.Length);

            int srcId = srcIdArr[lo + i - 1];
            int tgtId = tgtIdArr[lo + i - 1];
            T weight = weightArr[lo + i - 1];

            int child;
            while (i <= n / 2)
            {
                child = 2 * i;
                if (child < n && Compare(srcIdArr[lo + child - 1], tgtIdArr[lo + child - 1], srcIdArr[lo + child], tgtIdArr[lo + child]) < 0) {
                    child++;
                }
                if (!(Compare(srcId, tgtId, srcIdArr[lo + child - 1], tgtIdArr[lo + child - 1]) < 0)) {
                    break;
                }

                srcIdArr[lo + i - 1] = srcIdArr[lo + child - 1];
                tgtIdArr[lo + i - 1] = tgtIdArr[lo + child - 1];
                weightArr[lo + i - 1] = weightArr[lo + child - 1];
                i = child;
            }

            srcIdArr[lo + i - 1] = srcId;
            tgtIdArr[lo + i - 1] = tgtId;
            weightArr[lo + i - 1] = weight;
        }

        #endregion

        #region Private Static Methods [Misc]

        private static int Compare(int srcIdA, int tgtIdA, int srcIdB, int tgtIdB)
        {
            if(srcIdA < srcIdB) {
                return -1;
            }
            if(srcIdA > srcIdB) {
                return 1;
            }

            if(tgtIdA < tgtIdB) {
                return -1;
            }
            if(tgtIdA > tgtIdB) {
                return 1;
            }
            return 0;
        }

        private static int FloorLog2(int n)
        {
            int result = 0;
            while (n >= 1)
            {
                result++;
                n = n / 2;
            }
            return result;
        }

        #endregion
    }
}
