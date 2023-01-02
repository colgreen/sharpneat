// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;
using SharpNeat.Neat.DistanceMetrics;

namespace SharpNeat.Neat.Speciation.GeneticKMeans.Parallelized;

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
public sealed class GeneticKMeansSpeciationStrategy<T> : ISpeciationStrategy<NeatGenome<T>,T>
    where T : struct
{
    readonly IDistanceMetric<T> _distanceMetric;
    readonly int _maxKMeansIters;
    readonly ParallelOptions _parallelOptions;
    readonly GeneticKMeansSpeciationInit<T> _kmeansInit;

    #region Constructors

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="distanceMetric">Distance metric.</param>
    /// <param name="maxKMeansIters">Maximum number of k-means iterations.</param>
    /// <param name="degreeOfParallelism">The number of CPU threads to distribute work to.</param>
    public GeneticKMeansSpeciationStrategy(
        IDistanceMetric<T> distanceMetric,
        int maxKMeansIters,
        int degreeOfParallelism)
    {
        _distanceMetric = distanceMetric;
        _maxKMeansIters = maxKMeansIters;

        // Reject degreeOfParallelism values less than 2. -1 should have been resolved to an actual number by the time
        // this constructor is invoked, and 1 is nonsensical for a parallel strategy.
        if(degreeOfParallelism < 2) throw new ArgumentException("Must be 2 or above.", nameof(degreeOfParallelism));

        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = degreeOfParallelism
        };

        _kmeansInit = new GeneticKMeansSpeciationInit<T>(distanceMetric, _parallelOptions);
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public Species<T>[] SpeciateAll(IList<NeatGenome<T>> genomeList, int speciesCount, IRandomSource rng)
    {
        if(genomeList.Count < speciesCount)
            throw new ArgumentException("The number of genomes is less than speciesCount.");

        // Initialise using k-means++ initialisation method.
        var speciesArr = _kmeansInit.InitialiseSpecies(genomeList, speciesCount, rng);

        // Run the k-means algorithm.
        RunKMeans(speciesArr);

        // Return the initialised species array.
        return speciesArr;
    }

    /// <inheritdoc/>
    public void SpeciateAdd(IList<NeatGenome<T>> genomeList, Species<T>[] speciesArr, IRandomSource rng)
    {
        // Create a temporary working array of species modification bits.
        var updateBits = new bool[speciesArr.Length];

        // Allocate the new genomes to the species centroid they are nearest too.
        Parallel.ForEach(genomeList, _parallelOptions, (genome) =>
        {
            var nearestSpeciesIdx = SpeciationUtils.GetNearestSpecies(_distanceMetric, genome, speciesArr);
            var nearestSpecies = speciesArr[nearestSpeciesIdx];

            lock(nearestSpecies.GenomeList)
            {
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
        // Note. The nested parallel loop here is intentional and should give good thread concurrency in the general case.
        // For more info see: "Is it OK to use nested Parallel.For loops?"
        // https://blogs.msdn.microsoft.com/pfxteam/2012/03/14/is-it-ok-to-use-nested-parallel-for-loops/

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
                    lock(species.PendingRemovesList)
                    {
                        species.PendingRemovesList.Add(genome.Id);
                    }

                    var nearestSpecies = speciesArr[nearestSpeciesIdx];
                    lock(nearestSpecies.PendingAddsList)
                    {
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
        // removal from a list has O(N) complexity, because removing an item from a list requires
        // shuffling items to fill the gap.
        Parallel.ForEach(speciesArr, _parallelOptions, species => species.LoadWorkingDictionary());
    }

    private void KMeansComplete(Species<T>[] speciesArr)
    {
        // Check for empty species (this can happen with k-means), and if there are any then
        // move genomes into those empty species.
        var emptySpeciesArr = speciesArr.Where(x => x.GenomeById.Count == 0).ToArray();
        if(emptySpeciesArr.Length != 0)
            SpeciationUtils.PopulateEmptySpecies(_distanceMetric, emptySpeciesArr, speciesArr);

        // Transfer all genomes from GenomeById to GenomeList.
        Parallel.ForEach(speciesArr, _parallelOptions, species => species.FlushWorkingDictionary());
    }

    private void RecalcCentroids_GenomeById(Species<T>[] speciesArr, bool[] updateBits)
    {
        Parallel.ForEach(
            Enumerable.Range(0, speciesArr.Length).Where(i => updateBits[i]),
            _parallelOptions,
            () => new List<ConnectionGenes<T>>(),
            (speciesIdx, loopState, connGenesList) =>
            {
                var species = speciesArr[speciesIdx];
                SpeciationUtils.ExtractConnectionGenes(connGenesList, species.GenomeById);
                species.Centroid = _distanceMetric.CalculateCentroid(connGenesList);
                return connGenesList;
            },
            (connGenesList) => connGenesList.Clear());
    }

    private void RecalcCentroids_GenomeList(Species<T>[] speciesArr, bool[] updateBits)
    {
        Parallel.ForEach(
            Enumerable.Range(0, speciesArr.Length).Where(i => updateBits[i]),
            _parallelOptions,
            () => new List<ConnectionGenes<T>>(),
            (speciesIdx, loopState, connGenesList) =>
            {
                var species = speciesArr[speciesIdx];
                SpeciationUtils.ExtractConnectionGenes(connGenesList, species.GenomeList);
                species.Centroid = _distanceMetric.CalculateCentroid(connGenesList);
                return connGenesList;
            },
            (connGenesList) => connGenesList.Clear());
    }

    #endregion
}
