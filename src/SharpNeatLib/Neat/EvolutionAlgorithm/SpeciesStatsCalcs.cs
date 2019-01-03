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
using System.Linq;
using Redzen.Random;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Neat.EvolutionAlgorithm
{
    /// <summary>
    /// Static method(s) for calculating species statistics. 
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public static class SpeciesStatsCalcs<T> where T : struct
    {
        #region Public Static Methods

        /// <summary>
        /// Calculate and update a number of statistical values and target size values on each species in the givben population.
        /// </summary>
        /// <param name="pop">The population to update species statistics on.</param>
        /// <param name="eaSettings">Evolution algorithm settings.</param>
        /// <param name="rng">Random source.</param>
        public static void CalcAndStoreSpeciesStats(
            NeatPopulation<T> pop,
            NeatEvolutionAlgorithmSettings eaSettings,
            IRandomSource rng)
        {
            // Calc and store the mean fitness of each species.
            CalcAndStoreSpeciesFitnessMeans(pop);

            // Calc and store the target size of each species (based on the NEAT fitness sharing method).
            SpeciesAllocationCalcs<T>.CalcAndStoreSpeciesAllocationSizes(pop, eaSettings, rng);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Calc mean fitness of all species, and store the results on each species' stats object.
        /// </summary>
        /// <param name="pop">The population.</param>
        /// <returns>Sum of species mean fitnesses.</returns>
        private static void CalcAndStoreSpeciesFitnessMeans(NeatPopulation<T> pop)
        {
            Species<T>[] speciesArr = pop.SpeciesArray;

            // Calc mean fitness of all species, and sum of all species means.
            double totalMeanFitness = 0.0;
            for(int i=0; i < speciesArr.Length; i++)
            {
                var species = speciesArr[i];
                double meanFitness = species.GenomeList.Average(x => x.FitnessInfo.PrimaryFitness);
                species.Stats.MeanFitness = meanFitness;
                totalMeanFitness += meanFitness;
            }

            pop.TotalSpeciesMeanFitness = totalMeanFitness;
        }

        #endregion
    }
}
