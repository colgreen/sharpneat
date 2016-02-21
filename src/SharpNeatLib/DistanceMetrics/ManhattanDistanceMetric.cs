/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpNeat.Core;

namespace SharpNeat.DistanceMetrics
{
    /// <summary>
    /// Manhattan distance metric.
    /// 
    /// The Manhattan distance is simply the sum total of all of the distances in each dimension. 
    /// Also known as the taxicab distance, rectilinear distance, L1 distance or L1 norm.
    /// 
    /// Use the default constructor for classical Manhattan Distance.
    /// Optionally the constructor can be provided with a two coefficients and a constant that can be used to modify/distort
    /// distance measures. These are:
    /// 
    /// matchDistanceCoeff - When comparing two positions in the same dimension the distance between those two position is 
    /// multiplied by this coefficient.
    /// 
    /// mismatchDistanceCoeff, mismatchDistanceConstant - When comparing two coordinates where one describes a position in a given 
    /// dimenasion and the other does not then the second coordinate is assumed to be at position zero in that dimension. However,
    /// the resulting distance is multiplied by this coefficient and mismatchDistanceConstant is added, therefore allowing matches and 
    /// mismatches to be weighted differently, e.g. more emphasis can be placed on mismatches (and therefore network topology).
    /// If mismatchDistanceCoeff is zero and mismatchDistanceConstant is non-zero then the distance of mismatches is a fixed value.
    /// 
    /// The two coefficients and constant allow the following schemes:
    /// 
    /// 1) Classical Manhattan distance.
    /// 2) Topology only distance metric (ignore connections weights).
    /// 3) Equivalent of genome distance in Original NEAT (O-NEAT). This is actually a mix of (1) and (2).
    /// </summary>
    public class ManhattanDistanceMetric : IDistanceMetric
    {
        /// <summary>A coefficient to applied to the distance obtained from two coordinates that both 
        /// describe a position in a given dimension.</summary>
        readonly double _matchDistanceCoeff;
        /// <summary>A coefficient applied to the distance obtained from two coordinates where only one of the coordinates describes
        /// a position in a given dimension. The other point is taken to be at position zero in that dimension.</summary>
        readonly double _mismatchDistanceCoeff;
        /// <summary>A constant that is added to the distance where only one of the coordinates describes a position in a given dimension.
        /// This adds extra emphasis to distance when comparing coordinates that exist in different dimesions.</summary>
        readonly double _mismatchDistanceConstant;

        #region Constructors

        /// <summary>
        /// Constructs using default weightings for comparisons on matching and mismatching dimensions.
        /// Classical Manhattan Distance.
        /// </summary>
        public ManhattanDistanceMetric() : this(1.0, 1.0, 0.0)
        {
        }

        /// <summary>
        /// Constructs using the provided weightings for comparisons on matching and mismatching dimensions.
        /// </summary>
        /// <param name="matchDistanceCoeff">A coefficient to applied to the distance obtained from two coordinates that both 
        /// describe a position in a given dimension.</param>
        /// <param name="mismatchDistanceCoeff">A coefficient applied to the distance obtained from two coordinates where only one of the coordinates describes
        /// a position in a given dimension. The other point is taken to be at position zero in that dimension.</param>
        /// <param name="mismatchDistanceConstant">A constant that is added to the distance where only one of the coordinates describes a position in a given dimension.
        /// This adds extra emphasis to distance when comparing coordinates that exist in different dimesions.</param>
        public ManhattanDistanceMetric(double matchDistanceCoeff, double mismatchDistanceCoeff, double mismatchDistanceConstant)
        {
            _matchDistanceCoeff = matchDistanceCoeff;
            _mismatchDistanceCoeff = mismatchDistanceCoeff;
            _mismatchDistanceConstant = mismatchDistanceConstant;
        }

