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
using Redzen.Numerics.Distributions.Double;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Neat.EvolutionAlgorithm;

// TODO: Unit tests.

/// <summary>
/// Static utility methods for creating instances of <see cref="DiscreteDistribution"/> that describe genome and species selection probabilities.
/// </summary>
/// <typeparam name="T">Neural net numeric data type.</typeparam>
internal static class CreateSelectionDistributionUtils<T>
    where T : struct
{
    #region Public Static Methods

    /// <summary>
    /// Create instances of <see cref="DiscreteDistribution"/> for sampling species, and for genomes within each given species.
    /// </summary>
    /// <param name="speciesArr">Species array.</param>
    /// <param name="speciesDist">Returns a new instance of <see cref="DiscreteDistribution"/> for sampling from the species array.</param>
    /// <param name="genomeDistArr">Returns an array of <see cref="DiscreteDistribution"/>, for sampling from genomes within each species.</param>
    /// <param name="nonEmptySpeciesCount">Returns the number of species that contain at least one genome.</param>
    public static void CreateSelectionDistributions(
        Species<T>[] speciesArr,
        out DiscreteDistribution speciesDist,
        out DiscreteDistribution?[] genomeDistArr,
        out int nonEmptySpeciesCount)
    {
        // Species selection distribution.
        speciesDist = CreateSpeciesSelectionDistribution(speciesArr, out nonEmptySpeciesCount);

        // Per-species genome selection distributions.
        genomeDistArr = CreateIntraSpeciesGenomeSelectionDistributions(speciesArr);
    }

    #endregion

    #region Private Static Methods

    /// <summary>
    /// Create a <see cref="DiscreteDistribution"/> that describes the probability of each species being selected, for
    /// cross species reproduction.
    /// </summary>
    /// <param name="speciesArr">Species array.</param>
    /// <param name="nonEmptySpeciesCount">Returns the number of species that contain at least one genome.</param>
    /// <returns>A new instance of <see cref="DiscreteDistribution"/> for sampling from the species array.</returns>
    private static DiscreteDistribution CreateSpeciesSelectionDistribution(
        Species<T>[] speciesArr,
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
        return new DiscreteDistribution(speciesFitnessArr);
    }

    private static DiscreteDistribution?[] CreateIntraSpeciesGenomeSelectionDistributions(
        Species<T>[] speciesArr)
    {
        int speciesCount = speciesArr.Length;
        DiscreteDistribution?[] distArr = new DiscreteDistribution[speciesCount];

        // For each species build a DiscreteDistribution for genome selection within
        // that species. I.e. fitter genomes have higher probability of selection.
        for(int i = 0; i < speciesCount; i++)
        {
            Species<T> species = speciesArr[i];

            distArr[i] = species.Stats.SelectionSizeInt switch
            {
                0 => null,
                1 => DiscreteDistribution.SingleOutcome,
                _ => CreateIntraSpeciesGenomeSelectionDistribution(species)
            };
        }

        return distArr;
    }

    private static DiscreteDistribution CreateIntraSpeciesGenomeSelectionDistribution(
        Species<T> species)
    {
        var probArr = new double[species.Stats.SelectionSizeInt];
        var genomeList = species.GenomeList;

        for(int i=0; i < probArr.Length; i++)
            probArr[i] = genomeList[i].FitnessInfo.PrimaryFitness;

        return new DiscreteDistribution(probArr);
    }

    #endregion
}
