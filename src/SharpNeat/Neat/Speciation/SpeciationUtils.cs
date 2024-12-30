﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Neat.DistanceMetrics;

namespace SharpNeat.Neat.Speciation;

/// <summary>
/// Static utility methods related to speciation.
/// </summary>
public static class SpeciationUtils
{
    #region Public Static Methods

    /// <summary>
    /// Get the index of the species with a centroid that is nearest to the provided genome.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="distanceMetric">Distance metric.</param>
    /// <param name="genome">The genome.</param>
    /// <param name="speciesArr">An array of species to compare the genome with.</param>
    /// <returns>The index of the species that is nearest to <paramref name="genome"/>.</returns>
    public static int GetNearestSpecies<TScalar>(
        IDistanceMetric<TScalar> distanceMetric,
        NeatGenome<TScalar> genome,
        Species<TScalar>[] speciesArr)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // TODO: Select random species if there are multiple species that are equally nearest.
        int nearestSpeciesIdx = 0;
        double nearestDistance = distanceMetric.CalcDistance(genome.ConnectionGenes, speciesArr[0].Centroid);

        for(int i=1; i < speciesArr.Length; i++)
        {
            double distance = distanceMetric.CalcDistance(genome.ConnectionGenes, speciesArr[i].Centroid);
            if(distance < nearestDistance)
            {
                nearestSpeciesIdx = i;
                nearestDistance = distance;
            }
        }
        return nearestSpeciesIdx;
    }

    /// <summary>
    /// Populate empty species with a single genome.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="distanceMetric">Distance metric.</param>
    /// <param name="emptySpeciesArr">An array of empty species that are to be populated.</param>
    /// <param name="speciesArr">An array of all species.</param>
    public static void PopulateEmptySpecies<TScalar>(
        IDistanceMetric<TScalar> distanceMetric,
        Species<TScalar>[] emptySpeciesArr,
        Species<TScalar>[] speciesArr)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // TODO: Select the required genomes all together, rather than one at a time per empty species.

        // Create a temporary, reusable, working list.
        var tmpPointList = new List<ConnectionGenes<TScalar>>();

        foreach(Species<TScalar> emptySpecies in emptySpeciesArr)
        {
            // Get and remove a genome from a species with many genomes.
            var genome = GetGenomeForEmptySpecies(distanceMetric, speciesArr, tmpPointList);

            // Add the genome to the empty species.
            emptySpecies.GenomeById.Add(genome.Id, genome);

            // Update the centroid. There's only one genome so it is the centroid.
            emptySpecies.Centroid = genome.ConnectionGenes;
        }

        tmpPointList.Clear();
    }

    /// <summary>
    /// Populate a list of <see cref="ConnectionGenes{T}"/> with the connection genes from a dictionary of
    /// <see cref="NeatGenome{T}"/>.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="targetList">The list to populate.</param>
    /// <param name="genomeById">The dictionary of genomes.</param>
    public static void ExtractConnectionGenes<TScalar>(
        List<ConnectionGenes<TScalar>> targetList,
        Dictionary<int,NeatGenome<TScalar>> genomeById)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        targetList.Clear();

        // Increase the target list's capacity, if necessary.
        int count = genomeById.Count;
        if(count > targetList.Capacity)
        {
            int newCapacity = MathUtils.CeilingToPowerOfTwo(count);
            targetList.Capacity = newCapacity;
        }

        targetList.AddRange(genomeById.Values.Select(x => x.ConnectionGenes));
    }

    /// <summary>
    /// Populate a list of <see cref="ConnectionGenes{T}"/> with the connection genes from a list of
    /// <see cref="NeatGenome{T}"/>.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="targetList">The list to populate.</param>
    /// <param name="genomeList">The list of genomes.</param>
    public static void ExtractConnectionGenes<TScalar>(
        List<ConnectionGenes<TScalar>> targetList,
        List<NeatGenome<TScalar>> genomeList)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        targetList.Clear();

        // Increase the target list's capacity, if necessary.
        int count = genomeList.Count;
        if(count > targetList.Capacity)
        {
            int newCapacity = MathUtils.CeilingToPowerOfTwo(count);
            targetList.Capacity = newCapacity;
        }

        targetList.AddRange(genomeList.Select(x => x.ConnectionGenes));
    }

    #endregion

    #region Private Static Methods

    private static NeatGenome<TScalar> GetGenomeForEmptySpecies<TScalar>(
        IDistanceMetric<TScalar> distanceMetric,
        Species<TScalar>[] speciesArr,
        List<ConnectionGenes<TScalar>> tmpPointList)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // TODO: Select donor species stochastically from a pool of the largest species.

        // Get the species with the highest number of genomes.
        Species<TScalar> species = speciesArr.Aggregate((x, y) => x.GenomeById.Count > y.GenomeById.Count ? x : y);

        // Get the genome furthest from the species centroid.
        double maxDistance = -1.0;
        NeatGenome<TScalar>? chosenGenome = null;

        foreach(var genome in species.GenomeById.Values)
        {
            double distance = distanceMetric.CalcDistance(species.Centroid, genome.ConnectionGenes);
            if(distance > maxDistance)
            {
                maxDistance = distance;
                chosenGenome = genome;
            }
        }

        // Remove the genome from its current species.
        species.GenomeById.Remove(chosenGenome!.Id);

        // Extract the ConnectionGenes<TWeight> object from each genome in the species' genome list.
        ExtractConnectionGenes(tmpPointList, species.GenomeById);

        // Calc and update species centroid.
        species.Centroid = distanceMetric.CalculateCentroid(tmpPointList);

        // Return the selected genome.
        return chosenGenome;
    }

    #endregion
}
