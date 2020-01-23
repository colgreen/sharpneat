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
using System.Collections.Generic;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.CartPole.SinglePole
{
    /// <summary>
    /// Evaluation scheme for the cart and pole balancing task, with a single pole.
    /// </summary>
    public class CartSinglePoleEvaluationScheme : IBlackBoxEvaluationScheme<double>
    {
        #region Properties

        /// <summary>
        /// The number of black box inputs expected/required by the black box evaluation scheme.
        /// </summary>
        /// <remarks>
        /// The 4 inputs of the single pole balancing task, plus one bias input (input zero).
        /// </remarks>
        public int InputCount => 3;

        /// <summary>
        /// The number of black box outputs expected/required by the black box evaluation scheme.
        /// </summary>
        public int OutputCount => 1;

        /// <summary>
        /// Indicates if the evaluation scheme is deterministic, i.e. will always return the same fitness score for a given genome.
        /// </summary>
        /// <remarks>
        /// An evaluation scheme that has some random/stochastic characteristics may give a different fitness score at each invocation 
        /// for the same genome, such as scheme is non-deterministic.
        /// </remarks>
        public bool IsDeterministic => true;

        /// <summary>
        /// Gets a fitness comparer for the scheme.
        /// </summary>
        public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

        /// <summary>
        /// Gets a fitness comparer for the scheme.
        /// </summary>
        /// <remarks>
        /// Typically there is a single fitness score whereby a higher score is better, however if there are multiple fitness scores
        /// per genome then we need a more general purpose comparer to determine an ordering on FitnessInfo(s), i.e. to be able to 
        /// determine which is the better FitnessInfo between any two.
        /// </remarks>
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
            return new CartSinglePoleEvaluator();
        }

        /// <summary>
        /// Accepts a <see cref="FitnessInfo"/>, which is intended to be from the fittest genome in the population, and returns a boolean
        /// that indicates if the evolution algorithm can stop, i.e. because the fitness is the best that can be achieved (or good enough).
        /// </summary>
        /// <param name="fitnessInfo">The fitness info object to test.</param>
        /// <returns>Returns true if the fitness is good enough to signal the evolution algorithm to stop.</returns>
        public bool TestForStopCondition(FitnessInfo fitnessInfo)
        {
            return (fitnessInfo.PrimaryFitness >= 1448.0);
        }

        #endregion
    }
}
