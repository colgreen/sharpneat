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
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Neat
{
    /// <summary>
    /// NeatPopulation statistics.
    /// </summary>
    public class NeatPopulationStatistics : PopulationStatistics
    {
        #region Auto Properties [NeatPopulation Statistics]

        /// <summary>
        /// Index of the species that the best genome is within.
        /// </summary>
        public int BestGenomeSpeciesIdx { get; set; }

        /// <summary>
        /// Sum of species fitness means.
        /// </summary>
        public double SumSpeciesMeanFitness { get; set; }

        /// <summary>
        /// The average (mean) fitness calculated over all species' best/champ genomes.
        /// </summary>
        public double AverageSpeciesBestFitness { get; set; }

        #endregion
    }
}
