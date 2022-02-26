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
using System;
using System.Collections.Generic;
using SharpNeat.Graphs;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.DistanceMetrics.Double;

// TODO: Performance tuning target. E.g. use Math.Fma(), vectorisation, or use single-precision floats for some of the calcs.

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
/// matchDistanceCoeff: When comparing two positions in the same dimension the distance between those two position is
/// multiplied by this coefficient.
///
/// mismatchDistanceCoeff, mismatchDistanceConstant: When comparing two coordinates where one describes a position in a given
/// dimension and the other does not then the second coordinate is assumed to be at position zero in that dimension. However,
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
public sealed class ManhattanDistanceMetric : IDistanceMetric<double>
{
    #region Instance Fields

    // A coefficient to applied to the distance obtained from two coordinates that both
    // describe a position in a given dimension.
    readonly double _matchDistanceCoeff;

    // A coefficient applied to the distance obtained from two coordinates where only one of the coordinates describes
    // a position in a given dimension. The other point is taken to be at position zero in that dimension.
    readonly double _mismatchDistanceCoeff;

    // A constant that is added to the distance where only one of the coordinates describes a position in a given
    // dimension. This adds extra emphasis to distance when comparing coordinates that exist in different dimensions.
    readonly double _mismatchDistanceConstant;

    #endregion

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
    /// This adds extra emphasis to distance when comparing coordinates that exist in different dimensions.</param>
    public ManhattanDistanceMetric(
        double matchDistanceCoeff,
        double mismatchDistanceCoeff,
        double mismatchDistanceConstant)
    {
        _matchDistanceCoeff = matchDistanceCoeff;
        _mismatchDistanceCoeff = mismatchDistanceCoeff;
        _mismatchDistanceConstant = mismatchDistanceConstant;
    }

    #endregion

    #region IDistanceMetric Members

    /// <summary>
    /// Calculates the distance between two positions.
    /// </summary>
    /// <param name="p1">Position one.</param>
    /// <param name="p2">Position two.</param>
    /// <returns>The distance between <paramref name="p1"/> and <paramref name="p2"/>.</returns>
    public double CalcDistance(ConnectionGenes<double> p1, ConnectionGenes<double> p2)
    {
        DirectedConnection[] connArr1 = p1._connArr;
        DirectedConnection[] connArr2 = p2._connArr;
        double[] weightArr1 = p1._weightArr;
        double[] weightArr2 = p2._weightArr;

        // Store these heavily used values locally.
        int length1 = connArr1.Length;
        int length2 = connArr2.Length;

        // Test for special cases.
        if(length1 == 0 && length2 == 0)
        {   // Both arrays are empty. No disparities, therefore the distance is zero.
            return 0.0;
        }

        double distance = 0.0;
        if(length1 == 0)
        {
            // All p2 genes are mismatches.
            for(int i=0; i < length2; i++)
                distance += Math.Abs(weightArr2[i]);

            return (_mismatchDistanceConstant * length2) + (distance * _mismatchDistanceCoeff);
        }

        if(length2 == 0)
        {
            // All p1 elements are mismatches.
            for(int i=0; i < length1; i++)
                distance += Math.Abs(weightArr1[i]);

            return (_mismatchDistanceConstant * length1) + (distance * _mismatchDistanceCoeff);
        }

        // Both arrays contain elements.
        int arr1Idx = 0;
        int arr2Idx = 0;
        DirectedConnection conn1 = connArr1[arr1Idx];
        DirectedConnection conn2 = connArr2[arr2Idx];
        double weight1 = weightArr1[arr1Idx];
        double weight2 = weightArr2[arr2Idx];

        for(;;)
        {
            if(conn1 < conn2)
            {
                // p2 doesn't specify a value in this dimension therefore we take its position to be 0.
                distance += _mismatchDistanceConstant + (Math.Abs(weight1) * _mismatchDistanceCoeff);

                // Move to the next element in p1.
                arr1Idx++;
            }
            else if(conn1 == conn2)
            {
                // Matching elements.
                distance += Math.Abs(weight1 - weight2) * _matchDistanceCoeff;

                // Move to the next element in both arrays.
                arr1Idx++;
                arr2Idx++;
            }
            else // conn2 > conn1
            {
                // p1 doesn't specify a value in this dimension therefore we take its position to be 0.
                distance += _mismatchDistanceConstant + (Math.Abs(weight2) * _mismatchDistanceCoeff);

                // Move to the next element in p2.
                arr2Idx++;
            }

            // Check if we have exhausted one or both of the arrays.
            if(arr1Idx == length1)
            {
                // All remaining p2 elements are mismatches.
                for(int i = arr2Idx; i < length2; i++)
                    distance += _mismatchDistanceConstant + (Math.Abs(weightArr2[i]) * _mismatchDistanceCoeff);

                return distance;
            }

            if(arr2Idx == length2)
            {
                // All remaining p1 elements are mismatches.
                for(int i = arr1Idx; i < connArr1.Length; i++)
                    distance += _mismatchDistanceConstant + (Math.Abs(weightArr1[i]) * _mismatchDistanceCoeff);

                return distance;
            }

            conn1 = connArr1[arr1Idx];
            conn2 = connArr2[arr2Idx];
            weight1 = weightArr1[arr1Idx];
            weight2 = weightArr2[arr2Idx];
        }
    }

