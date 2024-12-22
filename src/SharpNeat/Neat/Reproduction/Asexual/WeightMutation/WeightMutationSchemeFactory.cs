﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation;

/// <summary>
/// Static factory methods for creation of <see cref="WeightMutationScheme{T}"/> instances.
/// </summary>
public static class WeightMutationSchemeFactory
{
    /// <summary>
    /// Create the default connection weight scheme.
    /// </summary>
    /// <remarks>
    /// The scheme currently in use is taken from SharpNEAT version 2.x, i.e. this is the long standing default scheme
    /// in sharpneat. There has been little to no exploration of what schemes are good and whether this scheme in particular
    /// is good, as such this is a candidate for future research and improvements.
    ///
    /// At time of writing it was thought wise to adopt the scheme from SharpNEAT 2.x because the code base is being
    /// rewritten/refactored and it is therefore useful to keep schemes such as this as close to the previous version as
    /// possible to assist in debugging and testing of the new code base by comparing performance/results, etc. with the 2.x
    /// code base.
    /// </remarks>
    /// <typeparam name="TWeight">Connection weight data type.</typeparam>
    /// <param name="weightScale">Connection weight scale/range.</param>
    /// <returns>A new instance of <see cref="WeightMutationScheme{Double}"/>.</returns>
    public static WeightMutationScheme<TWeight> CreateDefaultScheme<TWeight>(double weightScale)
        where TWeight : unmanaged, IBinaryFloatingPointIeee754<TWeight>
    {
        var probabilityArr = new double[6];
        var strategyArr = new IWeightMutationStrategy<TWeight>[6];

        // Gaussian delta with sigma=0.01 (most values between +-0.02)
        // Mutate 1, 2 and 3 connections respectively.
        probabilityArr[0] = 0.5985;
        probabilityArr[1] = 0.2985;
        probabilityArr[2] = 0.0985;
        strategyArr[0] = CreateCardinalGaussianDeltaStrategy<TWeight>(1, 0.01);
        strategyArr[1] = CreateCardinalGaussianDeltaStrategy<TWeight>(2, 0.01);
        strategyArr[2] = CreateCardinalGaussianDeltaStrategy<TWeight>(3, 0.01);

        // Reset mutations. 1, 2 and 3 connections respectively.
        probabilityArr[3] = 0.015;
        probabilityArr[4] = 0.015;
        probabilityArr[5] = 0.015;
        strategyArr[3] = CreateCardinalUniformResetStrategy<TWeight>(1, weightScale);
        strategyArr[4] = CreateCardinalUniformResetStrategy<TWeight>(2, weightScale);
        strategyArr[5] = CreateCardinalUniformResetStrategy<TWeight>(3, weightScale);

        return new WeightMutationScheme<TWeight>(probabilityArr, strategyArr);
    }

    #region Private Static Methods

    private static DeltaWeightMutationStrategy<TWeight> CreateCardinalGaussianDeltaStrategy<TWeight>(
        int selectCount, double stdDev)
        where TWeight : unmanaged, IBinaryFloatingPointIeee754<TWeight>
    {
        var selectStrategy = new CardinalSubsetSelectionStrategy(selectCount);
        return DeltaWeightMutationStrategy<TWeight>.CreateGaussianDeltaStrategy(selectStrategy, stdDev);
    }

    private static ResetWeightMutationStrategy<TWeight> CreateCardinalUniformResetStrategy<TWeight>(
        int selectCount, double weightScale)
        where TWeight : unmanaged, IBinaryFloatingPointIeee754<TWeight>
    {
        var selectStrategy = new CardinalSubsetSelectionStrategy(selectCount);
        return ResetWeightMutationStrategy<TWeight>.CreateUniformResetStrategy(selectStrategy, weightScale);
    }

    #endregion
}
