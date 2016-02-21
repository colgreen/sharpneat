/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2012 Colin Green (sharpneat@gmail.com)
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

namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    /// <summary>
    /// Evaluator for the Walker 2D task.
    /// </summary>
    public class WalkerBox2dEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        #region Instance Fields

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
            WalkerWorld world = new WalkerWorld();
            world.InitSimulationWorld();

            // Create an interface onto the walker.
            WalkerInterface walkerIface = world.CreateWalkerInterface();

            // Create a neural net controller for the walker.
            NeuralNetController walkerController = new NeuralNetController(walkerIface, box);

            // Run the simulation.
            LegInterface leftLeg = walkerIface.LeftLegIFace;
            LegInterface rightLeg = walkerIface.RightLegIFace;
            double angleLimit = Math.PI * 0.5;
            double totalAppliedTorque = 0.0;
            int timestep = 0;
            for(; timestep < _maxTimesteps; timestep++)
            {
                // Simulate one timestep.
                world.Step();
                walkerController.Step();
                totalAppliedTorque += walkerIface.TotalAppliedTorque;

                // Test for breaking conditions.
                if(    walkerIface.TorsoPosition.X > 290f || walkerIface.TorsoPosition.Y < 1.30f 
                    || (leftLeg.FootHeight > 0.11f && rightLeg.FootHeight > 0.11f)
                    || Math.Abs(leftLeg.HipJointAngle) > angleLimit || Math.Abs(leftLeg.KneeJointAngle) > angleLimit
                    || Math.Abs(rightLeg.HipJointAngle) > angleLimit || Math.Abs(rightLeg.KneeJointAngle) > angleLimit)
                {   // Stop simulation.
                    break;
                }
            }

            _evalCount++;
            double fitness = Math.Max(0.0, walkerIface.TorsoPosition.X);
            return new FitnessInfo(fitness, walkerIface.TorsoPosition.X);
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
