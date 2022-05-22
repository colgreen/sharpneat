﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.CartPole.DoublePole;

/// <summary>
/// Evaluation scheme for the cart and pole balancing task, with two poles.
/// </summary>
public sealed class CartDoublePoleEvaluationScheme : IBlackBoxEvaluationScheme<double>
{
    #region Properties

    /// <summary>
    /// The number of black box inputs expected/required by the black box evaluation scheme.
    /// </summary>
    /// <remarks>
    /// The 6 inputs of the double pole balancing task, plus one bias input (input zero).
    /// </remarks>
    public int InputCount => 7;

    /// <summary>
    /// The number of black box outputs expected/required by the black box evaluation scheme.
    /// </summary>
    public int OutputCount => 1;

    /// <summary>
    /// Indicates if the evaluation scheme is deterministic, i.e. will always return the same fitness score for a given genome.
    /// </summary>
    /// <remarks>
    /// An evaluation scheme that has some random/stochastic characteristics may give a different fitness score at each invocation
    /// for the same genome, such a scheme is non-deterministic.
    /// </remarks>
    public bool IsDeterministic => true;

    /// <summary>
    /// Gets a fitness comparer for the scheme.
    /// </summary>
    /// <remarks>
    /// Typically there is a single fitness score and a higher score is considered better/fitter. However, if there are multiple
    /// fitness values assigned to a genome (e.g. where multiple measures of fitness are in use) then we need a task specific
    /// comparer to determine the relative fitness between two instances of <see cref="FitnessInfo"/>.
    /// </remarks>
    public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

    /// <summary>
    /// Represents the zero or null fitness for the task. I.e. e.g. for genomes that utterly fail at the task, or genomes that
    /// fail even to decode (not possible in NEAT).
    /// </summary>
    public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

    /// <summary>
    /// Indicates if the evaluators created by <see cref="CreateEvaluator"/> have state.
    /// </summary>
    /// <remarks>
    /// If an evaluator has no state then it is sufficient to create a single instance and to use that evaluator concurrently on multiple threads.
    /// If an evaluator has state then concurrent use requires the creation of one evaluator instance per thread.
    /// </remarks>
    public bool EvaluatorsHaveState => true;

    #endregion

    #region Public Methods

    /// <summary>
    /// Create a new evaluator object.
    /// </summary>
    /// <returns>A new instance of <see cref="IPhenomeEvaluator{T}"/>.</returns>
    public IPhenomeEvaluator<IBlackBox<double>> CreateEvaluator()
    {
        return new CartDoublePoleEvaluator();
    }

    /// <summary>
    /// Accepts a <see cref="FitnessInfo"/>, which is intended to be from the fittest genome in the population, and returns a boolean
    /// that indicates if the evolution algorithm can stop, i.e. because the fitness is the best that can be achieved (or good enough).
    /// </summary>
    /// <param name="fitnessInfo">The fitness info object to test.</param>
    /// <returns>Returns true if the fitness is good enough to signal the evolution algorithm to stop.</returns>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return (fitnessInfo.PrimaryFitness >= 100.0);
    }

    #endregion
}