        #endregion

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
                    distance += Math.Abs(arr2[i].Value);
                }
                distance = (_mismatchDistanceConstant * arr2Length) + (distance * _mismatchDistanceCoeff);
                return distance < threshold;
            }

            if(0 == arr2Length)
            {   // All arr1 elements are mismatches.
                // p2 doesn't specify a value in these dimensions therefore we take it's position to be 0 in all of them.
                for(int i=0; i<arr1Length; i++) {
                    distance += Math.Abs(arr1[i].Value);
                }
                distance = (_mismatchDistanceConstant * arr1Length) + (distance * _mismatchDistanceCoeff);
                return distance < threshold;
            }

        //----- Both arrays contain elements. Compare the contents starting from the ends where the greatest discrepancies
        //      between coordinates are expected to occur. In the general case this should result in less element comparisons 
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
                    distance += _mismatchDistanceConstant + (Math.Abs(elem2.Value) * _mismatchDistanceCoeff);

                    // Move to the next element in arr2.
                    arr2Idx--;
                }
                else if(elem1.Key == elem2.Key)
                {
                    // Matching elements.
                    distance += Math.Abs(elem1.Value - elem2.Value) * _matchDistanceCoeff;

                    // Move to the next element in both arrays.
                    arr1Idx--;
                    arr2Idx--;
                }
                else // elem1.Key > elem2.Key
                {
                    // p2 doesn't specify a value in this dimension therefore we take it's position to be 0.
                    distance += _mismatchDistanceConstant + (Math.Abs(elem1.Value) * _mismatchDistanceCoeff);

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
                    for(int i=arr2Idx; i >- 1; i--) {
                        distance += _mismatchDistanceConstant + (Math.Abs(arr2[i].Value) * _mismatchDistanceCoeff);
                    }
                    return distance < threshold;
                }

                if(arr2Idx < 0)
                {   // All remaining arr1 elements are mismatches.
                    for(int i=arr1Idx; i > -1; i--) {
                        distance += _mismatchDistanceConstant + (Math.Abs(arr1[i].Value) * _mismatchDistanceCoeff);
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
                    distance += Math.Abs(arr2[i].Value);
                }
                return (_mismatchDistanceConstant * arr2Length) + (distance * _mismatchDistanceCoeff);
            }

            if(0 == arr2Length)
            {   // All arr1 elements are mismatches.
                for(int i=0; i<arr1Length; i++) {
                    distance += Math.Abs(arr1[i].Value);
                }
                return (_mismatchDistanceConstant * arr1Length) + (distance * _mismatchDistanceCoeff);
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
                    distance += _mismatchDistanceConstant + (Math.Abs(elem1.Value) * _mismatchDistanceCoeff);

                    // Move to the next element in arr1.
                    arr1Idx++;
                }
                else if(elem1.Key == elem2.Key)
                {
                    // Matching elements.
                    distance += Math.Abs(elem1.Value - elem2.Value) * _matchDistanceCoeff;

                    // Move to the next element in both arrays.
                    arr1Idx++;
                    arr2Idx++;
                }
                else // elem2.Key < elem1.Key
                {
                    // p1 doesn't specify a value in this dimension therefore we take it's position to be 0.
                    distance += _mismatchDistanceConstant + (Math.Abs(elem2.Value) * _mismatchDistanceCoeff);

                    // Move to the next element in arr2.
                    arr2Idx++;
                }

                // Check if we have exhausted one or both of the arrays.
                if(arr1Idx == arr1Length)
                {   // All remaining arr2 elements are mismatches.
                    for(int i=arr2Idx; i<arr2Length; i++) {
                        distance += _mismatchDistanceConstant + (Math.Abs(arr2[i].Value) * _mismatchDistanceCoeff);
                    }
                    return distance;
                }

                if(arr2Idx == arr2Length)
                {   // All remaining arr1 elements are mismatches.
                    for(int i=arr1Idx; i<arr1.Length; i++) {
                        distance += _mismatchDistanceConstant + (Math.Abs(arr1[i].Value) * _mismatchDistanceCoeff);
                    }
                    return distance;
                }

                elem1 = arr1[arr1Idx];
                elem2 = arr2[arr2Idx];
            }
        }

        // TODO: Cleanup.
        ///// <summary>
        ///// Calculates the centroid for the given set of points.
        ///// This is perhaps most easy to consider as the center of mass of the set of points (of equal mass).
        ///// 
        ///// The centroid calculation is dependent on the distance metric in use. E.g. for Euclidean distance
        ///// we take the average value on each axis for all the points and this minimizes the total distance from 
        ///// points to centroid. For euclidean distance this has the side effect of also minimizing squared distance,
        ///// but this is not the goal when calculating a centroid.
        ///// 
        ///// For manhattan distance the centroid is thus given by the calculating the median value for each axis, this 
        ///// achieves the goal of minimizing total distance to the centroid but not squared distance. Other distance
        ///// metrics require their own centroid calculation accordingly.
        ///// 
        ///// A centroid is used in k-means clustering to define the center of a cluster.
        ///// </summary>
        ///// <param name="coordList"></param>
        ///// <returns></returns>
        //public CoordinateVector CalculateCentroid(IList<CoordinateVector> coordList)
        //{
        //    // Each coordinate element has an ID. Here we keep a list of each value observed for each ID
        //    // so that we can calculate the median value. 
        //    // Note. Where a coordinate does not specify a position on an axis that other coordinates in 
        //    // the list do specify, that coordinate's position on that axis is taken to be zero. However
        //    // we do not record those zeroes at this stage. We save storage and time by not recording the 
        //    // zeroes and taking them into account later.

        //    // Coord elements within a CoordinateVector must be sorted by ID, therefore we use a 
        //    // SortedDictionary here to eliminate the need to sort elements later.
        //    // We use SortedDictionary and not SortedList for performance, SortedList is fastest for insertion
        //    // only if the inserts are in order (sorted). However this is generally not the case here because although
        //    // cordinate IDs are sorted with a given CoordinateVector, not all IDs exist within all genomes, thus a 
        //    // low ID may be presented to coordElemArrays after a higher ID.
        //    SortedDictionary<ulong, List<double>> coordElemArrays = new SortedDictionary<ulong,List<double>>();

        //    // Loop over coords. Group together values by axis.
        //    int coordCount = coordList.Count;
        //    for(int i=0; i<coordCount; i++)
        //    {
        //        CoordinateVector coordVector = coordList[i];
                
        //        // Loop over each element within the current coord.
        //        foreach(KeyValuePair<ulong,double> coordElem in coordVector.CoordArray)
        //        {
        //            List<double> elemArray;
        //            if(!coordElemArrays.TryGetValue(coordElem.Key, out elemArray))
        //            {
        //                elemArray = new List<double>();
        //                coordElemArrays.Add(coordElem.Key, elemArray);
        //            }
        //            elemArray.Add(coordElem.Value);
        //        }
        //    }

        //    // We now now how many axes the centroid coordinate has. Allocate storage for the centroid coordinate elements.
        //    int centroidElemCount = coordElemArrays.Count;
        //    KeyValuePair<ulong,double>[] centroidElemArr = new KeyValuePair<ulong,double>[centroidElemCount];

        //    // Loop over each axis calculating the mean value for each. We also compensate here for all of the 
        //    // instances where a coordinate didn't specify a position on a given axis and therefore has an 
        //    // implied position of zero.

        //    // Create a temp array with enough capacity to hold an axis value for each coordinate.
        //    double[] tmpArr = new double[coordCount];

        //    int j=0;
        //    foreach(KeyValuePair<ulong,List<double>> coordElemItem in coordElemArrays)
        //    {
        //        List<double> valueList = coordElemItem.Value;

        //        // In total we expect coordCount values. Any missing values are form coordinates with an implied 
        //        // position of zero on the current axis.
        //        int zeroCount = coordCount - valueList.Count;

        //        // Calculate median value.
        //        double median;

        //        // Test special case.
        //        if(zeroCount > valueList.Count)
        //        {   // More than half the values are zero, therefore the median must be zero.
        //            median = 0.0;
        //        }
        //        else if(0 == zeroCount)
        //        {
        //            valueList.Sort();
        //            median = Utilities.CalculateMedian(valueList);
        //        }
        //        else
        //        {
        //            // ENHANCEMENT: We can stop and calculate the median once we reach halfway through the values.

        //            // Combine valueList with the required number of zeroes. Sort the list and calc the median.
        //            // We can save some effort by sorting valueList first and inserting the zeroes in-place afterwards.
        //            valueList.Sort();
                    
        //            // Insert all values below zero.
        //            int valueListCount = valueList.Count;
        //            int valueListIdx = 0;
        //            int k=0;
        //            for(;k<valueListCount && valueList[valueListIdx] < 0.0; valueListIdx++, k++)
        //            {
        //                tmpArr[k] = valueList[valueListIdx];
        //            }
                    
        //            // Insert zeroes.
        //            for(int l=0; l<zeroCount; l++, k++)
        //            {
        //                tmpArr[k] = 0.0;
        //            }

        //            // Insert all values zero or above.
        //            for(;valueListIdx<valueListCount; valueListIdx++, k++)
        //            {
        //                tmpArr[k] = valueList[valueListIdx];
        //            }

        //            median = Utilities.CalculateMedian(tmpArr);
        //        }

        //        // Store centroid coord element.
        //        centroidElemArr[j++] = new KeyValuePair<ulong,double>(coordElemItem.Key, median);
        //    }


        //    CoordinateVector tmp = CalculateCentroid_Euclidean(coordList);


        //    return new CoordinateVector(centroidElemArr);
        //}



        // TODO: Determine mathematically correct centroid. This method calculates the Euclidean distance centroid and
        // is an approximation of the true centroid in L1 space (manhatten distance).
        // Note. In practice this is possibly a near optimal centroid for all but small clusters.
        /// <summary>
        /// Calculates the centroid for the given set of points.
        /// The centroid is a central position within a set of points that minimizes the sum of the squared distance
        /// between each of those points and the centroid. As such it can also be thought of as being an exemplar 
        /// for a set of points.
        /// 
        /// The centroid calculation is dependent on the distance metric, hence this method is defined on IDistanceMetric.
        /// For some distance metrics the centroid may not be a unique point, in those cases one of the possible centroids
        /// is returned.
        /// 
        /// A centroid is used in k-means clustering to define the center of a cluster.
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
            // cordinate IDs are sorted within the source CoordinateVectors, not all IDs exist within all CoordinateVectors
            // therefore a low ID may be presented to coordElemTotals after a higher ID.
            SortedDictionary<ulong, double[]> coordElemTotals = new SortedDictionary<ulong,double[]>();

            // Loop over coords.
            foreach(CoordinateVector coord in coordList)
            {
                // Loop over each element within the current coord.
                foreach(KeyValuePair<ulong,double> coordElem in coord.CoordArray)
                {
                    // If the ID has previously been encountered then add the current element value to it, otherwise
                    // add a new double[1] tp hold the value. 
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
            {   // For speed we multiply by reciprocol instead of dividing by coordCount.
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
            {   // For speed we multiply by reciprocol instead of dividing by coordCount.
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
