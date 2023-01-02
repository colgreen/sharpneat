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
    /// <typeparam name="T">Coordinate component data type.</typeparam>
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
    public static ConnectionGenes<T> CalculateCentroid<T>(
        this IDistanceMetric<T> distanceMetric,
        List<ConnectionGenes<T>> points)
        where T : struct
    {
        return distanceMetric.CalculateCentroid(
            CollectionsMarshal.AsSpan(points));
    }
}
