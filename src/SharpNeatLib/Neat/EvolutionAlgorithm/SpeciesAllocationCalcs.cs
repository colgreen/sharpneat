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
using System;
using System.Diagnostics;
using System.Linq;
using Redzen.Linq;
using Redzen.Numerics;
using Redzen.Random;
using Redzen.Sorting;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Neat.EvolutionAlgorithm
{
    /// <summary>
    /// Static method(s) for calculating species target size allocations. 
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public class SpeciesAllocationCalcs<T> where T : struct
    {
        #region Public Static Methods

        /// <summary>
        /// Calc and store species target sizes based on relative mean fitness of each species, i.e. as per NEAT fitness sharing method.
        /// Then calc and store the elite, selection and offspring allocations/counts, per species.
        /// </summary>
        public static void CalcAndStoreSpeciesAllocationSizes(
            NeatPopulation<T> pop,
            NeatEvolutionAlgorithmSettings eaSettings,
            IRandomSource rng)
        {            
            // Calculate the new target size of each species using fitness sharing. 
            CalcAndStoreSpeciesTargetSizes(pop, rng);

            // Calculate elite, selection and offspring counts, per species.
            CalculateAndStoreEliteSelectionOffspringCounts(pop, eaSettings, rng);
        }

        #endregion

        #region Private Static Methods

        private static void CalcAndStoreSpeciesTargetSizes(
            NeatPopulation<T> pop, IRandomSource rng)
        {
            double totalMeanFitness = pop.TotalSpeciesMeanFitness;
            int totalTargetSizeInt = 0;

            // Handle specific case where all genomes/species have a zero fitness. 
            // Assign all species an equal targetSize.
            if(0.0 == totalMeanFitness) 
            {
                totalTargetSizeInt = CalcSpeciesTargetSizesInner_ZeroTotalMeanFitness(pop, rng);
            }
            else
            {
                // Calculate the new target size of each species using fitness sharing.
                double popSizeReal = pop.GenomeList.Count;
                Species<T>[] speciesArr = pop.SpeciesArray;

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
            }

            // Adjust each species' target allocation such that the sum total matches the required population size.
            AdjustSpeciesTargetSizes(pop, totalTargetSizeInt, rng);
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
            int targetSizeDeltaInt = totalTargetSizeInt - pop.PopulationSize;

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
            Debug.Assert(pop.SpeciesArray.Sum(x => x.Stats.TargetSizeInt) == pop.PopulationSize);
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

        #region Private Static Methods

        /// <summary>
        /// For each species, allocate the EliteSizeInt, OffspringCount (broken down into OffspringAsexualCount and OffspringSexualCount),
        /// and SelectionSizeInt values.
        /// </summary>
        /// <param name="pop"></param>
        /// <param name="eaSettings"></param>
        /// <param name="rng"></param>
        private static void CalculateAndStoreEliteSelectionOffspringCounts(
            NeatPopulation<T> pop,
            NeatEvolutionAlgorithmSettings eaSettings,
            IRandomSource rng)
        {
            Species<T>[] speciesArr = pop.SpeciesArray;

            // Loop the species, calculating and storing the various size/count properties.
            for(int i=0; i < speciesArr.Length; i++)
            {
                bool isBestGenomeSpecies = (pop.BestGenomeSpeciesIdx == i);
                AllocateEliteSelectionOffspringCounts(speciesArr[i], eaSettings, isBestGenomeSpecies, rng);
            }
        }

        private static void AllocateEliteSelectionOffspringCounts(
            Species<T> species,
            NeatEvolutionAlgorithmSettings eaSettings,
            bool isBestGenomeSpecies,
            IRandomSource rng)
        {
            SpeciesStats stats = species.Stats;

            // Special case - zero target size.
            if(stats.TargetSizeInt == 0) 
            {
                stats.EliteSizeInt = 0;
                stats.OffspringCount = 0;
                stats.OffspringAsexualCount = 0;
                stats.OffspringSexualCount = 0;
                stats.SelectionSizeInt = 0;
                return;
            }

            // Calculate the elite size as a proportion of the current species size.
            // Note. We discretize the real size with a probabilistic handling of the fractional part.
            double eliteSizeReal = species.GenomeList.Count * eaSettings.ElitismProportion;
            int eliteSizeInt = (int)NumericsUtils.ProbabilisticRound(eliteSizeReal, rng);

            // Ensure eliteSizeInt is no larger than the current target size. (I.e. the value was 
            // calculated as a proportion of the current size, not the new target size).
            stats.EliteSizeInt = Math.Min(eliteSizeInt, stats.TargetSizeInt);

            // Special case: ensure the species with the best genome preserves that genome. 
            // Note. This is done even for a target size of one, which would mean that no offspring are
            // produced from the best genome, apart from the (usually small) chance of a cross-species mating.
            if(isBestGenomeSpecies && stats.EliteSizeInt == 0)
            {
                Debug.Assert(stats.TargetSizeInt != 0, "Zero target size assigned to specie that contains the best genome.");
                stats.EliteSizeInt = 1;
            }

            // Determine how many offspring to produce for the species.
            stats.OffspringCount = stats.TargetSizeInt - stats.EliteSizeInt;

            // Determine the split between asexual and sexual reproduction. Again using probabilistic
            // rounding to compensate for any rounding bias.
            double offspringAsexualCountReal = stats.OffspringCount * eaSettings.OffspringAsexualProportion;
            stats.OffspringAsexualCount = (int)NumericsUtils.ProbabilisticRound(offspringAsexualCountReal, rng);
            stats.OffspringSexualCount = stats.OffspringCount - stats.OffspringAsexualCount;

            // Calculate the selectionSize. The number of the species' fittest genomes that are selected from 
            // to create offspring.
            // We ensure this is at least one; if TargetSizeInt is zero then it doesn't matter because no genomes will be
            // selected from this species to produce offspring, except for cross-species mating, hence the minimum of one is 
            // a useful general approach.
            double selectionSizeReal = species.GenomeList.Count * eaSettings.SelectionProportion;
            stats.SelectionSizeInt = Math.Max(1, (int)NumericsUtils.ProbabilisticRound(selectionSizeReal, rng));
        }

        #endregion
    }
}
