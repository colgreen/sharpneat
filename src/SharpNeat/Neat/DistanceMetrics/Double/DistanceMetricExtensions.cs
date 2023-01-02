// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.DistanceMetrics.Double;

/// <summary>
/// Extension method for distance metrics (double precision).
/// </summary>
public static class DistanceMetricExtensions
{
    /// <summary>
    /// Find medoid by comparing each coordinate with every other coordinate.
    /// </summary>
    /// <param name="distanceMetric">Distance metric.</param>
    /// <param name="points">The set of points.</param>
    /// <returns>The index of the element in <paramref name="points"/> that is the medoid.</returns>
    /// <remarks>
    /// The coord with the lowest average distance from all other coords is the most central coord (the medoid).
    /// This method uses an inefficient N*N comparison of coords to find a medoid. It is provided only as a last
    /// resort for distance metrics for which no means exist to calculate a centroid.
    /// </remarks>
    public static int FindMedoid(
        this IDistanceMetric<double> distanceMetric,
        ReadOnlySpan<ConnectionGenes<double>> points)
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
    /// <param name="distanceMetric">The distance metric.</param>
    /// <param name="points">The set of points.</param>
    /// <param name="idx">The index of the point to measure mean distance to.</param>
    private static double CalculateMeanDistanceFromAllOtherPoints(
        this IDistanceMetric<double> distanceMetric,
        ReadOnlySpan<ConnectionGenes<double>> points, int idx)
    {
        double totalDistance = 0.0;
        int count = points.Length;
        ConnectionGenes<double> targetCoord = points[idx];

        // Measure distance to all coords before the target one.
        for(int i = 0; i < idx; i++)
            totalDistance += distanceMetric.CalcDistance(targetCoord, points[i]);

        // Measure distance to all coords after the target one.
        for(int i = idx+1; i < count; i++)
            totalDistance += distanceMetric.CalcDistance(targetCoord, points[i]);

        return totalDistance / (count-1);
    }
}
