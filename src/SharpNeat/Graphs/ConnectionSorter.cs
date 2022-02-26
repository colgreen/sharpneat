/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Diagnostics;
using System.Numerics;

namespace SharpNeat.Graphs;

/// <summary>
/// A variant on ArraySortHelper in the core framework:
///
///    https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Collections/Generic/ArraySortHelper.cs
///
/// This version is customised for sorting network connections. I.e. sort order is based on both source and
/// target node IDs (which are held in separate arrays), and a separate array of values is re-ordered to keep
/// the values at the same array index as their respective source and target IDs.
///
/// This functionality could be achieved by using the various sort() methods in the core framework, but less
/// efficiently than with this customised class, in terms of both speed, and also RAM allocations (and thus GC overhead).
/// </summary>
/// <typeparam name="T">Value data type.</typeparam>
public static class ConnectionSorter<T>
{
    // This is the threshold at which Introspective sort switches to Insertion sort.
    // Empirically 24 seems to give near optimal performance here, noting that this has been increased from the
    // value of 16 defined in ArraySortHelper.cs
    const int __introsortSizeThreshold = 24;

    #region Public Static Methods

    /// <summary>
    /// Sort the connections represented by <paramref name="connIds"/>, and an accompanying values span.
    /// </summary>
    /// <param name="connIds">Represents the endpoint IDs of the connections to sort.</param>
    /// <param name="vals">A span of values that will have its items reordered in the same way as <paramref name="connIds"/>.</param>
    public static void Sort(
        in ConnectionIds connIds,
        Span<T> vals)
    {
        Debug.Assert(connIds.GetSourceIdSpan().Length == connIds.GetTargetIdSpan().Length);
        Debug.Assert(connIds.GetSourceIdSpan().Length == vals.Length);

        IntrospectiveSort(
            connIds.GetSourceIdSpan(),
            connIds.GetTargetIdSpan(),
            vals);
    }

    #endregion

    #region Private Static Methods [Introspective Sort]

    private static void IntrospectiveSort(
        Span<int> srcIds,
        Span<int> tgtIds,
        Span<T> vals)
    {
        if(srcIds.Length < 2)
            return;

        IntroSortInner(
            srcIds, tgtIds, vals,
            2 * (BitOperations.Log2((uint)srcIds.Length) + 1));
    }

    private static void IntroSortInner(
        Span<int> srcIds,
        Span<int> tgtIds,
        Span<T> vals,
        int depthLimit)
    {
        Debug.Assert(!srcIds.IsEmpty);
        Debug.Assert(depthLimit >= 0);

        int partitionSize = srcIds.Length;
        while(partitionSize > 1)
        {
            if(partitionSize <= __introsortSizeThreshold)
            {
                if(partitionSize == 2)
                {
                    SwapIfGreater(srcIds, tgtIds, vals, 0, 1);
                    return;
                }

                if(partitionSize == 3)
                {
                    SwapIfGreater(srcIds, tgtIds, vals, 0, 1);
                    SwapIfGreater(srcIds, tgtIds, vals, 0, 2);
                    SwapIfGreater(srcIds, tgtIds, vals, 1, 2);
                    return;
                }

                InsertionSort(
                    srcIds.Slice(0, partitionSize),
                    tgtIds.Slice(0, partitionSize),
                    vals.Slice(0, partitionSize));
                return;
            }

            if(depthLimit == 0)
            {
                Heapsort(
                    srcIds.Slice(0, partitionSize),
                    tgtIds.Slice(0, partitionSize),
                    vals.Slice(0, partitionSize));
                return;
            }
            depthLimit--;

            int p = PickPivotAndPartition(
                srcIds.Slice(0, partitionSize),
                tgtIds.Slice(0, partitionSize),
                vals.Slice(0, partitionSize));

            // Note we've already partitioned around the pivot and do not have to move the pivot again.
            IntroSortInner(
                srcIds[(p+1)..partitionSize],
                tgtIds[(p+1)..partitionSize],
                vals[(p+1)..partitionSize],
                depthLimit);

            partitionSize = p;
        }
    }

    private static int PickPivotAndPartition(
        Span<int> srcIds,
        Span<int> tgtIds,
        Span<T> vals)
    {
        Debug.Assert(srcIds.Length >= __introsortSizeThreshold);

        // Use median-of-three to select a pivot. Define indexes to Length-1th, and Length/2th elements, and sort them.
        int len = srcIds.Length;
        int last = len - 1;
        int middle = (last) >> 1;
        SwapIfGreater(srcIds, tgtIds, vals, 0, middle);
        SwapIfGreater(srcIds, tgtIds, vals, 0, last);
        SwapIfGreater(srcIds, tgtIds, vals, middle, last);

        // Select the middle value as the pivot, and move it to be just before the last element.
        int nextToLast = len - 2;
        int pivotSrcId = srcIds[middle];
        int pivotTgtId = tgtIds[middle];
        Swap(srcIds, tgtIds, vals, middle, nextToLast);

        // Walk the left and right indexes, swapping elements as necessary, until they cross.
        int left = 0, right = nextToLast;
        while(left < right)
        {
            while(Compare(ref srcIds[++left], ref tgtIds[left], ref pivotSrcId, ref pivotTgtId) < 0);
            while(Compare(ref pivotSrcId, ref pivotTgtId, ref srcIds[--right], ref tgtIds[right]) < 0);

            if(left >= right)
                break;

            Swap(srcIds, tgtIds, vals, left, right);
        }

        // Put pivot in the right location.
        if(left != nextToLast)
        {
            Swap(srcIds, tgtIds, vals, left, len - 2);
        }

        return left;
    }

