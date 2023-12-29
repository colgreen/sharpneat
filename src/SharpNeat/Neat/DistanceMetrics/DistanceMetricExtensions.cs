// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Runtime.InteropServices;

namespace SharpNeat.Neat.DistanceMetrics;

/// <summary>
/// Extension methods for <see cref="IDistanceMetric{T}"/>.
/// </summary>
public static class DistanceMetricExtensions
{
    /// <summary>
    /// Calculates the centroid for the given set of points.
    /// </summary>
    /// <typeparam name="TWeight">Connection weight data type.</typeparam>
    /// <param name="distanceMetric">The distance metric instance.</param>
    /// <param name="points">The set of points.</param>
    /// <returns>A new instance of <see cref="ConnectionGenes{T}"/>.</returns>
    /// <remarks>
    /// The centroid is a central position within a set of points that minimizes the sum of the squared distance
    /// between each of those points and the centroid. As such it can also be thought of as being representative
    /// of the set of points.
    ///
    /// The centroid calculation is dependent on the distance metric, hence this method is defined on
    /// <see cref="IDistanceMetric{T}"/>. For some distance metrics the centroid may not be a unique point, in
    /// those cases one of the possible centroids is returned.
    ///
    /// A centroid is used in k-means clustering to define the centre of a cluster.
    /// </remarks>
    public static ConnectionGenes<TWeight> CalculateCentroid<TWeight>(
        this IDistanceMetric<TWeight> distanceMetric,
        List<ConnectionGenes<TWeight>> points)
        where TWeight : struct
    {
        return distanceMetric.CalculateCentroid(
            CollectionsMarshal.AsSpan(points));
    }

    /// <summary>
    /// Find medoid by comparing each coordinate with every other coordinate.
    /// </summary>
    /// <typeparam name="TWeight">Connection weight data type.</typeparam>
    /// <param name="distanceMetric">Distance metric.</param>
    /// <param name="points">The set of points.</param>
    /// <returns>The index of the element in <paramref name="points"/> that is the medoid.</returns>
    /// <remarks>
    /// The coord with the lowest average distance from all other coords is the most central coord (the medoid).
    /// This method uses an inefficient N*N comparison of coords to find a medoid. It is provided only as a last
    /// resort for distance metrics for which no means exist to calculate a centroid.
    /// </remarks>
    public static int FindMedoid<TWeight>(
        this IDistanceMetric<TWeight> distanceMetric,
        ReadOnlySpan<ConnectionGenes<TWeight>> points)
        where TWeight : struct
    {
        // Special case. One item in list, therefore it is the centroid.
        if(points.Length == 1)
            return 0;

        // Find coord that is most central.
        // Handle first coordinate.
        int medoidIdx = 0;
        double medoidDistance = distanceMetric.CalculateMeanDistanceFromAllOtherPoints(points, 0);

        // Handle all other coordinates.
        int count = points.Length;
        for(int i = 1; i < count; i++)
        {
            double distance = distanceMetric.CalculateMeanDistanceFromAllOtherPoints(points, i);
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

    /// <summary>
    /// Calculate the mean distance of the specified point from all of the other points, using the provided
    /// distance metric.
    /// </summary>
    /// <typeparam name="TWeight">Connection weight data type.</typeparam>
    /// <param name="distanceMetric">The distance metric.</param>
    /// <param name="points">The set of points.</param>
    /// <param name="idx">The index of the point to measure mean distance to.</param>
    private static double CalculateMeanDistanceFromAllOtherPoints<TWeight>(
        this IDistanceMetric<TWeight> distanceMetric,
        ReadOnlySpan<ConnectionGenes<TWeight>> points, int idx)
        where TWeight : struct
    {
        double totalDistance = 0.0;
        int count = points.Length;
        ConnectionGenes<TWeight> targetCoord = points[idx];

        // Measure distance to all coords before the target one.
        for(int i = 0; i < idx; i++)
            totalDistance += distanceMetric.CalcDistance(targetCoord, points[i]);

        // Measure distance to all coords after the target one.
        for(int i = idx+1; i < count; i++)
            totalDistance += distanceMetric.CalcDistance(targetCoord, points[i]);

        return totalDistance / (count-1);
    }
}