    /// <summary>
    /// Tests if the distance between two positions is less than some threshold.
    /// </summary>
    /// <param name="p1">Position one.</param>
    /// <param name="p2">Position two.</param>
    /// <param name="threshold">Distance threshold.</param>
    /// <returns>
    /// True if the distance between <paramref name="p1"/> and <paramref name="p2"/> is less than
    /// <paramref name="threshold"/>.
    /// </returns>
    public bool TestDistance(ConnectionGenes<double> p1, ConnectionGenes<double> p2, double threshold)
    {
        DirectedConnection[] connArr1 = p1._connArr;
        DirectedConnection[] connArr2 = p2._connArr;
        double[] weightArr1 = p1._weightArr;
        double[] weightArr2 = p2._weightArr;

        // Store these heavily used values locally.
        int length1 = connArr1.Length;
        int length2 = connArr2.Length;

        // Test for special case.
        if(length1 == 0 && length2 == 0)
        {   // Both arrays are empty. No disparities, therefore the distance is zero.
            return threshold > 0.0;
        }

        double distance = 0.0;
        if(length1 == 0)
        {
            // All p2 elements are mismatches.
            // p1 doesn't specify a value in these dimensions therefore we take its position to be 0 in all of them.
            for(int i=0; i < length2; i++)
                distance += Math.Abs(weightArr2[i]);

            distance = (_mismatchDistanceConstant * length2) + (distance * _mismatchDistanceCoeff);
            return distance < threshold;
        }

        if(length2 == 0)
        {
            // All p1 elements are mismatches.
            // p2 doesn't specify a value in these dimensions therefore we take its position to be 0 in all of them.
            for(int i=0; i < length1; i++)
                distance += Math.Abs(weightArr1[i]);

            distance = (_mismatchDistanceConstant * length1) + (distance * _mismatchDistanceCoeff);
            return distance < threshold;
        }

        // Both arrays contain elements. Compare the contents starting from the ends where the greatest discrepancies
        // between coordinates are expected to occur. In the general case this should result in less element comparisons
        // before the threshold is passed and we exit the method.
        int arr1Idx = length1 - 1;
        int arr2Idx = length2 - 1;
        DirectedConnection conn1 = connArr1[arr1Idx];
        DirectedConnection conn2 = connArr2[arr2Idx];
        double weight1 = weightArr1[arr1Idx];
        double weight2 = weightArr2[arr2Idx];

        for(;;)
        {
            if(conn1 > conn2)
            {
                // p2 doesn't specify a value in this dimension therefore we take its position to be 0.
                distance += _mismatchDistanceConstant + (Math.Abs(weight1) * _mismatchDistanceCoeff);

                // Move to the next element in p1.
                arr1Idx--;
            }
            else if(conn1 == conn2)
            {
                // Matching elements.
                distance += Math.Abs(weight1 - weight2) * _matchDistanceCoeff;

                // Move to the next element in both arrays.
                arr1Idx--;
                arr2Idx--;
            }
            else // conn2 > conn1
            {
                // p1 doesn't specify a value in this dimension therefore we take its position to be 0.
                distance += _mismatchDistanceConstant + (Math.Abs(weight2) * _mismatchDistanceCoeff);

                // Move to the next element in p12
                arr2Idx--;
            }

            // Test the threshold.
            if(distance >= threshold)
                return false;

            // Check if we have exhausted one or both of the arrays.
            if(arr1Idx < 0)
            {
                // Any remaining p2 elements are mismatches.
                for(int i = arr2Idx; i > -1; i--)
                    distance += _mismatchDistanceConstant + (Math.Abs(weightArr2[i]) * _mismatchDistanceCoeff);

                return distance < threshold;
            }

            if(arr2Idx < 0)
            {
                // All remaining p1 elements are mismatches.
                for(int i = arr1Idx; i > -1; i--)
                    distance += _mismatchDistanceConstant + (Math.Abs(weightArr1[i]) * _mismatchDistanceCoeff);

                return distance < threshold;
            }

            conn1 = connArr1[arr1Idx];
            conn2 = connArr2[arr2Idx];
            weight1 = weightArr1[arr1Idx];
            weight2 = weightArr2[arr2Idx];
        }
    }

    // TODO: Determine mathematically correct centroid. This method calculates the Euclidean distance centroid and
    // is an approximation of the true centroid in L1 space (Manhattan distance).
    // Note. In practice this is possibly a near optimal centroid for all but small clusters.
    /// <summary>
    /// Calculates the centroid for a set of points.
    /// </summary>
    /// <param name="pointList">The set of points.</param>
    /// <returns>A new instance of <see cref="ConnectionGenes{T}"/>.</returns>
    public ConnectionGenes<double> CalculateCentroid(IList<ConnectionGenes<double>> pointList)
    {
        return DistanceMetricUtils.CalculateEuclideanCentroid(pointList);
    }

    #endregion
}
