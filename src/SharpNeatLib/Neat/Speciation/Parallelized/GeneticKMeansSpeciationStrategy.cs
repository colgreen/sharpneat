using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Redzen.Linq;
using Redzen.Numerics;
using Redzen.Random;
using SharpNeat.Neat.DistanceMetrics;
using SharpNeat.Neat.Genome;
using static SharpNeat.Neat.Speciation.GeneticKMeansSpeciationStrategyUtils;

namespace SharpNeat.Neat.Speciation.Parallelized
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
    /// <typeparam name="T">Connection weight and input/output numeric type (double or float).</typeparam>
    public class GeneticKMeansSpeciationStrategy<T> : ISpeciationStrategy<NeatGenome<T>,T>
        where T : struct
    {
        #region Instance Fields

        readonly IDistanceMetric<T> _distanceMetric;
        readonly int _maxKMeansIters;
        readonly ParallelOptions _parallelOptions;
        readonly IRandomSource _rng = RandomSourceFactory.Create();

        #endregion
        
        #region Constructors
        
        public GeneticKMeansSpeciationStrategy(
            IDistanceMetric<T> distanceMetric,
            int maxKMeansIters)
        {
            _distanceMetric = distanceMetric;
            _maxKMeansIters = maxKMeansIters;
            _parallelOptions = new ParallelOptions();
        }

        public GeneticKMeansSpeciationStrategy(
            IDistanceMetric<T> distanceMetric,
            int maxKMeansIters,
            ParallelOptions parallelOptions)
        {
            _distanceMetric = distanceMetric;
            _maxKMeansIters = maxKMeansIters;
            _parallelOptions = parallelOptions;
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
            // Create a temporary working array of species modification bits.
            var updateBits = new bool[speciesArr.Length];

            // Allocate the new genomes to the species centroid they are nearest too.
            Parallel.ForEach(genomeList, _parallelOptions, (genome) =>
            {
                var nearestSpeciesIdx = GetNearestSpecies(genome, speciesArr, _distanceMetric);
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
            Parallel.ForEach(remainingGenomes, _parallelOptions, genome => 
            {
                var nearestSpeciesIdx = GetNearestSpecies(genome, speciesArr, _distanceMetric);
                var nearestSpecies = speciesArr[nearestSpeciesIdx];

                lock(nearestSpecies.GenomeList) {
                    nearestSpecies.GenomeList.Add(genome);
                }
            });

            // Recalc species centroids.
            Parallel.ForEach(speciesArr, _parallelOptions, species => {
                species.Centroid = _distanceMetric.CalculateCentroid(species.GenomeList.Select(genome => genome.ConnectionGenes));
            });

            return speciesArr;
        }

        private NeatGenome<T> GetSeedGenome(List<NeatGenome<T>> seedGenomeList, List<NeatGenome<T>> remainingGenomes)
        {
            // Select from a random subset of remainingGenomes rather than the full set, otherwise
            // k-means will have something like O(n^2) scalability
            int subsetCount;
            
            // For 10 or fewer genomes just select all of them.
            if(remainingGenomes.Count <= 10) {
                subsetCount = remainingGenomes.Count;
            }
            else 
            {   // For more than ten remainingGenomes we choose a subset size proportional to log(count).
                subsetCount = (int)(Math.Log10(remainingGenomes.Count) * 10.0);
            }
                        
            // Get the indexes of a random subset of remainingGenomes.
            int[] genomeIdxArr = EnumerableUtils.RangeRandomOrder(0, remainingGenomes.Count, _rng).Take(subsetCount).ToArray();

            // Create an array of relative selection probabilities for the candidate genomes.
            double[] pArr = new double[subsetCount];

            Parallel.For(0, subsetCount, (i) =>
            {
                // Note. k-means++ assigns a probability that is the squared distance to the nearest existing centroid.
                double distance = GetDistanceFromNearestSeed(seedGenomeList, remainingGenomes[genomeIdxArr[i]]); 
                pArr[i] = distance * distance; 
            });

            // Select a remaining genome at random based on pArr; remove it from remainingGenomes and return it.
            int selectIdx = new DiscreteDistribution(_rng, pArr).Sample();
            return GetAndRemove(remainingGenomes, genomeIdxArr[selectIdx]);
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
                Parallel.ForEach(species.GenomeById.Values, (genome) =>
                {
                    // Determine the species centroid the genome is nearest to.
                    var nearestSpeciesIdx = GetNearestSpecies(genome, speciesArr, _distanceMetric);

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
                PopulateEmptySpecies(emptySpeciesArr, speciesArr);
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

        #region Private Methods [Empty Species Handling]

        private void PopulateEmptySpecies(Species<T>[] emptySpeciesArr, Species<T>[] speciesArr)
        {
            foreach(Species<T> emptySpecies in emptySpeciesArr)
            {
                // Get and remove a genome from a species with many genomes.
                var genome = GetGenomeForEmptySpecies(speciesArr);

                // Add the genome to the empty species.
                emptySpecies.GenomeById.Add(genome.Id, genome);

                // Update the centroid. There's only one genome so it is the centroid.
                emptySpecies.Centroid = genome.ConnectionGenes;
            }
        }

        private NeatGenome<T> GetGenomeForEmptySpecies(Species<T>[] speciesArr)
        {
            // Get the species with the highest number of genomes.
            Species<T> species = speciesArr.Aggregate((x, y) => x.GenomeById.Count > y.GenomeById.Count ?  x : y);

            // Get the genome furthest from the species centroid.
            // Note. The use of AsParallel() here over dictionary values is perhaps unusual, but should be safe AFAIK.
            var genome = species.GenomeById.Values.AsParallel().Aggregate((x, y) => _distanceMetric.GetDistance(species.Centroid, x.ConnectionGenes) > _distanceMetric.GetDistance(species.Centroid, y.ConnectionGenes) ? x : y);

            // Remove the genome from its current species.
            species.GenomeById.Remove(genome.Id);

            // Update the species centroid.
            species.Centroid = _distanceMetric.CalculateCentroid(species.GenomeById.Values.Select(x => x.ConnectionGenes));

            // Return the selected genome.
            return genome;
        }

        #endregion
    }
}
