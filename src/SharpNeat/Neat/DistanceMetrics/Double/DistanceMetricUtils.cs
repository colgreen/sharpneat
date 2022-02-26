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
using SharpNeat.Graphs;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.DistanceMetrics.Double;

/// <summary>
/// Static utility methods related to genetic distance metrics.
/// </summary>
public static class DistanceMetricUtils
{
    #region Public Static Methods

    /// <summary>
    /// Calculates the L2/Euclidean centroid for the given set of points.
    /// </summary>
    /// <param name="pointList">The set of points.</param>
    /// <returns>A new instance of <see cref="ConnectionGenes{T}"/>.</returns>
    /// <remarks>
    /// The euclidean centroid is a central position within a set of points that minimizes the sum of the squared
    /// distance between each of those points and the centroid. As such it can also be thought of as being an exemplar
    /// for a set of points.
    ///
    /// In Euclidean space the centroid is obtained by calculating the componentwise mean over the set of points.
    /// </remarks>
    public static ConnectionGenes<double> CalculateEuclideanCentroid(
        IList<ConnectionGenes<double>> pointList)
    {
        if(pointList.Count == 1)
        {   // Special case. One item in list, therefore it is the centroid.
            return pointList[0];
        }
        else if(pointList.Count == 0)
        {   // This scenario isn't intended to occur; see https://github.com/colgreen/sharpneat-refactor/issues/5
            return new ConnectionGenes<double>(0);
        }

        // ENHANCEMENT: Obtain dictionary from a pool to avoid allocation and initialisation cost on each call to this method.

        // Each coordinate element has an ID. Here we calculate the total for each ID across all CoordinateVectors,
        // then divide the totals by the number of CoordinateVectors to get the average for each ID. That is, we
        // calculate the componentwise mean.
        var coordElemTotals = new Dictionary<DirectedConnection,double>(
            Math.Max(pointList[0].Length, 32));

        // Loop over coords.
        foreach(ConnectionGenes<double> point in pointList)
        {
            // Loop over each element within the current coord.
            DirectedConnection[] connArr = point._connArr;
            double[] weightArr = point._weightArr;

            for(int i=0; i < connArr.Length; i++)
            {
                DirectedConnection conn = connArr[i];
                double weight = weightArr[i];

                // ENHANCEMENT: Updating an existing entry here requires a second lookup; in principle this could be avoided,
                // e.g. by using a custom dictionary implementation with InsertOrSum() method.

                // If the ID has previously been encountered then add the current element value to it, otherwise
                // add a new entry to the dictionary.
                // TODO: [.NET 6+] Use Marshal.GetValueRefOrAddDefault here to avoid the second lookup for adding a missing item.
                if(coordElemTotals.TryGetValue(conn, out double weightAcc))
                {
                    coordElemTotals[conn] = weightAcc + weight;
                }
                else
                {
                    coordElemTotals.Add(conn, weight);
                }
            }
        }

        // Create and return the centroid.
        return CreateCentroid(coordElemTotals, pointList.Count);
    }

    /// <summary>
    /// Find medoid by comparing each coordinate with every other coordinate. The coord with the lowest
    /// average distance from all other coords is the most central coord (the medoid).
    /// This method uses an inefficient N*N comparison of coords to find a medoid. It is provided only as a last
    /// resort for distance metrics for which no means exist to calculate a centroid.
    /// </summary>
    /// <param name="distanceMetric">Distance metric.</param>
    /// <param name="pointList">Point list.</param>
    /// <returns>The index of the element in <paramref name="pointList"/> that is the medoid.</returns>
    public static int FindMedoid(
        IDistanceMetric<double> distanceMetric,
        IList<ConnectionGenes<double>> pointList)
    {
        // Special case. One item in list, therefore it is the centroid.
        if(pointList.Count == 1)
            return 0;

        // Find coord that is most central.
        // Handle first coordinate.
        int medoidIdx = 0;
        double medoidDistance = CalculateMeanDistanceFromCoords(distanceMetric, pointList, 0);

        // Handle all other coordinates.
        int count = pointList.Count;
        for(int i=1; i < count; i++)
        {
            double distance = CalculateMeanDistanceFromCoords(distanceMetric, pointList, i);
            if(distance < medoidDistance)
            {
                // We have a new centroid candidate.
                medoidDistance = distance;
                medoidIdx = i;
            }
        }

        // Return the coord that is the medoid.
        return medoidIdx;
    }

    #endregion

    #region Private Static Methods

    private static ConnectionGenes<double> CreateCentroid(
        Dictionary<DirectedConnection,double> centroidElements,
        int pointCount)
    {
        int length = centroidElements.Count;
        var connGenes = new ConnectionGenes<double>(length);

        var connArr = connGenes._connArr;
        var weightArr = connGenes._weightArr;

        // Copy the unique coord elements from coordElemTotals into arrays, dividing each element's value
        // by the total number of coords as we go.
        double pointCountReciprocol = 1.0 / pointCount;

        int idx = 0;
        foreach(var elem in centroidElements)
        {
            connArr[idx] = elem.Key;
            weightArr[idx] = elem.Value * pointCountReciprocol;
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
        for(int i=0; i < idx; i++)
            totalDistance += distanceMetric.CalcDistance(targetCoord, coordList[i]);

        // Measure distance to all coords after the target one.
        for(int i = idx+1; i < count; i++)
            totalDistance += distanceMetric.CalcDistance(targetCoord, coordList[i]);

        return totalDistance / (count-1);
    }

    #endregion
}
