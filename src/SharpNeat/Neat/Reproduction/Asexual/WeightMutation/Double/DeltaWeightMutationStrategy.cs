// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Numerics.Distributions;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Double;

/// <summary>
/// A connection weight mutation strategy that applies deltas to existing weights.
/// </summary>
public sealed class DeltaWeightMutationStrategy : IWeightMutationStrategy<double>
{
    readonly ISubsetSelectionStrategy _selectionStrategy;
    readonly IStatelessSampler<double> _weightDeltaSampler;

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="selectionStrategy">Weight selection strategy.</param>
    /// <param name="weightDeltaSampler">Weight delta sampler.</param>
    public DeltaWeightMutationStrategy(
        ISubsetSelectionStrategy selectionStrategy,
        IStatelessSampler<double> weightDeltaSampler)
    {
        _selectionStrategy = selectionStrategy;
        _weightDeltaSampler = weightDeltaSampler;
    }

    #region Public Methods

    /// <inheritdoc/>
    public void Invoke(double[] weightArr, IRandomSource rng)
    {
        // Select a subset of connection genes to mutate.
        int[] selectedIdxArr = _selectionStrategy.SelectSubset(weightArr.Length, rng);

        // Loop over the connection genes to be mutated, and mutate them.
        for(int i=0; i < selectedIdxArr.Length; i++)
            weightArr[selectedIdxArr[i]] += _weightDeltaSampler.Sample(rng);
    }

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Create a weight mutation strategy that applies deltas to a random subset of weights, with deltas sampled from a uniform distribution.
    /// </summary>
    /// <param name="selectionStrategy">Weight selection strategy.</param>
    /// <param name="weightScale">The uniform distribution scale.</param>
    /// <returns>A new instance of <see cref="DeltaWeightMutationStrategy"/>.</returns>
    public static DeltaWeightMutationStrategy CreateUniformDeltaStrategy(
        ISubsetSelectionStrategy selectionStrategy,
        double weightScale)
    {
        var sampler = UniformDistributionSamplerFactory.CreateStatelessSampler<double>(weightScale, true);
        return new DeltaWeightMutationStrategy(selectionStrategy, sampler);
    }

    // TODO: Consider Laplacian distribution.

    /// <summary>
    ///  Create a weight mutation strategy that applies deltas to a random subset of weights, with deltas sampled from a Gaussian distribution.
    /// </summary>
    /// <param name="selectionStrategy">Weight selection strategy.</param>
    /// <param name="stdDev">Gaussian standard deviation.</param>
    /// <returns>A new instance of <see cref="DeltaWeightMutationStrategy"/>.</returns>
    public static DeltaWeightMutationStrategy CreateGaussianDeltaStrategy(
        ISubsetSelectionStrategy selectionStrategy,
        double stdDev)
    {
        var sampler = GaussianDistributionSamplerFactory.CreateStatelessSampler<double>(0, stdDev);
        return new DeltaWeightMutationStrategy(selectionStrategy, sampler);
    }

    #endregion
}
