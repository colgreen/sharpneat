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
using System;
using System.Collections.Generic;
using SharpNeat.Evaluation;
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Neat.EvolutionAlgorithm;

/// <summary>
/// For comparing genomes based on their fitness.
/// The comparison result is reversed compared to the standard rules for an IComparer, therefore using this comparer
/// to sort genomes will result in genomes sorted in descending fitness order, i.e. fittest genomes first.
/// </summary>
public sealed class GenomeComparerDescending : IComparer<IGenome>
{
    readonly IComparer<FitnessInfo> _fitnessInfoComparer;

    /// <summary>
    /// Construct with the given <see cref="FitnessInfo"/> comparer.
    /// </summary>
    /// <param name="fitnessInfoComparer">A <see cref="FitnessInfo"/> comparer that can be used to compare the relative fitness of any two genomes.</param>
    public GenomeComparerDescending(IComparer<FitnessInfo> fitnessInfoComparer)
    {
        _fitnessInfoComparer = fitnessInfoComparer ?? throw new ArgumentNullException(nameof(fitnessInfoComparer));
    }

    /// <summary>
    /// Compares two genomes, and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">The first genome to compare.</param>
    /// <param name="y">The second genome to compare.</param>
    /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />.
    /// If genome x's fitness is higher then genome y's fitness then returns a positive integer.
    /// If genome y's fitness is higher then genome x's fitness then returns a negative integer.
    /// If the two genomes have equal fitness then returns zero.
    /// </returns>
    public int Compare(IGenome? x, IGenome? y)
    {
        // Note. The x and y genome argument order is swapped/reversed; this results in a sorting of genomes
        // based on this IComparer<> sorting in descending order, i.e. highest fitness to lowest fitness.
        return _fitnessInfoComparer.Compare(y!.FitnessInfo, x!.FitnessInfo);
    }
}
