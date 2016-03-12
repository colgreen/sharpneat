/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using Redzen.Numerics;

namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    /// <summary>
    /// Evaluator for the Walker 2D task.
    /// </summary>
    public class WalkerBox2dEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        #region Instance Fields

        XorShiftRandom _rng;
		int	_maxTimesteps;

        // Evaluator state.
        ulong _evalCount;
        bool _stopConditionSatisfied;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
		public WalkerBox2dEvaluator() : this(1800)
		{}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
		public WalkerBox2dEvaluator(int maxTimesteps)
		{
            _rng = new XorShiftRandom();
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
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            // Init Box2D world.
            WalkerWorld world = new WalkerWorld(_rng);
            world.InitSimulationWorld();

            // Create an interface onto the walker.
            WalkerInterface walkerIface = world.CreateWalkerInterface();

            // Create a neural net controller for the walker.
            NeuralNetController walkerController = new NeuralNetController(walkerIface, box, world.SimulationParameters._frameRate);

            // Run the simulation.
            LegInterface leftLeg = walkerIface.LeftLegIFace;
            LegInterface rightLeg = walkerIface.RightLegIFace;
            double totalAppliedTorque = 0.0;
            int timestep = 0;
            for(; timestep < _maxTimesteps; timestep++)
            {
                // Simulate one timestep.
                world.Step();
                walkerController.Step();
                totalAppliedTorque += walkerIface.TotalAppliedTorque;

                // Test for stopping conditions.
                if (walkerIface.TorsoPosition.X < -0.7 || walkerIface.TorsoPosition.X > 150f || walkerIface.TorsoPosition.Y < 1.15f)
                {   // Stop simulation.
                    break;
                }
            }

            // Track number of evaluations.
            _evalCount++;

            // Final fitness calcs / adjustments.
            double fitness; 
            double x = walkerIface.TorsoPosition.X;
            if(x < 0) fitness = 0.0;
            else fitness = x*x;
            return new FitnessInfo(fitness, x);
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
