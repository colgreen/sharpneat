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
using Redzen.Numerics.Distributions;
using Redzen.Random;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation
{
    /// <summary>
    /// A connection weight mutation strategy that resets connection weights.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public sealed class ResetWeightMutationStrategy<T> : IWeightMutationStrategy<T>
        where T : struct
    {
        readonly ISubsetSelectionStrategy _selectionStrategy;
        readonly IStatelessSampler<T> _weightSampler;

        #region Constructor

        /// <summary>
        /// Construct with the given selection strategy and weight sampler.
        /// </summary>
        /// <param name="selectionStrategy">Weight selection strategy.</param>
        /// <param name="weightSampler">Weight sampler.</param>
        public ResetWeightMutationStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            IStatelessSampler<T> weightSampler)
        {
            _selectionStrategy = selectionStrategy;
            _weightSampler = weightSampler;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invoke the strategy.
        /// </summary>
        /// <param name="weightArr">The connection weight array to apply mutations to.</param>
        /// <param name="rng">Random source.</param>
        public void Invoke(T[] weightArr, IRandomSource rng)
        {
            // Select a subset of connection genes to mutate.
            int[] selectedIdxArr = _selectionStrategy.SelectSubset(weightArr.Length, rng);

            // Loop over the connection genes to be mutated, and mutate them.
            for(int i=0; i < selectedIdxArr.Length; i++)
                weightArr[selectedIdxArr[i]] = _weightSampler.Sample(rng);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Create a weight mutation strategy that replaces a random subset of weights, with new weights sampled
        /// from a uniform distribution.
        /// </summary>
        /// <param name="selectionStrategy">Weight selection strategy.</param>
        /// <param name="weightScale">The uniform distribution scale.</param>
        /// <returns>A new instance of <see cref="ResetWeightMutationStrategy{T}"/>.</returns>
        public static ResetWeightMutationStrategy<T> CreateUniformResetStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            double weightScale)
        {
            var sampler = UniformDistributionSamplerFactory.CreateStatelessSampler<T>(weightScale, true);
            return new ResetWeightMutationStrategy<T>(selectionStrategy, sampler);
        }

        // TODO: Consider Laplacian distribution.

        /// <summary>
        /// Create a weight mutation strategy that replaces a random subset of weights, with new weights sampled
        /// from a Gaussian distribution.
        /// </summary>
        /// <param name="selectionStrategy">Weight selection strategy.</param>
        /// <param name="stdDev">Gaussian standard deviation.</param>
        /// <returns>A new instance of <see cref="ResetWeightMutationStrategy{T}"/>.</returns>
        public static ResetWeightMutationStrategy<T> CreateGaussianResetStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            double stdDev)
        {
            var sampler = GaussianDistributionSamplerFactory.CreateStatelessSampler<T>(0, stdDev);
            return new ResetWeightMutationStrategy<T>(selectionStrategy, sampler);
        }

        #endregion
    }
}