    #endregion

    #region Private Static Methods [Heap Sort]

    private static void Heapsort(
        Span<int> srcIds,
        Span<int> tgtIds,
        Span<T> vals)
    {
        Debug.Assert(!srcIds.IsEmpty);

        int n = srcIds.Length;
        for(int i = n >> 1; i >= 1; i--)
        {
            DownHeap(srcIds, tgtIds, vals, i, n);
        }

        for(int i = n; i > 1; i--)
        {
            Swap(srcIds, tgtIds, vals, 0, i - 1);
            DownHeap(srcIds, tgtIds, vals, 1, i - 1);
        }
    }

    private static void DownHeap(
        Span<int> srcIds,
        Span<int> tgtIds,
        Span<T> vals,
        int i, int n)
    {
        int srcId = srcIds[i - 1];
        int tgtId = tgtIds[i - 1];
        T val = vals[i - 1];

        while(i <= n >> 1)
        {
            int child = 2 * i;
            if(child < n && Compare(ref srcIds[child - 1], ref tgtIds[child - 1], ref srcIds[child], ref tgtIds[child]) < 0)
            {
                child++;
            }

            if(Compare(ref srcId, ref tgtId, ref srcIds[child - 1], ref tgtIds[child - 1]) >=0)
                break;

            srcIds[i - 1] = srcIds[child - 1];
            tgtIds[i - 1] = tgtIds[child - 1];
            vals[i - 1] = vals[child - 1];
            i = child;
        }

        srcIds[i - 1] = srcId;
        tgtIds[i - 1] = tgtId;
        vals[i - 1] = val;
    }

    #endregion

    #region Private Static Methods [Insertion Sort]

    private static void InsertionSort(
        Span<int> srcIds,
        Span<int> tgtIds,
        Span<T> vals)
    {
        for(int i = 0; i < srcIds.Length - 1; i++)
        {
            int j = i;
            int srcId = srcIds[i + 1];
            int tgtId = tgtIds[i + 1];
            T val = vals[i + 1];

            while(j >= 0 && Compare(ref srcId, ref tgtId, ref srcIds[j], ref tgtIds[j]) < 0)
            {
                srcIds[j + 1] = srcIds[j];
                tgtIds[j + 1] = tgtIds[j];
                vals[j + 1] = vals[j];
                j--;
            }

            srcIds[j + 1] = srcId;
            tgtIds[j + 1] = tgtId;
            vals[j + 1] = val;
        }
    }

    #endregion

    #region Private Static Methods [Misc]

    private static void SwapIfGreater(
        Span<int> srcIds,
        Span<int> tgtIds,
        Span<T> vals,
        int a, int b)
    {
        if(Compare(ref srcIds[a], ref tgtIds[a], ref srcIds[b], ref tgtIds[b]) > 0)
        {
            int id = srcIds[a];
            srcIds[a] = srcIds[b];
            srcIds[b] = id;

            id = tgtIds[a];
            tgtIds[a] = tgtIds[b];
            tgtIds[b] = id;

            T w = vals[a];
            vals[a] = vals[b];
            vals[b] = w;
        }
    }

    private static void Swap(
        Span<int> srcIds,
        Span<int> tgtIds,
        Span<T> vals,
        int i, int j)
    {
        Debug.Assert(i != j);

        int id = srcIds[i];
        srcIds[i] = srcIds[j];
        srcIds[j] = id;

        id = tgtIds[i];
        tgtIds[i] = tgtIds[j];
        tgtIds[j] = id;

        T w = vals[i];
        vals[i] = vals[j];
        vals[j] = w;
    }

    private static int Compare(ref int srcIdA, ref int tgtIdA, ref int srcIdB, ref int tgtIdB)
    {
        // Notes.
        // Return a negative number if connection A is before B in the sort order.
        // Return a positive number if connection B is before A in the sort order.
        // Return zero if connections A and B are equal (have the same source and target node IDs).
        // The strictly correct way of doing this is with a series of comparisons, like so:
        //
        //    if(srcIdA < srcIdB) return -1;
        //    if(srcIdA > srcIdB) return 1;
        //
        //    if(tgtIdA < tgtIdB) return -1;
        //    if(tgtIdA > tgtIdB) return 1;
        //    return 0;
        //
        // Instead we use arithmetic subtraction to effectively compare the source and target IDs of
        // connections A and B, this results in the elimination of all but one of the above conditional
        // branches, and therefore improved performance.
        //
        // Noting that the range of an Int32 is [-2^31, (2^31)-1], hence in the extreme case where a node with
        // ID int.MaxValue is subtracted from node ID zero, the result does not overflow. However, it's
        // unlikely that the node ID space would ever reach 2 billion+; if that is expected then the node ID
        // type would have to be increase to a 64 bit integer anyway.
        int diff = srcIdA - srcIdB;
        if(diff != 0) return diff;
        return tgtIdA - tgtIdB;
    }

    #endregion
}
