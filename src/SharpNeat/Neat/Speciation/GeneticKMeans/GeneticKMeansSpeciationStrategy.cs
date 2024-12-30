// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Neat.DistanceMetrics;

namespace SharpNeat.Neat.Speciation.GeneticKMeans;

/// <summary>
/// A speciation strategy that assigns genomes to species using k-means clustering on the genes of each genome.
/// </summary>
/// <remarks>
/// This is the speciation scheme used in SharpNEAT 2.x.
/// </remarks>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public sealed class GeneticKMeansSpeciationStrategy<TScalar> : ISpeciationStrategy<NeatGenome<TScalar>, TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    readonly IDistanceMetric<TScalar> _distanceMetric;
    readonly int _maxKMeansIters;
    readonly GeneticKMeansSpeciationInit<TScalar> _kmeansInit;

    #region Constructor

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="distanceMetric">Distance metric.</param>
    /// <param name="maxKMeansIters">Maximum number of k-means iterations.</param>
    public GeneticKMeansSpeciationStrategy(IDistanceMetric<TScalar> distanceMetric, int maxKMeansIters)
    {
        _distanceMetric = distanceMetric ?? throw new ArgumentNullException(nameof(distanceMetric));
        _maxKMeansIters = maxKMeansIters;
        _kmeansInit = new GeneticKMeansSpeciationInit<TScalar>(distanceMetric);
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public Species<TScalar>[] SpeciateAll(
        IList<NeatGenome<TScalar>> genomeList,
        int speciesCount,
        IRandomSource rng)
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
    public void SpeciateAdd(
        IList<NeatGenome<TScalar>> genomeList,
        Species<TScalar>[] speciesArr,
        IRandomSource rng)
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

    private void RunKMeans(Species<TScalar>[] speciesArr)
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

    private int KMeansIteration(
        Species<TScalar>[] speciesArr,
        bool[] updateBits)
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
        foreach(var species in speciesArr)
            species.CompletePendingMoves();

        // Recalc the species centroids for species that have been modified.
        RecalcCentroids_GenomeById(speciesArr, updateBits);

        return reallocCount;
    }

    #endregion

    #region Private Methods [KMeans Helper Methods]

    private static void KMeansInit(Species<TScalar>[] speciesArr)
    {
        // Transfer all genomes from GenomeList to GenomeById.
        // Notes. moving genomes between species is more efficient when using dictionaries;
        // removal from a list has O(N) complexity, because removing an item from a list requires
        // shuffling items to fill the gap.
        foreach(var species in speciesArr)
            species.LoadWorkingDictionary();
    }

    private void KMeansComplete(Species<TScalar>[] speciesArr)
    {
        // Check for empty species (this can happen with k-means), and if there are any then
        // move genomes into those empty species.
        var emptySpeciesArr = speciesArr.Where(x => x.GenomeById.Count == 0).ToArray();
        if(emptySpeciesArr.Length != 0)
            SpeciationUtils.PopulateEmptySpecies(_distanceMetric, emptySpeciesArr, speciesArr);

        // Transfer all genomes from GenomeById to GenomeList.
        foreach(var species in speciesArr)
            species.FlushWorkingDictionary();
    }

    private void RecalcCentroids_GenomeById(
        Species<TScalar>[] speciesArr,
        bool[] updateBits)
    {
        // Create a temporary, reusable, working list.
        var tmpConnGenes = new List<ConnectionGenes<TScalar>>();

        for(int i=0; i < speciesArr.Length; i++)
        {
            if(updateBits[i])
            {
                var species = speciesArr[i];

                // Extract the ConnectionGenes<TWeight> object from each genome in the GenomeById dictionary.
                SpeciationUtils.ExtractConnectionGenes(tmpConnGenes, species.GenomeById);

                // Calculate the centroid for the extracted connection genes.
                species.Centroid = _distanceMetric.CalculateCentroid(tmpConnGenes);
            }
        }

        tmpConnGenes.Clear();
    }

    private void RecalcCentroids_GenomeList(
        Species<TScalar>[] speciesArr,
        bool[] updateBits)
    {
        // Create a temporary, reusable, working list.
        var tmpConnGenes = new List<ConnectionGenes<TScalar>>();

        for(int i=0; i < speciesArr.Length; i++)
        {
            if(updateBits[i])
            {
                var species = speciesArr[i];

                // Extract the ConnectionGenes<TWeight> object from each genome in GenomeList.
                SpeciationUtils.ExtractConnectionGenes(tmpConnGenes, species.GenomeList);

                // Calculate the centroid for the extracted connection genes.
                species.Centroid = _distanceMetric.CalculateCentroid(tmpConnGenes);
            }
        }

        tmpConnGenes.Clear();
    }

    #endregion
}
