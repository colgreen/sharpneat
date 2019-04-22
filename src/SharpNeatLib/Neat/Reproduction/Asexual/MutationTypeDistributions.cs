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

namespace SharpNeat.Neat.Reproduction.Asexual
{
    /// <summary>
    /// Container for <see cref="DiscreteDistribution"/> instances that represent discrete probability distributions 
    /// over the set of possible genome mutation types.
    /// </summary>
    public class MutationTypeDistributions
    {
        #region Auto Properties [Mutation Type Distributions]

        /// <summary>
        /// The genome mutation type probability settings represented as a <see cref="DiscreteDistribution"/>.
        /// </summary>
        public DiscreteDistribution MutationTypeDistribution { get; }

        /// <summary>
        /// A copy of <see cref="MutationTypeDistribution"/> but with all destructive mutations (i.e. delete connections)
        /// removed. Useful when e.g. mutating a genome with very few connections.
        /// </summary>
        public DiscreteDistribution MutationTypeDistributionNonDestructive { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="settings">Asexual reproduction settings.</param>
        public MutationTypeDistributions(NeatReproductionAsexualSettings settings)
        {
            this.MutationTypeDistribution = CreateMutationTypeDiscreteDistribution(settings);
            this.MutationTypeDistributionNonDestructive = CreateMutationTypeDiscreteDistribution_NonDestructive(settings);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Create a new instance of <see cref="DiscreteDistribution"/> that represents all of the possible
        /// genome mutation types, and their relative probabilities.
        /// </summary>
        /// <param name="settings">Asexual reproduction settings.</param>
        /// <returns>A new instance of <see cref="DiscreteDistribution"/>.</returns>
        private static DiscreteDistribution CreateMutationTypeDiscreteDistribution(
            NeatReproductionAsexualSettings settings)
        {
            double[] probabilities = new double[] 
                {
                    settings.ConnectionWeightMutationProbability, 
                    settings.AddNodeMutationProbability,
                    settings.AddConnectionMutationProbability,
                    settings.DeleteConnectionMutationProbability
                };
            return new DiscreteDistribution(probabilities);
        }

        /// <summary>
        /// Create a new instance of <see cref="DiscreteDistribution"/> that represents a subset of the possible
        /// genome mutation types, and their relative probabilities. The subset consists of mutation types that 
        /// are non-destructive (i.e. weight mutation, add node mutation, add connection mutation).
        /// </summary>
        /// <param name="settings">Asexual reproduction settings.</param>
        /// <returns>A new instance of <see cref="DiscreteDistribution"/>.</returns>
        private static DiscreteDistribution CreateMutationTypeDiscreteDistribution_NonDestructive(
            NeatReproductionAsexualSettings settings)
        {
            double[] probabilities = new double[] 
                {
                    settings.ConnectionWeightMutationProbability, 
                    settings.AddNodeMutationProbability,
                    settings.AddConnectionMutationProbability
                };
            return new DiscreteDistribution(probabilities);
        }

        #endregion
    }
}
