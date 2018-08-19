using System;
using System.Collections.Generic;
using System.Text;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Neat.EvolutionAlgorithm
{
    /// <summary>
    /// Static method(s) for calculating population stats. 
    /// </summary>
    /// <typeparam name="T">Connection weight and signal data type.</typeparam>
    public static class PopulationStatsCalcs<T> where T : struct
    {
        #region Public Static Methods

        public static void CalcAndStorePopulationStats(NeatPopulation<T> pop)
        {
            // TODO: Calc more stats!

            CalcAndStoreBestGenome(pop);
        }

        #endregion

        #region Private Static Methods

        private static void CalcAndStoreBestGenome(NeatPopulation<T> pop)
        {
            // Note. If all genomes have the same fitness then we simply return the first genome. This will typically occur
            // for zero fitness, but could occur for any fitness score.
            NeatGenome<T> bestGenome = null;
            double bestFitness = -1.0;
            int bestSpecieIdx = -1;

            Species<T>[] speciesArr = pop.SpeciesArray;

            for(int i=0; i < speciesArr.Length; i++)
            {
                // Get the species' first genome; genomes are sorted fittest first, therefore this is also the fittest 
                // genome in the species.
                var genome = speciesArr[i].GenomeList[0];
                if(genome.FitnessInfo.PrimaryFitness > bestFitness)
                {
                    bestGenome = genome;
                    bestFitness = genome.FitnessInfo.PrimaryFitness;
                    bestSpecieIdx = i;
                }
            }

            // Store species index.
            pop.BestGenomeSpeciesIdx = bestSpecieIdx;

            // Scan to find the index of the best genome in the full population genome list.
            for(int i=0; i < pop.GenomeList.Count; i++)
            {
                if(pop.GenomeList[i] == bestGenome)
                {
                    pop.BestGenomeIdx = i;
                    break;
                }
            }
        }

        #endregion
    }
}
