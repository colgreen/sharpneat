/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.Xor
{
    /// <summary>
    /// XOR logic task evaluator.
    /// 
    /// Two inputs supply the two XOR input values.
    /// 
    /// The correct response for the single output is input1 XOR input2.
    /// 
    /// Evaluation consists of querying the provided black box for all possible input combinations (2^2 = 4).
    /// </summary>
    public class XorEvaluator : IPhenomeEvaluator<IBlackBox<double>>
    {
        #region Properties

        /// <summary>
        /// Indicates if the evaluation scheme is deterministic, i.e. will always return the same fitness score for a given genome.
        /// </summary>
        /// <remarks>
        /// An evaluation scheme that has some random/stochastic characteristics may give a different fitness score at each invocation 
        /// for the same genome, such as scheme is non-deterministic.
        /// </remarks>
        public bool IsDeterministic => true;

        /// <summary>
        /// Gets a null fitness score, i.e. for genomes that cannot be assigned a fitness score for whatever reason, e.g.
        /// if a genome failed to decode to a viable phenome that could be tested.
        /// </summary>
        public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

        /// <summary>
        /// Gets a fitness comparer. 
        /// </summary>
        public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

        /// <summary>
        /// True if this phenome evaluator uses evaluation state objects, i.e. <see cref="CreateEvaluationStateObject"/> returns
        /// an object (rather than a null reference), and one of these objects must be passed on each call to <see cref="Evaluate(IBlackBox{double}, object)"/>
        /// </summary>
        public bool UsesEvaluationStateObject => false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluate the provided IBlackBox against the Binary 11-Multiplexer problem domain and return
        /// its fitness score.
        /// </summary>
        /// <param name="box">The black box to evaluate.</param>
        /// <param name="evaluationState">Optional evaluation state object that can be re-used between evaluations.</param>
        public FitnessInfo Evaluate(IBlackBox<double> box, object evaluationState)
        {
            double fitness = 0.0;
            bool success = true;

            // Test case 0, 0.
            double output = Activate(box, 0.0, 0.0);
            success &= output <= 0.5;
            fitness += 1.0 - (output * output);

            // Test case 1, 1.
            box.ResetState();
            output = Activate(box, 1.0, 1.0);
            success &= output <= 0.5;
            fitness += 1.0 - (output * output);

            // Test case 0, 1.
            box.ResetState();
            output = Activate(box, 0.0, 1.0);
            success &= output > 0.5;
            fitness += 1.0 - ((1.0 - output) * (1.0 - output));

            // Test case 1, 0.
            box.ResetState();
            output = Activate(box, 1.0, 0.0);
            success &= output > 0.5;
            fitness += 1.0 - ((1.0 - output) * (1.0 - output));

            // If all four responses were correct then we add 10 to the fitness.
            if(success) {
                fitness += 10.0;
            }

            return new FitnessInfo(fitness);
        }

        /// <summary>
        /// Accepts a <see cref="FitnessInfo"/>, which is intended to be from the fittest genome in the population, and returns a boolean
        /// that indicates if the evolution algorithm can stop, i.e. because the fitness is the best that can be achieved (or good enough).
        /// </summary>
        /// <param name="fitnessInfo">The fitness info object to test.</param>
        /// <returns>Returns true if the fitness is good enough to signal the evolution algorithm to stop.</returns>
        public bool TestForStopCondition(FitnessInfo fitnessInfo)
        {
            return (fitnessInfo.PrimaryFitness >= 10);
        }

        /// <summary>
        /// Create an evaluation state object.
        /// </summary>
        /// <returns>A new instance of an evaluation state object for the current.</returns>
        public object CreateEvaluationStateObject()
        {
            return null;
        }

        #endregion

        #region Private Static Methods

        private static double Activate(
            IBlackBox<double> box,
            double in1, double in2)
        {
            // Bias input.
            box.InputVector[0] = 1.0;

            // XOR inputs.
            box.InputVector[1] = in1;
            box.InputVector[2] = in2;

            // Activate the black box.
            box.Activate();

            // Read output signal.
            double output = box.OutputVector[0];
            Debug.Assert(output >= 0.0, "Unexpected negative output.");
            return output;
        }

        #endregion
    }
}
