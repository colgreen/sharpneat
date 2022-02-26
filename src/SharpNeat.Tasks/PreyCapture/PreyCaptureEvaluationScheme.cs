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
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.PreyCapture;

/// <summary>
/// Evaluation scheme for the prey capture task.
/// </summary>
public sealed class PreyCaptureEvaluationScheme : IBlackBoxEvaluationScheme<double>
{
    #region Instance Fields

    readonly int _preyInitMoves;
    readonly float _preySpeed;
    readonly float _sensorRange;
    readonly int _maxTimesteps;
    readonly int _trialsPerEvaluation;

    #endregion

    #region Properties

    /// <summary>
    /// The number of black box inputs expected/required by the black box evaluation scheme.
    /// </summary>
    /// <remarks>
    /// The 13 inputs of the prey capture task, plus one bias input (input zero).
    /// </remarks>
    public int InputCount => 14;

    /// <summary>
    /// The number of black box outputs expected/required by the black box evaluation scheme.
    /// </summary>
    public int OutputCount => 4;

    /// <summary>
    /// Indicates if the evaluation scheme is deterministic, i.e. will always return the same fitness score for a given genome.
    /// </summary>
    /// <remarks>
    /// An evaluation scheme that has some random/stochastic characteristics may give a different fitness score at each invocation
    /// for the same genome, such a scheme is non-deterministic.
    /// </remarks>
    public bool IsDeterministic => false;

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

    #region Constructor

    /// <summary>
    /// Construct with the provided task and evaluator parameters.
    /// </summary>
    /// <param name="preyInitMoves">Prey initial moves. The number of moves the prey is allowed to move before the agent can move.</param>
    /// <param name="preySpeed">Prey speed; in the interval [0, 1].</param>
    /// <param name="sensorRange">The sensor range of the agent.</param>
    /// <param name="maxTimesteps">The maximum number of simulation timesteps to run without the agent capturing the prey.</param>
    /// <param name="trialsPerEvaluation">The number of prey capture trials to run per evaluation.</param>
    public PreyCaptureEvaluationScheme(
        int preyInitMoves,
        float preySpeed,
        float sensorRange,
        int maxTimesteps,
        int trialsPerEvaluation)
    {
        _preyInitMoves = preyInitMoves;
        _preySpeed = preySpeed;
        _sensorRange = sensorRange;
        _maxTimesteps = maxTimesteps;
        _trialsPerEvaluation = trialsPerEvaluation;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Create a new evaluator object.
    /// </summary>
    /// <returns>A new instance of <see cref="IPhenomeEvaluator{T}"/>.</returns>
    public IPhenomeEvaluator<IBlackBox<double>> CreateEvaluator()
    {
        return new PreyCaptureEvaluator(
            _preyInitMoves,
            _preySpeed,
            _sensorRange,
            _maxTimesteps,
            _trialsPerEvaluation);
    }

    /// <summary>
    /// Accepts a <see cref="FitnessInfo"/>, which is intended to be from the fittest genome in the population, and returns a boolean
    /// that indicates if the evolution algorithm can stop, i.e. because the fitness is the best that can be achieved (or good enough).
    /// </summary>
    /// <param name="fitnessInfo">The fitness info object to test.</param>
    /// <returns>Returns true if the fitness is good enough to signal the evolution algorithm to stop.</returns>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return (fitnessInfo.PrimaryFitness >= _trialsPerEvaluation);
    }

    #endregion
}
