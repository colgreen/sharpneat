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
    /// Settings related to <see cref="NeatReproductionAsexual{T}"/>.
    /// </summary>
    public class NeatReproductionAsexualSettings
    {
        #region Auto Properties [Genome Mutation Settings]

        /// <summary>
        /// Probability that a genome mutation is a connection weights mutation.
        /// </summary>
        public double ConnectionWeightMutationProbability { get; }

        /// <summary>
        /// Probability that a genome mutation is an 'add node' mutation.
        /// </summary>
        public double AddNodeMutationProbability { get; }

        /// <summary>
        /// Probability that a genome mutation is an 'add connection' mutation.
        /// </summary>
        public double AddConnectionMutationProbability { get; }

        /// <summary>
        /// Probability that a genome mutation is a 'delete connection' mutation.
        /// </summary>
        public double DeleteConnectionMutationProbability { get; }

        #endregion

        #region Auto Properties [Mutation Type Distributions]

        /// <summary>
        /// The mutation type probability settings represented as a <see cref="DiscreteDistribution"/>.
        /// </summary>
        public DiscreteDistribution MutationTypeDistribution { get; }
        /// <summary>
        /// A copy of MutationTypeDistribution but with all destructive mutations (i.e. delete connections)
        /// removed. Useful when e.g. mutating a genome with very few connections.
        /// </summary>
        public DiscreteDistribution MutationTypeDistributionNonDestructive { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NeatReproductionAsexualSettings()
            : this(
                connectionWeightMutationProbability: 0.94,
                addNodeMutationProbability: 0.01,
                addConnectionMutationProbability: 0.025,
                deleteConnectionMutationProbability: 0.025)
        {}

        /// <summary>
        /// Construct a new instance with the provided settings.
        /// </summary>
        public NeatReproductionAsexualSettings(
            double connectionWeightMutationProbability,
            double addNodeMutationProbability,
            double addConnectionMutationProbability,
            double deleteConnectionMutationProbability)
        {
            // Store settings.
            this.ConnectionWeightMutationProbability = connectionWeightMutationProbability;
            this.AddNodeMutationProbability = addNodeMutationProbability;
            this.AddConnectionMutationProbability = addConnectionMutationProbability;
            this.DeleteConnectionMutationProbability = deleteConnectionMutationProbability;

            // Create discrete distributions over the mutation type probabilities.
            this.MutationTypeDistribution = CreateMutationTypeDiscreteDistribution(this);
            this.MutationTypeDistributionNonDestructive = CreateMutationTypeDiscreteDistribution_NonDestructive(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new settings object based on the current settings object but modified to be suitable for use when 
        /// the evolution algorithm is in simplifying mode.
        /// </summary>
        /// <returns></returns>
        public NeatReproductionAsexualSettings CreateSimplifyingSettings()
        {
            // Note. Currently all of the settings are modified, therefore this method could be static,
            // however, if additional settings are added that need to be copied into the new settings object
            // then they will be passed from the current object.
            return new NeatReproductionAsexualSettings(
                connectionWeightMutationProbability: 0.6,
                addNodeMutationProbability: 0.0,
                addConnectionMutationProbability: 0.0,
                deleteConnectionMutationProbability: 0.4);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Create a new instance of <see cref="DiscreteDistribution"/> that represents all of the possible
        /// genome mutation types, and their relative probabilities.
        /// </summary>
        /// <param name="settings">Settings object that conveys the mutation type probabilities.</param>
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
        /// <param name="settings">Settings object that conveys the mutation type probabilities.</param>
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
