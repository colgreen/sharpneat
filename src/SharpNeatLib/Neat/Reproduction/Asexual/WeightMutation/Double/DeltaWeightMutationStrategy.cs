/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
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

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Double
{
    /// <summary>
    /// A connection weight mutation strategy that applies deltas to existing weights.
    /// </summary>
    public class DeltaWeightMutationStrategy: IWeightMutationStrategy<double>
    {
        readonly ISubsetSelectionStrategy _selectionStrategy;
        readonly IStatelessSampler<double> _weightDeltaSampler;

        #region Constructor

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Invoke the strategy.
        /// </summary>
        /// <param name="weightArr">The connection weight array to apply mutations to.</param>
        /// <param name="rng">Random source.</param>
        public void Invoke(double[] weightArr, IRandomSource rng)
        {
            // Select a subset of connection genes to mutate.
            int[] selectedIdxArr = _selectionStrategy.SelectSubset(weightArr.Length, rng);

            // Loop over the connection genes to be mutated, and mutate them.
            for(int i=0; i<selectedIdxArr.Length; i++) {
                weightArr[selectedIdxArr[i]] += _weightDeltaSampler.Sample(rng);
            }
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
        ///  Create a weight mutation strategy that applies deltas to a random subset of weights, with deltas sampled from a gaussian distribution.
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
}
