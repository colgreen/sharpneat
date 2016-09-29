/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpNeat.Core;

namespace SharpNeat.DistanceMetrics
{
    // TODO: Include coefficients and constant present on ManhattanDistance metric.
    /// <summary>
    /// Euclidean distance metric.
    /// 
    /// The Euclidean distance is given by sqrt(sum(delta^2))
    /// Where [delta] is the absolute position difference in a given dimension (on a given axis).
    /// </summary>
    public class EuclideanDistanceMetric : IDistanceMetric
    {
        #region IDistanceMetric Members

        /// <summary>
        /// Tests if the distance between two positions is less than some threshold.
        /// 
        /// A simple way of implementing this method would be to calculate the distance between the
        /// two coordinates and test if it is less than the threshold. However, that approach requires that all of the
        /// elements in both CoordinateVectors be fully compared. We can improve performance in the general case
        /// by testing if the threshold has been passed after each vector element comparison thus allowing an early exit
        /// from the method for many calls. Further to this, we can begin comparing from the ends of the vectors where 
        /// differences are most likely to occur.
        /// </summary>
        public bool MeasureDistance(CoordinateVector p1, CoordinateVector p2, double threshold)
        {
            // Instead of calculating the euclidean distance we calculate distance squared (we skip the final sqrt 
            // part of the formula). If we then square the threshold value this obviates the need to take the square
            // root when comparing our accumulating calculated distance with the threshold.
            threshold = threshold * threshold;

            KeyValuePair<ulong,double>[] arr1 = p1.CoordArray;
            KeyValuePair<ulong,double>[] arr2 = p2.CoordArray;
            
            // Store these heavily used values locally.
            int arr1Length = arr1.Length;
            int arr2Length = arr2.Length;

        //--- Test for special cases.
            if(0 == arr1Length && 0 == arr2Length)
            {   // Both arrays are empty. No disparities, therefore the distance is zero.
                return 0.0 < threshold;
            }

            double distance = 0.0;
            if(0 == arr1Length)
            {   // All arr2 elements are mismatches.
                // p1 doesn't specify a value in these dimensions therefore we take its position to be 0 in all of them.
                for(int i=0; i<arr2Length; i++) {
                    distance += arr2[i].Value * arr2[i].Value;
                }
                return distance < threshold;
            }

            if(0 == arr2Length)
            {   // All arr1 elements are mismatches.
                // p2 doesn't specify a value in these dimensions therefore we take it's position to be 0 in all of them.
                for(int i=0; i<arr1Length; i++) {
                    distance += arr1[i].Value * arr1[i].Value;
                }
                return distance < threshold;
            }

        //----- Both arrays contain elements. Compare the contents starting from the ends where the greatest discrepancies
        //      between coordinates are expected to occur. Generally this should result in less element comparisons 
        //      before the threshold is passed and we exit the method.
            int arr1Idx = arr1Length - 1;
            int arr2Idx = arr2Length - 1;
            KeyValuePair<ulong,double> elem1 = arr1[arr1Idx];
            KeyValuePair<ulong,double> elem2 = arr2[arr2Idx];
            for(;;)
            {
                if(elem2.Key > elem1.Key)
                {
                    // p1 doesn't specify a value in this dimension therefore we take it's position to be 0.
                    distance += elem2.Value * elem2.Value;

                    // Move to the next element in arr2.
                    arr2Idx--;
                }
                else if(elem1.Key == elem2.Key)
                {
                    // Matching elements. Note that abs() isn't required because we square the result.
                    double tmp = elem1.Value - elem2.Value;
                    distance += tmp * tmp;

                    // Move to the next element in both lists.
                    arr1Idx--;
                    arr2Idx--;
                }
                else // elem1.Key > elem2.Key
                {
                    // p2 doesn't specify a value in this dimension therefore we take it's position to be 0.
                    distance += elem1.Value * elem1.Value;

                    // Move to the next element in arr1.
                    arr1Idx--;
                }

                // Test the threshold.  
                if(distance >= threshold) {
                    return false;
                }

                // Check if we have exhausted one or both of the arrays.
                if(arr1Idx < 0)
                {   // Any remaining arr2 elements are mismatches.
                    for(int i=arr2Idx; i > -1; i--) {
                        distance += arr2[i].Value * arr2[i].Value;
                    }
                    return distance < threshold;
                }

                if(arr2Idx < 0)
                {   // All remaining arr1 elements are mismatches.
                    for(int i=arr1Idx; i > -1; i--) {
                        distance += arr1[i].Value * arr1[i].Value;
                    }
                    return distance < threshold;
                }

                elem1 = arr1[arr1Idx];
                elem2 = arr2[arr2Idx];
            }
        }

