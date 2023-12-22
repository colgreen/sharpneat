/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using Box2DX.Common;
using Redzen.Random;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    /// <summary>
    /// Evaluator for the Walker 2D task.
    /// </summary>
    public class WalkerBox2dEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        const float one_60th = 1f/60f;

        readonly IRandomSource _rng;
        readonly int	_maxTimesteps;

        // Evaluator state.
        ulong _evalCount;

        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
		public WalkerBox2dEvaluator() : this(7200)
		{}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
		public WalkerBox2dEvaluator(int maxTimesteps)
		{
            _rng = RandomDefaults.CreateRandomSource();
            _maxTimesteps = maxTimesteps;
		}

		#endregion

        #region IPhenomeEvaluator<IBlackBox> Members

        /// <summary>
        /// Gets the total number of evaluations that have been performed.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return false; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            _evalCount++;
            double x = EvaluateInner(box);
            return new FitnessInfo(x, 0.0);
        }

        #endregion

        #region Private Methods

        private double EvaluateInner(IBlackBox box)
        {
            // Init Box2D world.
            WalkerWorld world = new WalkerWorld(_rng);
            world.InitSimulationWorld();

            // Create an interface onto the walker.
            WalkerInterface walkerIface = world.CreateWalkerInterface();

            // Create a neural net controller for the walker.
            NeuralNetController walkerController = new NeuralNetController(walkerIface, box, world.SimulationParameters._frameRate);

            Vec2 torsoPos = walkerIface.TorsoPosition;

            // Run the simulation.
            int timestep = 0;
            for(; timestep < _maxTimesteps; timestep++)
            {
                // Simulate one timestep.
                world.Step();
                walkerController.Step();

                torsoPos = walkerIface.TorsoPosition;

                // Test for stopping conditions.
                if(torsoPos.Y < 1.2f || torsoPos.X < -1f || torsoPos.X > 10f)
                {
                    // Stop simulation.
                    break;
                }
            }

            if(torsoPos.X < 0f)
                return 0f;


            float distance = torsoPos.X;
            float velocityAdjusted = distance / ((timestep * one_60th) + 120);
            double fitness = 100f * distance * velocityAdjusted;
            return fitness;
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {   
        }

        #endregion
    }
}
