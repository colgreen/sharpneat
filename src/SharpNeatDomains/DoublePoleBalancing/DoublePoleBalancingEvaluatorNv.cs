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
using System;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.DoublePoleBalancing
{
    /// <summary>
    /// Evaluator for the double pole balancing task with no velocity (NV) inputs.
    /// </summary>
    public class DoublePoleBalancingEvaluatorNv : DoublePoleBalancingEvaluator
    {
        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
		public DoublePoleBalancingEvaluatorNv() : base()
		{}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
		public DoublePoleBalancingEvaluatorNv(double trackLength, int maxTimesteps, double poleAngleThreshold)
            : base(trackLength, maxTimesteps, poleAngleThreshold)
		{
		}

		#endregion

        #region IPhenomeEvaluator<IBlackBox> Members

        /// <summary>
        /// Evaluate the provided IBlackBox.
        /// </summary>
        public override FitnessInfo Evaluate(IBlackBox box)
        {
            _evalCount++;

		    // [0] - Cart Position (meters).
		    // [1] - Cart velocity (m/s).
		    // [2] - Pole 1 angle (radians)
		    // [3] - Pole 1 angular velocity (radians/sec).
		    // [4] - Pole 2 angle (radians)
		    // [5] - Pole 2 angular velocity (radians/sec).
            double[] state = new double[6];
            state[2] = FourDegrees;
            
			// Run the pole-balancing simulation.
            int timestep = 0;
			for(; timestep < _maxTimesteps; timestep++)
			{
				// Provide state info to the network (normalised to +-1.0).
				// Markovian (With velocity info)
                box.InputSignalArray[0] = state[0] / _trackLengthHalf;  // Cart Position is +-trackLengthHalfed
                box.InputSignalArray[1] = state[2] / ThirtySixDegrees;  // Pole Angle is +-thirtysix_degrees. Values outside of this range stop the simulation.
                box.InputSignalArray[2] = state[4] / ThirtySixDegrees;  // Pole Angle is +-thirtysix_degrees. Values outside of this range stop the simulation.

				// Activate the black box.
                box.Activate();

                // Get black box response and calc next timestep state.
				performAction(state, box.OutputSignalArray[0]);
		
				// Check for failure state. Has the cart run off the ends of the track or has the pole
				// angle gone beyond the threshold.
				if(     (state[0]< -_trackLengthHalf) || (state[0]> _trackLengthHalf)
					||  (state[2] > _poleAngleThreshold) || (state[2] < -_poleAngleThreshold)
					||  (state[4] > _poleAngleThreshold) || (state[4] < -_poleAngleThreshold))
                {
					break;
                }
			}

            if(timestep == _maxTimesteps) {
                _stopConditionSatisfied = true;
            }

            // The controller's fitness is defined as the number of timesteps that elapse before failure.
            double fitness = timestep;
            return new FitnessInfo(fitness, fitness);
        }

        #endregion
    }
}
