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
using System.Collections.Generic;
using System.Linq;
using SharpNeat.Neat.Genome;
using SharpNeat.Graphs;

namespace SharpNeat.Neat.DistanceMetrics.Double
{
    /// <summary>
    /// Static utility methods related to genetic distance metrics.
    /// </summary>
    public static class DistanceMetricUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Calculates the L2/Euclidean centroid for the given set of points.
        /// Note. In Euclidean space the centroid is given by calculating the componentwise mean over the
        /// set of points.
        /// </summary>
        /// <remarks>
        /// The euclidean centroid is a central position within a set of points that minimizes the sum of the squared
        /// distance between each of those points and the centroid. As such it can also be thought of as being an exemplar
        /// for a set of points.
        /// </remarks>
        public static ConnectionGenes<double> CalculateEuclideanCentroid(
            IEnumerable<ConnectionGenes<double>> coordList)
        {
            // Special case. One item in list, therefore it is the centroid.
            int count = coordList.Count();

            if(1 == count) {
                return coordList.First();
            }

            // ENHANCEMENT: Obtain dictionary from a pool to avoid allocation and initialisation cost on each call to this method.

            // Each coordinate element has an ID. Here we calculate the total for each ID across all CoordinateVectors,
            // then divide the totals by the number of CoordinateVectors to get the average for each ID. That is, we
            // calculate the componentwise mean.
            var coordElemTotals = new Dictionary<DirectedConnection,double>(64);

            // Loop over coords.
            foreach(ConnectionGenes<double> coord in coordList)
            {
                // Loop over each element within the current coord.
                DirectedConnection[] connArr = coord._connArr;
                double[] weightArr = coord._weightArr;

                for(int i=0; i < connArr.Length; i++)
                {
                    DirectedConnection conn = connArr[i];
                    double weight = weightArr[i];

                    // ENHANCEMENT: Updating an existing entry here requires a second lookup; in principle this could be avoided,
                    // e.g. by using a custom dictionary implementation with InsertOrSum() method.

                    // If the ID has previously been encountered then add the current element value to it, otherwise
                    // add a new entry to the dictionary.
                    if(coordElemTotals.TryGetValue(conn, out double weightAcc)) {
                        coordElemTotals[conn] = weightAcc + weight;
                    }
                    else {
                        coordElemTotals.Add(conn, weight);
                    }
                }
            }

            // Create and return the centroid.
            return CreateCentroid(coordElemTotals, count);
        }

        /// <summary>
        /// Find medoid by comparing each coordinate with every other coordinate. The coord with the lowest
        /// average distance from all other coords is the most central coord (the medoid).
        /// This method uses an inefficient N*N comparison of coords to find a medoid. It is provided only as a last
        /// resort for distance metrics for which no means exist to calculate a centroid.
        /// </summary>
        public static ConnectionGenes<double> FindMedoid(
            IDistanceMetric<double> distanceMetric, IList<ConnectionGenes<double>> coordList)
        {
            // Special case. One item in list, therefore it is the centroid.
            if(1 == coordList.Count) {
                return coordList[0];
            }

            // Find coord that is most central.
            // Handle first coordinate.
            int medoidIdx = 0;
            double medoidDistance = CalculateMeanDistanceFromCoords(distanceMetric, coordList, 0);

            // Handle all other coordinates.
            int count = coordList.Count;
            for(int i=1; i < count; i++)
            {
                double distance = CalculateMeanDistanceFromCoords(distanceMetric, coordList, i);
                if(distance < medoidDistance)
                {
                    // We have a new centroid candidate.
                    medoidDistance = distance;
                    medoidIdx = i;
                }
            }

            // Return the coord that is the medoid.
            return coordList[medoidIdx];
        }

        #endregion

        #region Private Static Methods

        private static ConnectionGenes<double> CreateCentroid(
            Dictionary<DirectedConnection,double> centroidElements,
            int coordCount)
        {
            int length = centroidElements.Count;
            var connGenes = new ConnectionGenes<double>(length);

            var connArr = connGenes._connArr;
            var weightArr = connGenes._weightArr;

            // Copy the unique coord elements from coordElemTotals into arrays, dividing each element's value
            // by the total number of coords as we go.
            double coordCountReciprocol = 1.0 / coordCount;

            int idx = 0;
            foreach(var elem in centroidElements)
            {
                connArr[idx] = elem.Key;
                weightArr[idx] = elem.Value * coordCountReciprocol;
                idx++;
            }

            // Sort the connection genes.
            connGenes.Sort();

            return connGenes;
        }

        /// <summary>
        /// Calculate the mean distance of the specified coord from all of the other coords using
        /// the provided distance metric.
        /// </summary>
        /// <param name="distanceMetric">The distance metric.</param>
        /// <param name="coordList">The list of coordinates.</param>
        /// <param name="idx">The index of the coordinate to measure distance to.</param>
        private static double CalculateMeanDistanceFromCoords(
            IDistanceMetric<double> distanceMetric,
            IList<ConnectionGenes<double>> coordList, int idx)
        {
            double totalDistance = 0.0;
            int count = coordList.Count;
            ConnectionGenes<double> targetCoord = coordList[idx];

            // Measure distance to all coords before the target one.
            for(int i=0; i < idx; i++) {
                totalDistance += distanceMetric.CalcDistance(targetCoord, coordList[i]);
            }

            // Measure distance to all coords after the target one.
            for(int i = idx+1; i < count; i++) {
                totalDistance += distanceMetric.CalcDistance(targetCoord, coordList[i]);
            }

            return totalDistance / (count-1);
        }

        #endregion
    }
}
