/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
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

namespace SharpNeat.DomainsExtra.InvertedDoublePendulum
{
    /// <summary>
    /// Evaluator for the single pole balancing task.
    /// </summary>
    public class InvertedDoublePendulumEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        #region Constants

        const float __TrackLength = 4.8f;
        const float __TrackLengthHalf = __TrackLength * 0.5f;
        const float __MaxForceNewtons   = 100f;
        const float __MaxForceNewtonsX2 = __MaxForceNewtons * 2f;
        // Some precalced angle constants.
        const float __TwoDegrees		= (float)(Math.PI / 90.0);
        const float __ThreeDegrees		= (float)(Math.PI / 60.0);
        const float __SixDegrees		= (float)(Math.PI / 30.0);	//= 0.1047192;
        const float __TwelveDegrees		= (float)(Math.PI / 15.0);	//= 0.2094384;

        #endregion

        #region Instance Fields

		int	_maxTimesteps;
        float _poleAngleInitial;
        float _heightThreshold;

        // Evaluator state.
        ulong _evalCount;
        bool _stopConditionSatisfied;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
		public InvertedDoublePendulumEvaluator() : this(21600, __ThreeDegrees, 0.8f)
		{}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
		public InvertedDoublePendulumEvaluator(int maxTimesteps, float poleAngleInitial, float heightThreshold)
		{
			_maxTimesteps = maxTimesteps;
            _poleAngleInitial = poleAngleInitial;
			_heightThreshold = heightThreshold;            
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
            // Init sim world. We add extra length to the track to allow cart to overshoot, we then detect overshooting by monitoring the cart's X position 
            // (this is just simpler and more robust than detecting if the cart has hit the ends of the track exactly).
            InvertedDoublePendulumWorld simWorld = new InvertedDoublePendulumWorld(__TrackLength + 0.5f, _poleAngleInitial, 0f);
            simWorld.InitSimulationWorld();

            // Run the pole-balancing simulation.
            int timestep = 1; // Start at 1 because to are using modulus timestep to determine when to apply an external destabilising force, and we don't want that to happen at t=0.
            float toggle = -1f;
            for(; timestep < _maxTimesteps; timestep++)
            {
                // Provide state info to the black box inputs.
                box.InputSignalArray[0] = simWorld.CartPosX / __TrackLengthHalf;    // CartPosX range is +-trackLengthHalf. Here we normalize it to [-1,1].
                box.InputSignalArray[1] = simWorld.CartVelocityX;                   // Cart track velocity x is typically +-0.75.

                box.InputSignalArray[2] = simWorld.CartJointAngle / __TwelveDegrees;// Rescale angle to match range of values during balancing.
                box.InputSignalArray[3] = simWorld.CartJointAngularVelocity;        // Pole angular velocity is typically +-1.0 radians. No scaling required.

                box.InputSignalArray[4] = simWorld.ElbowJointAngle / __TwelveDegrees;
                box.InputSignalArray[5] = simWorld.ElbowJointAngularVelocity;

                // Activate the network.
                box.Activate();

                // Read the network's force signal output.
                float force = (float)(box.OutputSignalArray[0] - 0.5f) * __MaxForceNewtonsX2;

                // Periodically we give the cart a push to test how robust the balancing strategy is to external destabilising forces.
                // A prime number is a good choice here to reduce chances of applying the force at the same point in some oscillatory cycle.
                if(timestep % 397 == 0)
                {
                    force = 200f * toggle;
                    toggle *= -1f;
                }

                // Simulate one timestep.
                simWorld.SetCartForce(force);
                simWorld.Step();

                // Check for failure state. Has the cart run off the ends of the track or has the pole
                // angle gone beyond the threshold.
                float cartPosX = simWorld.CartPosX;
                float poleTopY = simWorld.PoleTopPos.Y;
                if(     (cartPosX < -__TrackLengthHalf) || (cartPosX > __TrackLengthHalf)
                    ||  (poleTopY < _heightThreshold))
                {
                    break;
                }
            }

            _evalCount++;
            if(timestep == _maxTimesteps) {
                _stopConditionSatisfied = true;
            }

            // The controller's fitness is defined as the number of timesteps that elapse before failure.
            double fitness= timestep;
            return new FitnessInfo(fitness, fitness);
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
