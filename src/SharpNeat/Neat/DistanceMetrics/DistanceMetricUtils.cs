// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.InteropServices;

namespace SharpNeat.Neat.DistanceMetrics;

/// <summary>
/// Static utility methods related to genetic distance metrics.
/// </summary>
public static class DistanceMetricUtils
{
    /// <summary>
    /// Calculates the L2/Euclidean centroid for the given set of points.
    /// </summary>
    /// <typeparam name="TWeight">Connection weight data type.</typeparam>
    /// <param name="points">The set of points.</param>
    /// <returns>A new instance of <see cref="ConnectionGenes{T}"/>.</returns>
    /// <remarks>
    /// The Euclidean centroid is a central position within a set of points that minimizes the sum of the squared
    /// distance between each of those points and the centroid. As such it can also be thought of as being
    /// representative of a set of points.
    ///
    /// In Euclidean space the centroid is obtained by calculating the componentwise mean over the set of points.
    /// </remarks>
    public static ConnectionGenes<TWeight> CalculateEuclideanCentroid<TWeight>(
        ReadOnlySpan<ConnectionGenes<TWeight>> points)
        where TWeight : unmanaged, IBinaryFloatingPointIeee754<TWeight>
    {
        // Special case. One item in list, therefore it is the centroid.
        if (points.Length == 1)
            return points[0];

        // This scenario isn't intended to occur; see https://github.com/colgreen/sharpneat-refactor/issues/5
        if (points.Length == 0)
            return new ConnectionGenes<TWeight>(0);

        // ENHANCEMENT: Obtain dictionary from a pool to avoid allocation and initialisation cost on each call to this method.

        // Each coordinate element has an ID. Here we calculate the total for each ID across all CoordinateVectors,
        // then divide the totals by the number of CoordinateVectors to get the average for each ID. That is, we
        // calculate the componentwise mean.
        var coordElemTotals = new Dictionary<DirectedConnection,TWeight>(
            Math.Max(points[0].Length, 32));

        // Loop over coords.
        foreach (ConnectionGenes<TWeight> point in points)
        {
            // Loop over each element within the current coord.
            DirectedConnection[] connArr = point._connArr;
            TWeight[] weightArr = point._weightArr;

            for (int i = 0; i < connArr.Length; i++)
            {
                DirectedConnection conn = connArr[i];
                TWeight weight = weightArr[i];

                // If the ID has previously been encountered then add the current element value to it, otherwise
                // add a new entry to the dictionary.
                ref TWeight weightAcc = ref CollectionsMarshal.GetValueRefOrAddDefault(coordElemTotals, conn, out _);
                weightAcc += weight;
            }
        }

        // Create and return the centroid.
        return CreateCentroid(coordElemTotals, points.Length);
    }

    private static ConnectionGenes<TWeight> CreateCentroid<TWeight>(
        Dictionary<DirectedConnection, TWeight> centroidElements,
        int pointCount)
        where TWeight : unmanaged, IBinaryFloatingPointIeee754<TWeight>
    {
        int length = centroidElements.Count;
        var connGenes = new ConnectionGenes<TWeight>(length);

        var connArr = connGenes._connArr;
        var weightArr = connGenes._weightArr;

        // Copy the unique coord elements from coordElemTotals into arrays, dividing each element's value
        // by the total number of coords as we go.
        TWeight pointCountReciprocol = TWeight.One / TWeight.CreateChecked(pointCount);

        int idx = 0;
        foreach (var elem in centroidElements)
        {
            connArr[idx] = elem.Key;
            weightArr[idx] = elem.Value * pointCountReciprocol;
            idx++;
        }

        // Sort the connection genes.
        connGenes.Sort();

        return connGenes;
    }
}
