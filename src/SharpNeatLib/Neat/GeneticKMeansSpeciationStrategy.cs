using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Redzen.Numerics;
using Redzen.Random;
using SharpNeat.Neat.DistanceMetrics;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat
{
    /// <summary>
    /// A speciation strategy that assigns genomes to species using k-means clustering on the genes of each genome.
    /// </summary>
    /// <remarks>
    /// This is the speciation scheme used in SharpNEAT 2.x.
    /// </remarks>
    /// <typeparam name="T">Connection weight and input/output numeric type (double or float).</typeparam>
    public class GeneticKMeansSpeciationStrategy<T> : ISpeciationStrategy<NeatGenome<T>, T>
        where T : struct
    {
        #region Instance Fields

        readonly IDistanceMetric<T> _distanceMetric;
        readonly int _maxKMeansIters;
        readonly IRandomSource _rng = RandomSourceFactory.Create();

        #endregion
        
        #region Constructor
        
        public GeneticKMeansSpeciationStrategy(
            IDistanceMetric<T> distanceMetric,
            int maxKMeansIters)
        {
            _distanceMetric = distanceMetric;
            _maxKMeansIters = maxKMeansIters;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Initialise a new set of species based on the provided population of genomes and the 
        /// speciation method in use.
        /// </summary>
        /// <param name="genomeList">The genomes to speciate.</param>
        /// <param name="speciesCount">The number of required species.</param>
        /// <returns>A new array of species.</returns>
        public Species<T>[] SpeciateAll(IList<NeatGenome<T>> genomeList, int speciesCount)
        {
            if(genomeList.Count < speciesCount) {
                throw new ArgumentException("The number of genomes is less than speciesCount.");
            }

            // Initialise using k-means++ initialisation method.
            var speciesArr = InitialiseKMeansPlusPlus(genomeList, speciesCount);

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
        public void SpeciateAdd(IList<NeatGenome<T>> genomeList, Species<T>[] speciesArr)
        {
            // Allocate the new genomes to the species centroid they are nearest too.
            foreach(var genome in genomeList)
            {
                var nearestSpecies = GetNearestSpecies(genome, speciesArr, out int nearestSpeciesIdx);
                nearestSpecies.GenomeList.Add(genome);
            }

            // Run the k-means algorithm.
            RunKMeans(speciesArr);
        }

        #endregion

        #region Private Methods [Initialisation]

        private Species<T>[] InitialiseKMeansPlusPlus(IList<NeatGenome<T>> genomeList, int speciesCount)
        {
            // Create an array of seed genomes, i.e. each of these genomes will become the initial 
            // seed/centroid of one species.
            var seedGenomeList = new List<NeatGenome<T>>(speciesCount);

            // Create a list of genomes to select and remove seed genomes from.
            var remainingGenomes = new List<NeatGenome<T>>(genomeList);

            // Select first genome at random.
            seedGenomeList.Add(GetAndRemove(remainingGenomes, _rng.Next(remainingGenomes.Count)));

            // Select all other seed genomes using k-means++ method.
            for(int i=1; i < speciesCount; i++)
            {
                var seedGenome = GetSeedGenome(seedGenomeList, remainingGenomes);
                seedGenomeList.Add(seedGenome);
            }

            // Create an array of species initialised with the chosen seed genomes.

            // Each species is created with an initial capacity that will reduce the need for memory
            // reallocation but that isn't too wasteful of memory.
            int initialCapacity = (genomeList.Count * 2) / speciesCount;

            var speciesArr = new Species<T>[speciesCount];
            for(int i=0; i < speciesCount; i++) 
            {
                var seedGenome = seedGenomeList[i];
                speciesArr[i] = new Species<T>(i, seedGenome.ConnectionGenes, initialCapacity);
                speciesArr[i].GenomeList.Add(seedGenome);
            }

            // Allocate all other genomes to the species centroid they are nearest too.
            foreach(var genome in remainingGenomes)
            {
                var nearestSpecies = GetNearestSpecies(genome, speciesArr, out int nearestSpeciesIdx);
                nearestSpecies.GenomeList.Add(genome);
            }

            return speciesArr;
        }

        private NeatGenome<T> GetSeedGenome(List<NeatGenome<T>> seedGenomeList, List<NeatGenome<T>> remainingGenomes)
        {
            // Create an array of relative selection probabilities for the remaining genomes.
            int remainCount = remainingGenomes.Count;
            double[] pSelectionArr = new double[remainingGenomes.Count];

            for(int i=0; i < remainCount; i++)
            {
                // Note. k-means++ assigns a probability that is the squared distance to the nearest existing centroid.
                double distance = GetDistanceFromNearestSeed(seedGenomeList, remainingGenomes[i]); 
                pSelectionArr[i] = distance * distance; 
            }

            // Select a remaining genome at random based on pSelectionArr; remove it from remainingGenomes and return it.
            int selectIdx = new DiscreteDistribution(pSelectionArr).Sample();
            return GetAndRemove(remainingGenomes, selectIdx);
        }

        private double GetDistanceFromNearestSeed(List<NeatGenome<T>> seedGenomeList, NeatGenome<T> genome)
        {
            double minDistance = _distanceMetric.GetDistance(seedGenomeList[0].ConnectionGenes, genome.ConnectionGenes);

            for(int i=1; i < seedGenomeList.Count; i++) 
            {
                double distance = _distanceMetric.GetDistance(seedGenomeList[i].ConnectionGenes, genome.ConnectionGenes);
                distance = Math.Min(minDistance, distance);
            }

            return minDistance;
        }

        #endregion

        #region Private Methods [KMeans Algorithm]

        private void RunKMeans(Species<T>[] speciesArr)
        {
            KMeansInit(speciesArr);

            var updateBits = new BitArray(speciesArr.Length);

            for(int iter=0; iter < _maxKMeansIters; iter++)
            {
                int reallocCount = KMeansIteration(speciesArr, updateBits);
                if(0 == reallocCount) 
                {   
                    // The last k-means iteration made no re-allocations, therefore the k-means clusters are stable.
                    break;
                }
            }

            KMeansComplete(speciesArr);
        }

        private int KMeansIteration(Species<T>[] speciesArr, BitArray updateBits)
        {
            int reallocCount = 0;
            updateBits.SetAll(false);

            // Loop species.
            for(int speciesIdx=0; speciesIdx < speciesArr.Length; speciesIdx++)
            {
                var species = speciesArr[speciesIdx];

                // Loop genomes in the current species.
                foreach(var genome in species.GenomeList)
                {
                    // Determine the species centroid the genome is nearest to.
                    var nearestSpecies = GetNearestSpecies(genome, speciesArr, out int nearestSpeciesIdx);

                    // If the nearest species is not the species the genome is currently in, then move the genome.
                    if(nearestSpecies != species)
                    {
                        // Move genome.
                        species.GenomeById.Remove(genome.Id);
                        nearestSpecies.GenomeById.Add(genome.Id, genome);

                        // Set the modification bit for the two species.
                        updateBits[speciesIdx] = true;
                        updateBits[nearestSpeciesIdx] = true;
                    }
                }
            }

            // Recalc the species centroids, but only for those species that have been modified.
            RecalcCentroids(speciesArr, updateBits);

            return reallocCount;
        }

        #endregion

        #region Private Methods [KMeans Helper Methods]

        private void KMeansInit(Species<T>[] speciesArr)
        {
            // Transfer all genomes from GenomeList to GenomeById.
            // Notes. moving genomes between species is more efficient when using dictionaries, 
            // because removal from a list can have O(N) complexity because removing an item from 
            // a list requires shuffling up of items to fill the gap.
            foreach(var species in speciesArr) {
                species.LoadWorkingDictionary();
            }
        }

        private void KMeansComplete(Species<T>[] speciesArr)
        {
            foreach(var species in speciesArr) {
                species.FlushWorkingDictionary();
            }
        }

        private void RecalcCentroids(Species<T>[] speciesArr, BitArray updateBits)
        {
            for(int i=0; i < speciesArr.Length; i++)
            {
                if(updateBits[i])
                {
                    var species = speciesArr[i];
                    species.Centroid = _distanceMetric.CalculateCentroid(species.GenomeById.Values.Select(x => x.ConnectionGenes));
                }
            }
        }

        private Species<T> GetNearestSpecies(NeatGenome<T> genome, Species<T>[] speciesArr, out int nearestSpeciesIdx)
        {
            var nearestSpecies = speciesArr[0];
            nearestSpeciesIdx = 0;
            double nearestDistance = _distanceMetric.GetDistance(genome.ConnectionGenes, speciesArr[0].Centroid);

            for(int i=1; i < speciesArr.Length; i++)
            {
                double distance = _distanceMetric.GetDistance(genome.ConnectionGenes, speciesArr[i].Centroid);
                if(distance < nearestDistance)
                {
                    nearestSpecies = speciesArr[i];
                    nearestSpeciesIdx = i;
                    nearestDistance = distance;
                }
            }
            return nearestSpecies;
        }

        #endregion

        #region Private Static Methods

        private static U GetAndRemove<U>(List<U> list, int idx)
        {
            U tmp = list[idx];
            list.RemoveAt(idx);
            return tmp;
        }

        #endregion
    }
}
