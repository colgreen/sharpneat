/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2022 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
namespace SharpNeat.Evaluation;

/// <summary>
/// Represents an evaluator of lists of genomes.
/// </summary>
/// <typeparam name="TGenome">Genome type.</typeparam>
public interface IGenomeListEvaluator<TGenome>
{
    /// <summary>
    /// Indicates if the evaluation scheme is deterministic, i.e. will always return the same fitness score for a given genome.
    /// </summary>
    /// <remarks>
    /// An evaluation scheme that has some random/stochastic characteristics may give a different fitness score at each invocation
    /// for the same genome, such a scheme is non-deterministic.
    /// </remarks>
    bool IsDeterministic { get; }

    /// <summary>
    /// Gets a fitness comparer.
    /// </summary>
    /// <remarks>
    /// Typically there is a single fitness score whereby a higher score is better, however if there are multiple fitness scores
    /// per genome then we need a more general purpose comparer to determine an ordering on FitnessInfo, i.e. to be able to
    /// determine which is the better FitenssInfo between any two.
    /// </remarks>
    IComparer<FitnessInfo> FitnessComparer { get; }

    /// <summary>
    /// Evaluates a list of genomes, assigning fitness info to each.
    /// </summary>
    /// <param name="genomeList">The list of genomes to evaluate.</param>
    void Evaluate(IList<TGenome> genomeList);

    /// <summary>
    /// Accepts a <see cref="FitnessInfo"/>, which is intended to be from the fittest genome in the population, and returns a boolean
    /// that indicates if the evolution algorithm can stop, i.e. because the fitness is the best that can be achieved (or good enough).
    /// </summary>
    /// <param name="fitnessInfo">The fitness info object to test.</param>
    /// <returns>Returns true if the fitness is good enough to signal the evolution algorithm to stop.</returns>
    bool TestForStopCondition(FitnessInfo fitnessInfo);
}