        /// <summary>
        /// Measures the distance between two positions.
        /// </summary>
        public double MeasureDistance(CoordinateVector p1, CoordinateVector p2)
        {
            KeyValuePair<ulong,double>[] arr1 = p1.CoordArray;
            KeyValuePair<ulong,double>[] arr2 = p2.CoordArray;
            
            // Store these heavily used values locally.
            int arr1Length = arr1.Length;
            int arr2Length = arr2.Length;

        //--- Test for special cases.
            if(0 == arr1Length && 0 == arr2Length)
            {   // Both arrays are empty. No disparities, therefore the distance is zero.
                return 0.0;
            }

            double distance = 0;
            if(0 == arr1Length)
            {   // All arr2 genes are mismatches.
                for(int i=0; i<arr2Length; i++) {
                    distance += arr2[i].Value * arr2[i].Value;
                }
                return Math.Sqrt(distance);
            }

            if(0 == arr2Length)
            {   // All arr1 elements are mismatches.
                for(int i=0; i<arr1Length; i++) {
                    distance += arr1[i].Value * arr1[i].Value;
                }
                return Math.Sqrt(distance);
            }

        //----- Both arrays contain elements. 
            int arr1Idx = 0;
            int arr2Idx = 0;
            KeyValuePair<ulong,double> elem1 = arr1[arr1Idx];
            KeyValuePair<ulong,double> elem2 = arr2[arr2Idx];
            for(;;)
            {
                if(elem1.Key < elem2.Key)
                {
                    // p2 doesn't specify a value in this dimension therefore we take it's position to be 0.
                    distance += elem1.Value * elem1.Value;

                    // Move to the next element in arr1.
                    arr1Idx++;
                }
                else if(elem1.Key == elem2.Key)
                {
                    // Matching elements. Note that abs() isn't required because we square the result.
                    double tmp = elem1.Value - elem2.Value;
                    distance += tmp * tmp;

                    // Move to the next element in both arrays.
                    arr1Idx++;
                    arr2Idx++;
                }
                else // elem2.Key < elem1.Key
                {
                    // p1 doesn't specify a value in this dimension therefore we take it's position to be 0.
                    distance += elem2.Value * elem2.Value;

                    // Move to the next element in arr2.
                    arr2Idx++;
                }

                // Check if we have exhausted one or both of the arrays.
                if(arr1Idx == arr1.Length)
                {   // All remaining arr2 elements are mismatches.
                    for(int i=arr2Idx; i<arr2Length; i++) {
                        distance += arr2[i].Value * arr2[i].Value;
                    }
                    return Math.Sqrt(distance);
                }

                if(arr2Idx == arr2Length)
                {   // All remaining arr1 elements are mismatches.
                    for(int i=arr1Idx; i<arr1.Length; i++) {
                        distance += arr1[i].Value * arr1[i].Value;
                    }
                    return Math.Sqrt(distance);
                }

                elem1 = arr1[arr1Idx];
                elem2 = arr2[arr2Idx];
            }
        }

        /// <summary>
        /// Calculates the centroid for the given set of points.
        /// For the Euclidean distance metric the centroid is given by calculating the componentwise mean over the
        /// set of points.
        /// </summary>
        public CoordinateVector CalculateCentroid(IList<CoordinateVector> coordList)
        {
            // Special case - one item in list, it *is* the centroid.
            if(1 == coordList.Count) {
                return coordList[0];
            }

            // Each coordinate element has an ID. Here we calculate the total for each ID across all CoordinateVectors,
            // then divide the totals by the number of CoordinateVectors to get the average for each ID. That is, we 
            // calculate the component-wise mean.
            //
            // Coord elements within a CoordinateVector must be sorted by ID, therefore we use a SortedDictionary here 
            // when building the centroid coordinate to eliminate the need to sort elements later.
            //
            // We use SortedDictionary and not SortedList for performance. SortedList is fastest for insertion
            // only if the inserts are in order (sorted). However, this is generally not the case here because although
            // coordinate IDs are sorted within the source CoordinateVectors, not all IDs exist within all CoordinateVectors
            // therefore a low ID may be presented to coordElemTotals after a higher ID.
            SortedDictionary<ulong, double[]> coordElemTotals = new SortedDictionary<ulong,double[]>();

            // Loop over coords.
            foreach(CoordinateVector coord in coordList)
            {
                // Loop over each element within the current coord.
                foreach(KeyValuePair<ulong,double> coordElem in coord.CoordArray)
                {
                    // If the ID has previously been encountered then add the current element value to it, otherwise
                    // add a new double[1] to hold the value.. 
                    // Note that we wrap the double value in an object so that we do not have to re-insert values
                    // to increment them. In tests this approach was about 40% faster (including GC overhead).
                    double[] doubleWrapper;
                    if(coordElemTotals.TryGetValue(coordElem.Key, out doubleWrapper)) {
                        doubleWrapper[0] += coordElem.Value;
                    }
                    else {
                        coordElemTotals.Add(coordElem.Key, new double[]{coordElem.Value});
                    }
                }
            }

            // Put the unique coord elems from coordElemTotals into a list, dividing each element's value
            // by the total number of coords as we go.
            double coordCountReciprocol = 1.0 / (double)coordList.Count;
            KeyValuePair<ulong,double>[] centroidElemArr = new KeyValuePair<ulong,double>[coordElemTotals.Count];
            int i=0;
            foreach(KeyValuePair<ulong,double[]> coordElem in coordElemTotals)
            {   // For speed we multiply by reciprocal instead of dividing by coordCount.
                centroidElemArr[i++] = new KeyValuePair<ulong,double>(coordElem.Key, coordElem.Value[0] * coordCountReciprocol);
            }

            // Use the new list of elements to construct a centroid CoordinateVector.
            return new CoordinateVector(centroidElemArr);
        }

