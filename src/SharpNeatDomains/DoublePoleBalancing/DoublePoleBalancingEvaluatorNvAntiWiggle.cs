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

namespace SharpNeat.Domains.DoublePoleBalancing
{
    /// <summary>
    /// Evaluator for the double pole balancing task with no velocity (NV) inputs and an alternative
    /// evaulation scheme that punsihes fast oscillations (anti-wiggle).
    /// </summary>
    public class DoublePoleBalancingEvaluatorNvAntiWiggle : DoublePoleBalancingEvaluator
    {
        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
		public DoublePoleBalancingEvaluatorNvAntiWiggle() : base(4.8, 50000, ThirtySixDegrees)
		{}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
		public DoublePoleBalancingEvaluatorNvAntiWiggle(double trackLength, int maxTimesteps, double poleAngleThreshold)
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

		    JiggleBuffer jiggleBuffer1 = new JiggleBuffer(100);
		    JiggleBuffer jiggleBuffer2 = new JiggleBuffer(100);
            
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
		
                // Jiggle buffer updates..
				if(100 == jiggleBuffer1.Length)
				{	// Feed an old value from buffer 1 into buffer2.
					jiggleBuffer2.Enqueue(jiggleBuffer1.Dequeue());
				}

				// Place the latest jiggle value into buffer1.
				jiggleBuffer1.Enqueue(	Math.Abs(state[0]) + Math.Abs(state[1]) + 
										Math.Abs(state[2]) + Math.Abs(state[3]));

				// Check for failure state. Has the cart run off the ends of the track or has the pole
				// angle gone beyond the threshold.
				if(     (state[0]< -_trackLengthHalf) || (state[0]> _trackLengthHalf)
					||  (state[2] > _poleAngleThreshold) || (state[2] < -_poleAngleThreshold)
					||  (state[4] > _poleAngleThreshold) || (state[4] < -_poleAngleThreshold))
                {
					break;
                }

				// Give the simulation at least 500 timesteps(5secs) to stabilise before penalising instability.
				if(timestep > 499 && jiggleBuffer2.Total > 30.0)
				{	// Too much wiggling. Stop simulation early (30 was an experimentally determined value).
					break;					
				}
			}

            double fitness;
			if(timestep > 499 && timestep < 600)
			{	// For the 100(1 sec) steps after the 500(5 secs) mark we punish wiggling based
				// on the values from the 1 sec just gone. This is on the basis that the values
				// in jiggleBuffer2 (from 2 to 1 sec ago) will refelct the large amount of
				// wiggling that occurs at the start of the simulation when the system is still stabilising.
				fitness = timestep + (10.0 / Math.Max(1.0, jiggleBuffer1.Total));
			}
			else if(timestep > 599)
			{	// After 600 steps we use jiggleBuffer2 to punsih wiggling, this contains data from between
				// 2 and 1 secs ago. This is on the basis that when the system becomes unstable and causes
				// the simulation to terminate prematurely, the immediately prior 1 secs data will reflect that
				// instability, which may not be indicative of the overall stability of the system up to that time.
				fitness = timestep + (10.0 / Math.Max(1.0, jiggleBuffer2.Total));
			}
			else
			{	// Return just the currentTimestep without any extra fitness component.
				fitness = timestep;
			}

            // Max fitness is # of timesteps + max antiwiggle factor of 10 (which is actually impossible - some wiggle is necessary to balance the poles).
            if(timestep == _maxTimesteps + 10.0) {
                _stopConditionSatisfied = true;
            }
            return new FitnessInfo(fitness, fitness);
        }

        #endregion
    }
}
