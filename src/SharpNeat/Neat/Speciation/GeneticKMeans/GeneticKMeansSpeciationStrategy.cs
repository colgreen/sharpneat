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
using System;
using System.Collections.Generic;
using System.Linq;
using Redzen.Random;
using SharpNeat.Neat.DistanceMetrics;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Speciation.GeneticKMeans
{
    /// <summary>
    /// A speciation strategy that assigns genomes to species using k-means clustering on the genes of each genome.
    /// </summary>
    /// <remarks>
    /// This is the speciation scheme used in SharpNEAT 2.x.
    /// </remarks>
    /// <typeparam name="T">Neural net signal and weight data type.</typeparam>
    public sealed class GeneticKMeansSpeciationStrategy<T> : ISpeciationStrategy<NeatGenome<T>, T>
        where T : struct
    {
        #region Instance Fields

        readonly IDistanceMetric<T> _distanceMetric;
        readonly int _maxKMeansIters;
        readonly GeneticKMeansSpeciationInit<T> _kmeansInit;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="distanceMetric">Distance metric.</param>
        /// <param name="maxKMeansIters">Maximum number of k-means iterations.</param>
        public GeneticKMeansSpeciationStrategy(IDistanceMetric<T> distanceMetric, int maxKMeansIters)
        {
            _distanceMetric = distanceMetric ?? throw new ArgumentNullException(nameof(distanceMetric));
            _maxKMeansIters = maxKMeansIters;
            _kmeansInit = new GeneticKMeansSpeciationInit<T>(distanceMetric);
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
        /// <param name="speciesArr">An array of pre-existing species.</param>
        /// <param name="rng">Random source.</param>
        public void SpeciateAdd(IList<NeatGenome<T>> genomeList, Species<T>[] speciesArr, IRandomSource rng)
        {
            // Create a temporary working array of species modification bits.
            var updateBits = new bool[speciesArr.Length];

            // Allocate the new genomes to the species centroid they are nearest too.
            foreach(var genome in genomeList)
            {
                var nearestSpeciesIdx = SpeciationUtils.GetNearestSpecies(_distanceMetric, genome, speciesArr);
                speciesArr[nearestSpeciesIdx].GenomeList.Add(genome);

                // Set the modification bit for the species.
                updateBits[nearestSpeciesIdx] = true;
            }

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
                if(reallocCount == 0)
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
            for(int speciesIdx=0; speciesIdx < speciesArr.Length; speciesIdx++)
            {
                var species = speciesArr[speciesIdx];

                // Loop genomes in the current species.
                foreach(var genome in species.GenomeById.Values)
                {
                    // Determine the species centroid the genome is nearest to.
                    var nearestSpeciesIdx = SpeciationUtils.GetNearestSpecies(_distanceMetric, genome, speciesArr);

                    // If the nearest species is not the species the genome is currently in then move the genome.
                    if(nearestSpeciesIdx != speciesIdx)
                    {
                        // Move genome.
                        // Note. We can't modify species.GenomeById while we are enumerating through it, therefore we record the IDs
                        // of the genomes to be removed and remove them once we leave the enumeration loop.
                        species.PendingRemovesList.Add(genome.Id);
                        speciesArr[nearestSpeciesIdx].PendingAddsList.Add(genome);

                        // Set the modification bits for the two species.
                        updateBits[speciesIdx] = true;
                        updateBits[nearestSpeciesIdx] = true;

                        // Track the number of re-allocations.
                        reallocCount++;
                    }
                }
            }

            // Complete moving of genomes to their new species.
            foreach(var species in speciesArr) {
                species.CompletePendingMoves();
            }

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
            // removal from a list has O(N) complexity, because removing an item from a list requires
            // shuffling items to fill the gap.
            foreach(var species in speciesArr) {
                species.LoadWorkingDictionary();
            }
        }

        private void KMeansComplete(Species<T>[] speciesArr)
        {
            // Check for empty species (this can happen with k-means), and if there are any then
            // move genomes into those empty species.
            var emptySpeciesArr = speciesArr.Where(x => x.GenomeById.Count == 0).ToArray();
            if(emptySpeciesArr.Length != 0) {
                SpeciationUtils.PopulateEmptySpecies(_distanceMetric, emptySpeciesArr, speciesArr);
            }

            // Transfer all genomes from GenomeById to GenomeList.
            foreach(var species in speciesArr) {
                species.FlushWorkingDictionary();
            }
        }

        private void RecalcCentroids_GenomeById(
            Species<T>[] speciesArr,
            bool[] updateBits)
        {
            // Create a temporary, reusable, working list.
            var tmpConnGenes = new List<ConnectionGenes<T>>();

            for(int i=0; i < speciesArr.Length; i++)
            {
                if(updateBits[i])
                {
                    var species = speciesArr[i];

                    // Extract the ConnectionGenes<T> object from each genome in the GenomeById dictionary.
                    SpeciationUtils.ExtractConnectionGenes(tmpConnGenes, species.GenomeById);

                    // Calculate the centroid for the extracted connection genes.
                    species.Centroid = _distanceMetric.CalculateCentroid(tmpConnGenes);
                }
            }

            tmpConnGenes.Clear();
        }

        private void RecalcCentroids_GenomeList(
            Species<T>[] speciesArr,
            bool[] updateBits)
        {
            // Create a temporary, reusable, working list.
            var tmpConnGenes = new List<ConnectionGenes<T>>();

            for(int i=0; i < speciesArr.Length; i++)
            {
                if(updateBits[i])
                {
                    var species = speciesArr[i];

                    // Extract the ConnectionGenes<T> object from each genome in GenomeList.
                    SpeciationUtils.ExtractConnectionGenes(tmpConnGenes, species.GenomeList);

                    // Calculate the centroid for the extracted connection genes.
                    species.Centroid = _distanceMetric.CalculateCentroid(tmpConnGenes);
                }
            }

            tmpConnGenes.Clear();
        }

        #endregion
    }
}
