using System;
using System.Diagnostics;
using System.Linq;
using Redzen.Linq;
using Redzen.Numerics;
using Redzen.Random;
using Redzen.Sorting;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Neat.SelectionReproduction
{
    /// <summary>
    /// Static method(s) for calculating species target size allocations. 
    /// </summary>
    /// <typeparam name="T">Connection weight and signal data type.</typeparam>
    public class SpeciesAllocationCalcs<T> where T : struct
    {
        #region Public Static Methods

        /// <summary>
        /// Calc species target sizes based on relative mean fitness of each species, i.e. as per NEAT fitness sharing method.
        /// Store the target sizes on each species' stats object.
        /// </summary>
        public static void CalcAndStoreSpeciesTargetSizes(NeatPopulation<T> pop, IRandomSource rng)
        {            
            // Calculate the new target size of each species using fitness sharing. 
            int totalTargetSizeInt = CalcAndStoreSpeciesTargetSizesInner(pop, rng);

            // Adjust each species' target allocation such that the sum total matches the required population size.
            AdjustSpeciesTargetSizes(pop, totalTargetSizeInt, rng);
        }

        #endregion

        #region Private Static Methods

        private static int CalcAndStoreSpeciesTargetSizesInner(
            NeatPopulation<T> pop, IRandomSource rng)
        {
            double totalMeanFitness = pop.TotalSpeciesMeanFitness;

            // Handle specific case where all genomes/species have a zero fitness. 
            // Assign all species an equal targetSize.
            if(0.0 == totalMeanFitness) {
                return CalcSpeciesTargetSizesInner_ZeroTotalMeanFitness(pop, rng);
            }

            // Calculate the new target size of each species using fitness sharing.
            double popSizeReal = pop.GenomeList.Count;
            Species<T>[] speciesArr = pop.SpeciesArray;
            int totalTargetSizeInt = 0;

            // The size of each specie is based on its fitness relative to the other species.
            for(int i=0; i < speciesArr.Length; i++)
            {
                SpeciesStats stats = speciesArr[i].Stats;
                stats.TargetSizeReal = (stats.MeanFitness / totalMeanFitness) * popSizeReal;

                // Discretize targetSize (stochastic rounding).
                stats.TargetSizeInt = (int)NumericsUtils.ProbabilisticRound(stats.TargetSizeReal, rng);

                // Total up discretized target sizes.
                totalTargetSizeInt += stats.TargetSizeInt;
            }

            return totalTargetSizeInt;
        }

        /// <summary>
        /// Handle specific case where all genomes/species have a zero fitness. 
        /// </summary>
        private static int CalcSpeciesTargetSizesInner_ZeroTotalMeanFitness(NeatPopulation<T> pop, IRandomSource rng)
        {
            // Assign all species an equal targetSize.
            Species<T>[] speciesArr = pop.SpeciesArray;
            double popSizeReal = pop.GenomeList.Count;
            double targetSizeReal = popSizeReal / speciesArr.Length;

            // Keep a total of all allocated target sizes, typically this will vary slightly from the
            // required target population size due to rounding of each real valued target size.
            int totalTargetSizeInt = 0;

            for(int i=0; i < speciesArr.Length; i++) 
            {
                SpeciesStats stats = speciesArr[i].Stats;
                stats.TargetSizeReal = targetSizeReal;

                // Stochastic rounding will result in equal allocation if targetSizeReal is a whole
                // number, otherwise it will help to distribute allocations fairly.
                stats.TargetSizeInt = (int)NumericsUtils.ProbabilisticRound(targetSizeReal, rng);

                // Total up discretized target sizes.
                totalTargetSizeInt += stats.TargetSizeInt;
            }

            return totalTargetSizeInt;
        }

        #endregion

        #region Private Static Methods [Adjust Target Allocations]

        private static void AdjustSpeciesTargetSizes(
            NeatPopulation<T> pop,
            int totalTargetSizeInt,
            IRandomSource rng)
        {
            // Discretized target sizes may total up to a value that is not equal to the required population size. 
            // Here we check this and if the total does not match the required population size then we adjust the
            // species' targetSizeInt values to compensate for the difference.
            int popSize = pop.GenomeList.Count;
            int targetSizeDeltaInt = totalTargetSizeInt - popSize;

            if(targetSizeDeltaInt < 0) 
            {
                // Target size is too low; adjust up.
                AdjustSpeciesTargetSizesUp(pop.SpeciesArray, targetSizeDeltaInt, rng);
            }
            else if(targetSizeDeltaInt > 0)
            {
                // Target size is too high; adjust down.
                AdjustSpeciesTargetSizesDown(pop.SpeciesArray, targetSizeDeltaInt, rng);
            }

            // Ensure a non-zero target size for the species that contains the best genome.
            AdjustSpeciesTargetSizes_AccommodateBestGenomeSpecies(pop, rng);

            // Assert that Sum(TargetSizeInt) == popSize.
            Debug.Assert(pop.SpeciesArray.Sum(x => x.Stats.TargetSizeInt) == popSize);
        }

        private static void AdjustSpeciesTargetSizesUp(
            Species<T>[] speciesArr,
            int targetSizeDeltaInt,
            IRandomSource rng)
        { 
            // We are short of the required populationSize; add the required number of additional allocations to compensate.
            while(targetSizeDeltaInt < 0)
            {
                // Loop through the species in random order, incrementing the target size of each by as we go.
                foreach(var speciesIdx in EnumerableUtils.RangeRandomOrder(0, speciesArr.Length, rng))
                {
                    speciesArr[speciesIdx].Stats.TargetSizeInt++;
                    if(++targetSizeDeltaInt == 0) {
                        break;
                    }
                }

                // If we have looped through all species, but the number of target allocations is still too low, 
                // then go through species loop again. 
                // This is probably a rare execution path, but it's theoretically possible so we cover it.
            }
        }

        private static void AdjustSpeciesTargetSizesDown(
            Species<T>[] speciesArr,
            int targetSizeDeltaInt,
            IRandomSource rng)
        {
            // We have overshot the required populationSize; remove the required number of allocations to compensate.
            while(targetSizeDeltaInt > 0)
            {
                // Loop through the species in random order, decrementing the target size of each by as we go.
                foreach(var speciesIdx in EnumerableUtils.RangeRandomOrder(0, speciesArr.Length, rng))
                {
                    var stats = speciesArr[speciesIdx].Stats;

                    // We can only decrement non-zero allocations.
                    if(stats.TargetSizeInt > 0)
                    {
                        stats.TargetSizeInt--;
                        if(--targetSizeDeltaInt == 0) {
                            break;
                        }
                    }
                }

                // If we have looped through all species, but the number of target allocations is still too high, 
                // then go through species loop again. 
                // This is probably a rare execution path, but it's theoretically possible so we cover it.
            }
        }

        private static void AdjustSpeciesTargetSizes_AccommodateBestGenomeSpecies(
            NeatPopulation<T> pop, IRandomSource rng)
        {
            // Test if the best genome is in a species with a zero target size allocation.
            Species<T>[] speciesArr = pop.SpeciesArray;
            if(speciesArr[pop.BestGenomeSpeciesIdx].Stats.TargetSizeInt > 0)
            {
                // Nothing to do. The best genome is in a species with a non-zero allocation.
                return;
            }

            // Set the target size of the best genome species to a allow the best genome to survive to the next generation.
            speciesArr[pop.BestGenomeSpeciesIdx].Stats.TargetSizeInt++;

            // Adjust down the target size of one of the other species to compensate.
            // Pick a species at random (but not the champ species). Note that this may result in a species with a zero 
            // target size, this is OK at this stage. We handle allocations of zero elsewhere.

            // Create an array of shuffled indexes to select from, i.e. all of the species except for the one with the best genome in it.
            int speciesCount = speciesArr.Length;
            int[] speciesIdxArr = new int[speciesCount - 1];

            for(int i=0; i < pop.BestGenomeSpeciesIdx; i++) {
                speciesIdxArr[i] = i;
            }
            for(int i = pop.BestGenomeSpeciesIdx + 1; i < speciesCount; i++) {
                speciesIdxArr[i-1] = i;
            }
            SortUtils.Shuffle(speciesIdxArr, rng);

            // Loop the species indexes.
            bool success = false;
            foreach(int speciesIdx in speciesIdxArr)
            {
                if(speciesArr[speciesIdx].Stats.TargetSizeInt > 0) 
                {
                    speciesArr[speciesIdx].Stats.TargetSizeInt--;
                    success = true;
                    break;
                }
            }

            if(!success) {
                throw new Exception("All species have a zero target size.");
            }
        }

        #endregion
    }
}
