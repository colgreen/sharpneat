using System.Diagnostics;
using System.Linq;
using Redzen.Linq;
using Redzen.Numerics;
using Redzen.Random;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Neat.SelectionReproduction
{
    public class SpeciesAllocationCalcs<T> where T : struct
    {
        #region Public Static Methods

        /// <summary>
        /// Calc species target sizes based on relative mean fitness of each species, i.e. as per NEAT fitness sharing method.
        /// </summary>
        public static void CalcSpeciesTargetSizes(NeatPopulation<T> pop, IRandomSource rng)
        {
            // Calc mean fitness of all species, and get sum of all species means.
            double totalMeanFitness = UpdateSpeciesFitnessMeans(pop.SpeciesArray);
                        
            // Calculate the new target size of each species using fitness sharing. 
            int totalTargetSizeInt = CalcSpeciesTargetSizes(pop, totalMeanFitness, rng);

            // Adjust each species' target allocation such that the sum total matches the required population size.
            AdjustSpeciesTargetSizes(pop, totalTargetSizeInt, rng);
        }

        #endregion

        #region Private Static Methods [Initial Allocation]

        /// <summary>
        /// Calc mean fitness of all species, and return sum of all species means.
        /// </summary>
        /// <param name="speciesArr">The array of species.</param>
        /// <returns>Sum of species mean fitnesses.</returns>
        private static double UpdateSpeciesFitnessMeans(Species<T>[] speciesArr)
        {
            // Calc mean fitness of all species, and sum of all species means.
            double totalMeanFitness = 0.0;
            for(int i=0; i < speciesArr.Length; i++)
            {
                var species = speciesArr[i];
                double meanFitness = species.GenomeList.Average(x => x.FitnessInfo.PrimaryFitness);
                species.Stats.MeanFitness = meanFitness;
                totalMeanFitness += meanFitness;
            }
            return totalMeanFitness;
        }

        private static int CalcSpeciesTargetSizes(
            NeatPopulation<T> pop, double totalMeanFitness, IRandomSource rng)
        {
            // Handle specific case where all genomes/species have a zero fitness. 
            // Assign all species an equal targetSize.
            if(0.0 == totalMeanFitness) {
                return CalcSpeciesTargetSizes_ZeroTotalMeanFitness(pop, rng);
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
        private static int CalcSpeciesTargetSizes_ZeroTotalMeanFitness(NeatPopulation<T> pop, IRandomSource rng)
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

        #region Private Static Methods [Adjust Allocations]

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

        #endregion
    }
}
