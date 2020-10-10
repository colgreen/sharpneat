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
using Redzen.Numerics.Distributions.Double;
using Redzen.Random;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation
{
    /// <summary>
    /// Connection weight mutation scheme.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class WeightMutationScheme<T> 
        where T : struct
    {
        readonly DiscreteDistribution _strategySelectionDist;
        readonly IWeightMutationStrategy<T>[] _mutationStrategyArr;

        #region Constructor

        /// <summary>
        /// Construct a new instance with the given strategy arguments.
        /// </summary>
        /// <param name="strategyProbabilityArr">An array of strategy selection probabilities.</param>
        /// <param name="mutationStrategyArr">An array of weight mutation strategies.</param>
        public WeightMutationScheme(
            double[] strategyProbabilityArr,
            IWeightMutationStrategy<T>[] mutationStrategyArr)
        {
            _strategySelectionDist = new DiscreteDistribution(strategyProbabilityArr);
            _mutationStrategyArr = mutationStrategyArr;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Mutate the connection weights based on a stochastically chosen <see cref="IWeightMutationStrategy{T}"/>.
        /// </summary>
        /// <param name="weightArr">The connection weight array to apply mutations to.</param>
        /// <param name="rng">Random source.</param>
        public void MutateWeights(T[] weightArr, IRandomSource rng)
        {
            // Select a mutation strategy, and apply it to the array of connection genes.
            int strategyIdx = DiscreteDistribution.Sample(rng, _strategySelectionDist);
            var strategy = _mutationStrategyArr[strategyIdx];
            strategy.Invoke(weightArr, rng);
        }

        #endregion
    }
}
