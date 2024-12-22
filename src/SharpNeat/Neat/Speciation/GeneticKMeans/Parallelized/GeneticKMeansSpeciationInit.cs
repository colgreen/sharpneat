// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Linq;
using Redzen.Numerics.Distributions;
using SharpNeat.Neat.DistanceMetrics;
using static SharpNeat.Neat.Speciation.SpeciationUtils;

namespace SharpNeat.Neat.Speciation.GeneticKMeans.Parallelized;

/// <summary>
/// GeneticKMeansSpeciationStrategy initialisation.
///
/// This class handles initialising the k-means clusters (species), i.e. it takes a population of genomes
/// and forms an initial set of k-means clusters with which to begin running the k-means iterations upon.
/// upon.
///
/// Note. this implementation applies a modified version of the k-means++ initialisation method.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
internal sealed class GeneticKMeansSpeciationInit<TScalar>
    where TScalar : unmanaged
{
    readonly IDistanceMetric<TScalar> _distanceMetric;
    readonly ParallelOptions _parallelOptions;

    /// <summary>
    /// Construct with the given distance metric.
    /// </summary>
    /// <param name="distanceMetric">Distance metric.</param>
    /// <param name="parallelOptions">Parallel options.</param>
    public GeneticKMeansSpeciationInit(
        IDistanceMetric<TScalar> distanceMetric,
        ParallelOptions parallelOptions)
    {
        _distanceMetric = distanceMetric ?? throw new ArgumentNullException(nameof(distanceMetric));
        _parallelOptions = parallelOptions;
    }

    #region Public Methods

    /// <summary>
    /// Group the provided genomes into new species.
    /// </summary>
    /// <param name="genomeList">The genomes to partition into groups/species.</param>
    /// <param name="speciesCount">The required number of species.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>A new array of <see cref="Species{T}"/>, with each species containing a subset of the genomes from <paramref name="genomeList"/>.</returns>
    public Species<TScalar>[] InitialiseSpecies(
        IList<NeatGenome<TScalar>> genomeList,
        int speciesCount,
        IRandomSource rng)
    {
        // Create an array of seed genomes, i.e. each of these genomes will become the initial
        // seed/centroid of one species.
        var seedGenomeList = new List<NeatGenome<TScalar>>(speciesCount);

        // Create a list of genomes to select and remove seed genomes from.
        var remainingGenomes = new List<NeatGenome<TScalar>>(genomeList);

        // Select first genome at random.
        seedGenomeList.Add(GetAndRemove(remainingGenomes, rng.Next(remainingGenomes.Count)));

        // Select all other seed genomes using k-means++ method.
        for(int i=1; i < speciesCount; i++)
        {
            var seedGenome = GetSeedGenome(seedGenomeList, remainingGenomes, rng);
            seedGenomeList.Add(seedGenome);
        }

        // Create an array of species initialised with the chosen seed genomes.

        // Each species is created with an initial capacity that will reduce the need for memory
        // reallocation but that isn't too wasteful of memory.
        int initialCapacity = (genomeList.Count * 2) / speciesCount;

        var speciesArr = new Species<TScalar>[speciesCount];
        for(int i=0; i < speciesCount; i++)
        {
            var seedGenome = seedGenomeList[i];
            speciesArr[i] = new Species<TScalar>(i, seedGenome.ConnectionGenes, initialCapacity);
            speciesArr[i].GenomeList.Add(seedGenome);
        }

        // Allocate all other genomes to the species centroid they are nearest too.
        Parallel.ForEach(remainingGenomes, _parallelOptions, genome =>
        {
            var nearestSpeciesIdx = GetNearestSpecies(_distanceMetric, genome, speciesArr);
            var nearestSpecies = speciesArr[nearestSpeciesIdx];

            lock(nearestSpecies.GenomeList)
            {
                nearestSpecies.GenomeList.Add(genome);
            }
        });

        // Recalc species centroids.
        Parallel.ForEach(
            speciesArr,
            _parallelOptions,
            () => new List<ConnectionGenes<TScalar>>(),
            (species, loopState, connGenesList) =>
            {
                ExtractConnectionGenes(connGenesList, species.GenomeList);
                species.Centroid = _distanceMetric.CalculateCentroid(connGenesList);
                return connGenesList;
            },
            (connGenesList) => connGenesList.Clear());

        return speciesArr;
    }

    #endregion

    #region Private Methods

    private NeatGenome<TScalar> GetSeedGenome(
        List<NeatGenome<TScalar>> seedGenomeList,
        List<NeatGenome<TScalar>> remainingGenomes,
        IRandomSource rng)
    {
        // Select from a random subset of remainingGenomes rather than the full set, otherwise
        // k-means will have something like O(n^2) scalability
        int subsetCount;

        // For 10 or fewer genomes just select all of them.
        if(remainingGenomes.Count <= 10)
        {
            subsetCount = remainingGenomes.Count;
        }
        else
        {   // For more than ten remainingGenomes we choose a subset size proportional to log(count).
            subsetCount = (int)(Math.Log10(remainingGenomes.Count) * 10.0);
        }

        // Get the indexes of a random subset of remainingGenomes.
        int[] genomeIdxArr = EnumerableUtils.RangeRandomOrder(0, remainingGenomes.Count, rng).Take(subsetCount).ToArray();

        // Create an array of relative selection probabilities for the candidate genomes.
        double[] pArr = new double[subsetCount];

        Parallel.For(0, subsetCount, (i) =>
        {
            // Note. k-means++ assigns a probability that is the squared distance to the nearest existing centroid.
            double distance = GetDistanceFromNearestSeed(seedGenomeList, remainingGenomes[genomeIdxArr[i]]);
            pArr[i] = distance * distance;
        });

        // Select a remaining genome at random based on pArr; remove it from remainingGenomes and return it.
        int selectIdx = new DiscreteDistribution<double>(pArr).Sample(rng);
        return GetAndRemove(remainingGenomes, genomeIdxArr[selectIdx]);
    }

    private double GetDistanceFromNearestSeed(List<NeatGenome<TScalar>> seedGenomeList, NeatGenome<TScalar> genome)
    {
        double minDistance = _distanceMetric.CalcDistance(seedGenomeList[0].ConnectionGenes, genome.ConnectionGenes);

        for(int i=1; i < seedGenomeList.Count; i++)
        {
            double distance = _distanceMetric.CalcDistance(seedGenomeList[i].ConnectionGenes, genome.ConnectionGenes);
            minDistance = Math.Min(minDistance, distance);
        }

        return minDistance;
    }

    private static U GetAndRemove<U>(List<U> list, int idx)
    {
        U tmp = list[idx];
        list.RemoveAt(idx);
        return tmp;
    }

    #endregion
}
