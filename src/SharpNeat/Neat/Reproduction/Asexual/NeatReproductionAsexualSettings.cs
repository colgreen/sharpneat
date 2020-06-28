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
        public double ConnectionWeightMutationProbability { get; set; } = 0.94;

        /// <summary>
        /// Probability that a genome mutation is an 'add node' mutation.
        /// </summary>
        public double AddNodeMutationProbability { get; set; } = 0.01;

        /// <summary>
        /// Probability that a genome mutation is an 'add connection' mutation.
        /// </summary>
        public double AddConnectionMutationProbability { get; set; } = 0.025;

        /// <summary>
        /// Probability that a genome mutation is a 'delete connection' mutation.
        /// </summary>
        public double DeleteConnectionMutationProbability { get; set; } = 0.025;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NeatReproductionAsexualSettings()
        {}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public NeatReproductionAsexualSettings(NeatReproductionAsexualSettings copyFrom)
        { 
            this.ConnectionWeightMutationProbability = copyFrom.ConnectionWeightMutationProbability;
            this.AddNodeMutationProbability = copyFrom.AddNodeMutationProbability;
            this.AddConnectionMutationProbability = copyFrom.AddConnectionMutationProbability;
            this.DeleteConnectionMutationProbability = copyFrom.DeleteConnectionMutationProbability;
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
            // Invoke the copy constructor with the current object.
            //
            // Note. Currently all of the settings are modified, therefore it's not necessary to use the copy constructor
            // however, if additional settings are added to the settings class then they will be handled automatically here 
            // without having to update this code, so this is a slightly safer approach.
            var settings = new NeatReproductionAsexualSettings(this)
            {
                ConnectionWeightMutationProbability = 0.6,
                AddNodeMutationProbability = 0.0,
                AddConnectionMutationProbability = 0.0,
                DeleteConnectionMutationProbability = 0.4
            };
            return settings;
        }

        #endregion
    }
}
