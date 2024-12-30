// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using Redzen.Numerics.Distributions;

namespace SharpNeat.Neat.EvolutionAlgorithm;

// TODO: Unit tests.

/// <summary>
/// Static utility methods for creating instances of <see cref="DiscreteDistribution{Double}"/> that describe genome and species selection probabilities.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
internal static class CreateSelectionDistributionUtils<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <summary>
    /// Create instances of <see cref="DiscreteDistribution{Double}"/> for sampling species, and for genomes within each given species.
    /// </summary>
    /// <param name="speciesArr">Species array.</param>
    /// <param name="speciesDist">Returns a new instance of <see cref="DiscreteDistribution{Double}"/> for sampling from the species array.</param>
    /// <param name="genomeDistArr">Returns an array of <see cref="DiscreteDistribution{Double}"/>, for sampling from genomes within each species.</param>
    /// <param name="nonEmptySpeciesCount">Returns the number of species that contain at least one genome.</param>
    public static void CreateSelectionDistributions(
        Species<TScalar>[] speciesArr,
        out DiscreteDistribution<double> speciesDist,
        out DiscreteDistribution<double>?[] genomeDistArr,
        out int nonEmptySpeciesCount)
    {
        // Species selection distribution.
        speciesDist = CreateSpeciesSelectionDistribution(speciesArr, out nonEmptySpeciesCount);

        // Per-species genome selection distributions.
        genomeDistArr = CreateIntraSpeciesGenomeSelectionDistributions(speciesArr);
    }

    #region Private Static Methods

    /// <summary>
    /// Create a <see cref="DiscreteDistribution{Double}"/> that describes the probability of each species being selected, for
    /// cross species reproduction.
    /// </summary>
    /// <param name="speciesArr">Species array.</param>
    /// <param name="nonEmptySpeciesCount">Returns the number of species that contain at least one genome.</param>
    /// <returns>A new instance of <see cref="DiscreteDistribution{Double}"/> for sampling from the species array.</returns>
    private static DiscreteDistribution<double> CreateSpeciesSelectionDistribution(
        Species<TScalar>[] speciesArr,
        out int nonEmptySpeciesCount)
    {
        int speciesCount = speciesArr.Length;
        double[] speciesFitnessArr = new double[speciesCount];
        nonEmptySpeciesCount = 0;

        for(int i=0; i < speciesCount; i++)
        {
            int selectionSizeInt = speciesArr[i].Stats.SelectionSizeInt;
            speciesFitnessArr[i] = selectionSizeInt;
            if(selectionSizeInt != 0)
                nonEmptySpeciesCount++;
        }

        // Note. Here we pass an array of SelectionSizeInt to the constructor of DiscreteDistribution.
        // DiscreteDistribution will normalise these values such that they sum o 1.0, thus, the probability
        // a given species will be selected is proportional to its SelectionSizeInt value.
        return new DiscreteDistribution<double>(speciesFitnessArr);
    }

    private static DiscreteDistribution<double>?[] CreateIntraSpeciesGenomeSelectionDistributions(
        Species<TScalar>[] speciesArr)
    {
        int speciesCount = speciesArr.Length;
        DiscreteDistribution<double>?[] distArr = new DiscreteDistribution<double>[speciesCount];

        // For each species build a DiscreteDistribution for genome selection within
        // that species. I.e. fitter genomes have higher probability of selection.
        for(int i = 0; i < speciesCount; i++)
        {
            Species<TScalar> species = speciesArr[i];

            distArr[i] = species.Stats.SelectionSizeInt switch
            {
                0 => null,
                1 => DiscreteDistribution<double>.SingleOutcome,
                _ => CreateIntraSpeciesGenomeSelectionDistribution(species)
            };
        }

        return distArr;
    }

    private static DiscreteDistribution<double> CreateIntraSpeciesGenomeSelectionDistribution(
        Species<TScalar> species)
    {
        var probArr = new double[species.Stats.SelectionSizeInt];
        var genomeList = species.GenomeList;

        for(int i=0; i < probArr.Length; i++)
            probArr[i] = genomeList[i].FitnessInfo.PrimaryFitness;

        return new DiscreteDistribution<double>(probArr);
    }

    #endregion
}
