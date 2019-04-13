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

namespace SharpNeat.Neat.EvolutionAlgorithm
{
    /// <summary>
    /// NEAT evolution algorithm settings.
    /// </summary>
    public class NeatEvolutionAlgorithmSettings
    {
        #region Auto Properties

        /// <summary>
        /// The species count.
        /// </summary>
        public int SpeciesCount { get; set; } = 10;

        /// <summary>
        /// Elitism proportion. 
        /// We sort species genomes by fitness and keep the top N%, the other genomes are
        /// removed to make way for the offspring.
        /// </summary>
        public double ElitismProportion { get; set; } = 0.2;

        /// <summary>
        /// Selection proportion.
        /// We sort species genomes by fitness and select parent genomes for producing offspring from 
        /// the top N%. Selection is performed prior to elitism being applied, therefore selecting from more
        /// genomes than will be made elite is possible.
        /// </summary>
        public double SelectionProportion { get; set; } = 0.2;

        /// <summary>
        /// The proportion of offspring to be produced from asexual reproduction (mutation).
        /// </summary>
        public double OffspringAsexualProportion { get; set; } = 0.5;

        /// <summary>
        /// The proportion of offspring to be produced from sexual reproduction.
        /// </summary>
        public double OffspringSexualProportion { get; set; } = 0.5;

        /// <summary>
        /// The proportion of sexual reproductions that will use genomes from different species.
        /// </summary>
        public double InterspeciesMatingProportion { get; set; } = 0.01;

        /// <summary>
        /// Length of the history buffer used for calculating the moving average for best fitness, mean fitness and mean complexity.
        /// </summary>
        public int StatisticsMovingAverageHistoryLength { get; set; } = 100;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NeatEvolutionAlgorithmSettings()
        {}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public NeatEvolutionAlgorithmSettings(NeatEvolutionAlgorithmSettings copyFrom)
        {
            this.SpeciesCount = copyFrom.SpeciesCount;
            this.ElitismProportion = copyFrom.ElitismProportion;
            this.SelectionProportion = copyFrom.SelectionProportion;
            this.OffspringAsexualProportion = copyFrom.OffspringAsexualProportion;
            this.OffspringSexualProportion = copyFrom.OffspringSexualProportion;
            this.InterspeciesMatingProportion = copyFrom.InterspeciesMatingProportion;
            this.StatisticsMovingAverageHistoryLength = copyFrom.StatisticsMovingAverageHistoryLength;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new settings object based on the current settings object but modified to be suitable for use when 
        /// the evolution algorithm is in simplifying mode.
        /// </summary>
        /// <returns></returns>
        public NeatEvolutionAlgorithmSettings CreateSimplifyingSettings()
        {
            // Clone the current settings object.
            var settings = new NeatEvolutionAlgorithmSettings(this);
            settings.OffspringAsexualProportion = 1.0;
            settings.OffspringSexualProportion = 0.0;
            return settings;
        }

        #endregion
    }
}
