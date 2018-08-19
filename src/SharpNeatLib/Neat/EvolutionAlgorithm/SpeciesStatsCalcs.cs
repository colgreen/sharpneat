using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redzen.Random;
using SharpNeat.EA;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Neat.EvolutionAlgorithm
{
    /// <summary>
    /// Static method(s) for calculating species statistics. 
    /// </summary>
    /// <typeparam name="T">Connection weight and signal data type.</typeparam>
    public static class SpeciesStatsCalcs<T> where T : struct
    {
        #region Public Static Methods

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
        /// <param name="speciesArr">The array of species.</param>
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
