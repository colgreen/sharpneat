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
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.PreyCapture
{
    /// <summary>
    /// Evaluator for the prey capture task.
    /// </summary>
    public class PreyCaptureEvaluator : IPhenomeEvaluator<IBlackBox<double>>
    {
        readonly PreyCaptureWorld _world;
        readonly int _trialsPerEvaluation;

        #region Construction

        /// <summary>
        /// Construct with the provided task and evaluator parameters.
        /// </summary>
        public PreyCaptureEvaluator(
            int gridSize,
            int preyInitMoves,
            float preySpeed,
            float sensorRange,
            int maxTimesteps,
            int trialsPerEvaluation)
        {
            // Construct a re-usable instance of the prey capture world.
            _world = new PreyCaptureWorld(gridSize, preyInitMoves, preySpeed, sensorRange, maxTimesteps);
            _trialsPerEvaluation = trialsPerEvaluation;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluate the provided black box against the prey capture task,
        /// and return its fitness score.
        /// </summary>
        /// <param name="box">The black box to evaluate.</param>
        public FitnessInfo Evaluate(IBlackBox<double> box)
        {
            // Perform multiple independent trials.
            int fitness = 0;
            for(int i=0; i < _trialsPerEvaluation; i++)
            {
                // Run a single trial, and record if the prey was captured.
                if(_world.RunTrial(box)) {
                    fitness++;
                }
            }

            // Fitness is given by the number of trials in which the agent caught the prey.
            return new FitnessInfo(fitness);
        }

        #endregion
    }
}