        /// <summary>
        /// Parallelized version of CalculateCentroid().
        /// </summary>
        public CoordinateVector CalculateCentroidParallel(IList<CoordinateVector> coordList)
        {
            // Special case - one item in list, it *is* the centroid.
            if (1 == coordList.Count)
            {
                return coordList[0];
            }

            // Each coordinate element has an ID. Here we calculate the total for each ID across all CoordinateVectors,
            // then divide the totals by the number of CoordinateVectors to get the average for each ID. That is, we 
            // calculate the component-wise mean.

            // ConcurrentDictionary provides a low-locking strategy that greatly improves performance here 
            // compared to using mutual exclusion locks or even ReadWriterLock(s).
            ConcurrentDictionary<ulong,double[]> coordElemTotals = new ConcurrentDictionary<ulong, double[]>();

            // Loop over coords.
            Parallel.ForEach(coordList, delegate(CoordinateVector coord)
            {
                // Loop over each element within the current coord.
                foreach (KeyValuePair<ulong, double> coordElem in coord.CoordArray)
                {
                    // If the ID has previously been encountered then add the current element value to it, otherwise
                    // add a new double[1] to hold the value. 
                    // Note that we wrap the double value in an object so that we do not have to re-insert values
                    // to increment them. In tests this approach was about 40% faster (including GC overhead).

                    // If position is zero then (A) skip doing any work, and (B) zero will break the following logic.
                    if (coordElem.Value == 0.0) {
                        continue;
                    }

                    double[] doubleWrapper;
                    if (coordElemTotals.TryGetValue(coordElem.Key, out doubleWrapper))
                    {   // By locking just the specific object that holds the value we are incrementing
                        // we greatly reduce the amount of lock contention.
                        lock(doubleWrapper)
                        {
                            doubleWrapper[0] += coordElem.Value;
                        }
                    }
                    else
                    {
                        doubleWrapper = new double[] { coordElem.Value };
                        if (!coordElemTotals.TryAdd(coordElem.Key, doubleWrapper))
                        {
                            if(coordElemTotals.TryGetValue(coordElem.Key, out doubleWrapper))
                            {
                                lock (doubleWrapper)
                                {
                                    doubleWrapper[0] += coordElem.Value;
                                }
                            }
                        }
                    }
                }
            });

            // Put the unique coord elems from coordElemTotals into a list, dividing each element's value
            // by the total number of coords as we go.
            double coordCountReciprocol = 1.0 / (double)coordList.Count;
            KeyValuePair<ulong, double>[] centroidElemArr = new KeyValuePair<ulong, double>[coordElemTotals.Count];
            int i = 0;
            foreach (KeyValuePair<ulong, double[]> coordElem in coordElemTotals)
            {   // For speed we multiply by reciprocal instead of dividing by coordCount.
                centroidElemArr[i++] = new KeyValuePair<ulong, double>(coordElem.Key, coordElem.Value[0] * coordCountReciprocol);
            }

            // Coord elements within a CoordinateVector must be sorted by ID.
            Array.Sort(centroidElemArr, delegate(KeyValuePair<ulong, double> x, KeyValuePair<ulong, double> y)
            {
                if (x.Key < y.Key) {
                    return -1;
                }
                if (x.Key > y.Key) {
                    return 1;
                }
                return 0;
            });

            // Use the new list of elements to construct a centroid CoordinateVector.
            return new CoordinateVector(centroidElemArr);
        }

        #endregion
    }
}
