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
using System.Collections.Generic;
using SharpNeat.Core;

namespace SharpNeat.DistanceMetrics
{
    /// <summary>
    /// Static helper methods for distance metrics.
    /// </summary>
    public class DistanceMetricUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Calculates a centroid by comparing each coordinate with every other coordinate. The coord with the lowest 
        /// average distance from all other coords is the most central coord (the centroid).
        /// This method uses an inefficient N*N comparison of coords to find a centroid. It is provided only as a last
        /// resort for distance metrics for which no means exist to calculate a centroid more directly.
        /// </summary>
        public static CoordinateVector CalculateCentroid(IDistanceMetric distanceMetric, IList<CoordinateVector> coordList)
        {
            // Test special case - one coord therefore it is the centroid..
            if(1 == coordList.Count) {   
                return new CoordinateVector(coordList[0].CoordArray);
            }

            // Find coord that is most central.
            int centroidIdx = 0;
            double centroidDistance = CalculateMeanDistanceFromCoords(distanceMetric, coordList, 0);
            int count = coordList.Count;
            for(int i=1; i<count; i++)
            {
                double distance = CalculateMeanDistanceFromCoords(distanceMetric, coordList, i);
                if(distance < centroidDistance)
                {   // We have a new centroid candidate.
                    centroidDistance = distance;
                    centroidIdx = i;
                }
            }

            // We make a copy of the element to avoid any problems (CoordinateVector is intended to used as
            // an immutable type but it isn't actually immutable)
            return new CoordinateVector(coordList[centroidIdx].CoordArray);
        }
        
        /// <summary>
        /// Calculate the mean distance of the specified coord from all of the other coords using
        /// the provided distance metric.
        /// </summary>
        /// <param name="distanceMetric">The distance metric.</param>
        /// <param name="coordList">The list of coordinates.</param>
        /// <param name="idx">The index of the coordinate to measure distance to.</param>
        private static double CalculateMeanDistanceFromCoords(IDistanceMetric distanceMetric, IList<CoordinateVector> coordList, int idx)
        {
            double totalDistance = 0.0;
            int count = coordList.Count;
            CoordinateVector targetCoord = coordList[idx];

            // Measure distance to all coords before the target one.
            for(int i=0; i<idx; i++) {
                totalDistance += distanceMetric.GetDistance(targetCoord, coordList[i]);
            }

            // Measure distance to all coords after the target one.
            for(int i=idx+1; i<count; i++) {
                totalDistance += distanceMetric.GetDistance(targetCoord, coordList[i]);
            }

            return totalDistance / (count-1);
        }

        #endregion
    }
}
