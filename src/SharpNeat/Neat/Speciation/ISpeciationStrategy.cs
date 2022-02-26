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
using Redzen.Random;

namespace SharpNeat.Neat.Speciation;

/// <summary>
/// Represents a NEAT speciation strategy.
/// </summary>
/// <typeparam name="TGenome">Genome type.</typeparam>
/// <typeparam name="TWeight">Connection weight data type.</typeparam>
public interface ISpeciationStrategy<TGenome,TWeight>
    where TWeight : struct
{
    /// <summary>
    /// Initialise a new set of species based on the provided population of genomes and the
    /// speciation method in use.
    /// </summary>
    /// <param name="genomeList">The genomes to speciate.</param>
    /// <param name="speciesCount">The number of required species.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>A new array of species.</returns>
    Species<TWeight>[] SpeciateAll(IList<TGenome> genomeList, int speciesCount, IRandomSource rng);

    /// <summary>
    /// Merge new genomes into an existing set of species.
    /// </summary>
    /// <param name="genomeList">A list of genomes that have not yet been assigned a species.</param>
    /// <param name="speciesArr">An array of pre-existing species.</param>
    /// <param name="rng">Random source.</param>
    void SpeciateAdd(IList<TGenome> genomeList, Species<TWeight>[] speciesArr, IRandomSource rng);
}
