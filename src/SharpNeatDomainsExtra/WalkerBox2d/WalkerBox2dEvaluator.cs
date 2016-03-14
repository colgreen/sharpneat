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
using Box2DX.Common;

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
		public WalkerBox2dEvaluator() : this(600)
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
            const int trialCount = 5;
            double totalX = 0.0;

            for(int i=0; i<trialCount; i++) {
                totalX += EvaluateInner(box);
            }

            // Track number of evaluations.
            _evalCount++;

            double meanX = totalX / trialCount;
            double fitness = meanX * meanX;
            return new FitnessInfo(fitness, meanX);
        }

        #endregion

        #region Private Methods

        private double EvaluateInner(IBlackBox box)
        {
            const double angleLimit = System.Math.PI / 3.0;

            // Init Box2D world.
            WalkerWorld world = new WalkerWorld(_rng);
            world.InitSimulationWorld();

            // Create an interface onto the walker.
            WalkerInterface walkerIface = world.CreateWalkerInterface();

            // Create a neural net controller for the walker.
            NeuralNetController walkerController = new NeuralNetController(walkerIface, box, world.SimulationParameters._frameRate);

            Vec2 hipPos = walkerIface.LeftLegIFace.HipJointPosition;
            double torsoYMin = hipPos.Y;
            double torsoYMax = hipPos.Y;

            // Run the simulation.
            LegInterface leftLeg = walkerIface.LeftLegIFace;
            LegInterface rightLeg = walkerIface.RightLegIFace;
            //double totalAppliedTorque = 0.0;
            int timestep = 0;
            for(; timestep < _maxTimesteps; timestep++)
            {
                // Simulate one timestep.
                world.Step();
                walkerController.Step();
                //totalAppliedTorque += walkerIface.TotalAppliedTorque;

                // Track hip joint height and min/max extents.
                hipPos = walkerIface.LeftLegIFace.HipJointPosition;
                if(hipPos.Y < torsoYMin) {
                    torsoYMin = hipPos.Y;
                }
                else if(hipPos.Y > torsoYMax) {
                    torsoYMax = hipPos.Y;
                }

                double heightRange = torsoYMax - torsoYMin;

                // Test for stopping conditions.
                if (hipPos.X < -0.7 || hipPos.X > 150f  || heightRange > 0.20
                     || System.Math.Abs(leftLeg.HipJointAngle) > angleLimit || System.Math.Abs(leftLeg.KneeJointAngle) > angleLimit
                    || System.Math.Abs(rightLeg.HipJointAngle) > angleLimit || System.Math.Abs(rightLeg.KneeJointAngle) > angleLimit)
                {   // Stop simulation.
                    break;
                }
            }

            // Final fitness calcs / adjustments.            
            return System.Math.Max(hipPos.X, 0.0);
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
