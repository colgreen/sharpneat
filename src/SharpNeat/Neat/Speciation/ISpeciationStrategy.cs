﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace SharpNeat.Neat.Speciation;

/// <summary>
/// Represents a NEAT speciation strategy.
/// </summary>
/// <typeparam name="TGenome">Genome type.</typeparam>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public interface ISpeciationStrategy<TGenome,TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <summary>
    /// Initialise a new set of species based on the provided population of genomes and the
    /// speciation method in use.
    /// </summary>
    /// <param name="genomeList">The genomes to speciate.</param>
    /// <param name="speciesCount">The number of required species.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>A new array of species.</returns>
    Species<TScalar>[] SpeciateAll(
        IList<TGenome> genomeList,
        int speciesCount,
        IRandomSource rng);

    /// <summary>
    /// Merge new genomes into an existing set of species.
    /// </summary>
    /// <param name="genomeList">A list of genomes that have not yet been assigned a species.</param>
    /// <param name="speciesArr">An array of pre-existing species.</param>
    /// <param name="rng">Random source.</param>
    void SpeciateAdd(
        IList<TGenome> genomeList,
        Species<TScalar>[] speciesArr,
        IRandomSource rng);
}
