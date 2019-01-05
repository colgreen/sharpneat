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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Redzen.Random;
using SharpNeat.Neat.DistanceMetrics;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Speciation.Parallelized;

namespace SharpNeat.Neat.Speciation.GeneticKMeans.Parallelized
{
    /// <summary>
    /// A speciation strategy that assigns genomes to species using k-means clustering on the genes of each genome.
    /// </summary>
    /// <remarks>
    /// This is the speciation scheme used in SharpNEAT 2.x.
    /// 
    /// This is a multi-threaded equivalent of GeneticKMeansSpeciationStrategy, i.e. when calling the speciation methods 
    /// SpeciateAll() and SpeciateAdd(), this class will distribute workload to multiple threads to allow utilisation
    /// of multiple CPU cores if available.
    /// 
    /// Multi-threading is achieved using the .NET framework's Parallel classes, and thus by default will adjust to utilise
    /// however many CPU cores are available.
    /// </remarks>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public class GeneticKMeansSpeciationStrategy<T> : ISpeciationStrategy<NeatGenome<T>,T>
        where T : struct
    {
        #region Instance Fields

        readonly IDistanceMetric<T> _distanceMetric;
        readonly int _maxKMeansIters;
        readonly ParallelOptions _parallelOptions;
        readonly GeneticKMeansSpeciationInit<T> _kmeansInit;

        #endregion
        
        #region Constructors
        
        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="distanceMetric">Distance metric.</param>
        /// <param name="maxKMeansIters">Maximum number of k-means iterations.</param>
        public GeneticKMeansSpeciationStrategy(
            IDistanceMetric<T> distanceMetric,
            int maxKMeansIters)
            : this(distanceMetric, maxKMeansIters, new ParallelOptions())
        {}

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="distanceMetric">Distance metric.</param>
        /// <param name="maxKMeansIters">Maximum number of k-means iterations.</param>
        /// <param name="parallelOptions">Parallel execution options.</param>
        public GeneticKMeansSpeciationStrategy(
            IDistanceMetric<T> distanceMetric,
            int maxKMeansIters,
            ParallelOptions parallelOptions)
        {
            _distanceMetric = distanceMetric;
            _maxKMeansIters = maxKMeansIters;
            _parallelOptions = parallelOptions;
            _kmeansInit = new GeneticKMeansSpeciationInit<T>(distanceMetric, parallelOptions);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise a new set of species based on the provided population of genomes and the 
        /// speciation method in use.
        /// </summary>
        /// <param name="genomeList">The genomes to speciate.</param>
        /// <param name="speciesCount">The number of required species.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>A new array of species.</returns>
        public Species<T>[] SpeciateAll(IList<NeatGenome<T>> genomeList, int speciesCount, IRandomSource rng)
        {
            if(genomeList.Count < speciesCount) {
                throw new ArgumentException("The number of genomes is less than speciesCount.");
            }

            // Initialise using k-means++ initialisation method.
            var speciesArr = _kmeansInit.InitialiseSpecies(genomeList, speciesCount, rng);

            // Run the k-means algorithm.
            RunKMeans(speciesArr);

            // Return the initialised species array.
            return speciesArr;
        }

        /// <summary>
        /// Merge new genomes into an existing set of species.
        /// </summary>
        /// <param name="genomeList">A list of genomes that have not yet been assigned a species.</param>
        /// <param name="speciesArr">An array of pre-existing species</param>
        /// <param name="rng">Random source.</param>
        public void SpeciateAdd(IList<NeatGenome<T>> genomeList, Species<T>[] speciesArr, IRandomSource rng)
        {
            // Create a temporary working array of species modification bits.
            var updateBits = new bool[speciesArr.Length];

            // Allocate the new genomes to the species centroid they are nearest too.
            Parallel.ForEach(genomeList, _parallelOptions, (genome) =>
            {
                var nearestSpeciesIdx = SpeciationUtils.GetNearestSpecies(_distanceMetric, genome, speciesArr);
                var nearestSpecies = speciesArr[nearestSpeciesIdx];

                lock(nearestSpecies.GenomeList) {
                    nearestSpecies.GenomeList.Add(genome);
                }

                // Set the modification bit for the species.
                updateBits[nearestSpeciesIdx] = true;
            });

            // Recalc the species centroids for species that have been modified.
            RecalcCentroids_GenomeList(speciesArr, updateBits);

            // Run the k-means algorithm.
            RunKMeans(speciesArr);
        }

        #endregion

        #region Private Methods [KMeans Algorithm]

        private void RunKMeans(Species<T>[] speciesArr)
        {
            // Initialise.
            KMeansInit(speciesArr);

            // Create a temporary working array of species modification bits.
            var updateBits = new bool[speciesArr.Length];

            // The k-means iterations.
            for(int iter=0; iter < _maxKMeansIters; iter++)
            {
                int reallocCount = KMeansIteration(speciesArr, updateBits);
                if(0 == reallocCount) 
                {   
                    // The last k-means iteration made no re-allocations, therefore the k-means clusters are stable.
                    break;
                }
            }

            // Complete.
            KMeansComplete(speciesArr);
        }

        private int KMeansIteration(Species<T>[] speciesArr, bool[] updateBits)
        {
            int reallocCount = 0;
            Array.Clear(updateBits, 0, updateBits.Length);

            // Loop species.
            // Note. The nested parallel loop here is intentional and should give good thread concurrency in the general case.
            // For more info see: "Is it ok to use nested Parallel.For loops?" 
            // https://blogs.msdn.microsoft.com/pfxteam/2012/03/14/is-it-ok-to-use-nested-parallel-for-loops/
            //
            Parallel.For(0, speciesArr.Length, _parallelOptions, (speciesIdx) => 
            {
                var species = speciesArr[speciesIdx];

                // Loop genomes in the current species.
                Parallel.ForEach(species.GenomeById.Values, _parallelOptions, (genome) =>
                {
                    // Determine the species centroid the genome is nearest to.
                    var nearestSpeciesIdx = SpeciationUtils.GetNearestSpecies(_distanceMetric, genome, speciesArr);

                    // If the nearest species is not the species the genome is currently in then move the genome.
                    if(nearestSpeciesIdx != speciesIdx)
                    {
                        // Move genome.
                        // Note. We can't modify species.GenomeById while we are enumerating through it, therefore we record the IDs
                        // of the genomes to be removed and remove them once we leave the enumeration loop.
                        lock(species.PendingRemovesList) {
                            species.PendingRemovesList.Add(genome.Id);
                        }

                        var nearestSpecies = speciesArr[nearestSpeciesIdx];
                        lock(nearestSpecies.PendingAddsList) {
                            nearestSpecies.PendingAddsList.Add(genome);
                        }

                        // Set the modification bits for the two species.
                        updateBits[speciesIdx] = true;
                        updateBits[nearestSpeciesIdx] = true;

                        // Track the number of re-allocations.
                        Interlocked.Increment(ref reallocCount);
                    }
                });
            });

            // Complete moving of genomes to their new species.
            Parallel.ForEach(speciesArr, _parallelOptions, (species) => species.CompletePendingMoves());

            // Recalc the species centroids for species that have been modified.
            RecalcCentroids_GenomeById(speciesArr, updateBits);

            return reallocCount;
        }

        #endregion

        #region Private Methods [KMeans Helper Methods]

        private void KMeansInit(Species<T>[] speciesArr)
        {
            // Transfer all genomes from GenomeList to GenomeById.
            // Notes. moving genomes between species is more efficient when using dictionaries; 
            // removal from a list can have O(N) complexity because removing an item from 
            // a list requires shuffling up of items to fill the gap.
            Parallel.ForEach(speciesArr, _parallelOptions, species => species.LoadWorkingDictionary());
        }

        private void KMeansComplete(Species<T>[] speciesArr)
        {
            // Check for empty species (this can happen with k-means), and if there are any then 
            // move genomes into those empty species.
            var emptySpeciesArr = speciesArr.Where(x => 0 == x.GenomeById.Count).ToArray();
            if(emptySpeciesArr.Length != 0) {
                SpeciationUtilsParallel.PopulateEmptySpecies(_distanceMetric, emptySpeciesArr, speciesArr);
            }

            // Transfer all genomes from GenomeById to GenomeList.
            Parallel.ForEach(speciesArr, _parallelOptions, species => species.FlushWorkingDictionary());
        }

        private void RecalcCentroids_GenomeById(Species<T>[] speciesArr, bool[] updateBits)
        {
            Parallel.ForEach(Enumerable.Range(0, speciesArr.Length).Where(i => updateBits[i]), _parallelOptions, (i) =>
            {
                var species = speciesArr[i];
                species.Centroid = _distanceMetric.CalculateCentroid(species.GenomeById.Values.Select(x => x.ConnectionGenes)); 
            });
        }

        private void RecalcCentroids_GenomeList(Species<T>[] speciesArr, bool[] updateBits)
        {
            Parallel.ForEach(Enumerable.Range(0, speciesArr.Length).Where(i => updateBits[i]), _parallelOptions, (i) =>
            {
                var species = speciesArr[i];
                species.Centroid = _distanceMetric.CalculateCentroid(species.GenomeList.Select(x => x.ConnectionGenes)); 
            });
        }

        #endregion
    }
}
