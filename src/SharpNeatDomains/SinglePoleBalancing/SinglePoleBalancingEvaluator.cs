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

namespace SharpNeat.Domains.SinglePoleBalancing
{
    /// <summary>
    /// Evaluator for the single pole balancing task.
    /// </summary>
    public class SinglePoleBalancingEvaluator : IPhenomeEvaluator<IBlackBox>
    {
		#region Constants

		// Some physical model constants.
		const double Gravity = 9.8;
		const double MassCart = 1.0;
		const double MassPole = 0.1;
		const double TotalMass = (MassPole + MassCart);
		const double Length = 0.5;	  // actually half the pole's length.
		const double PoleMassLength = (MassPole * Length);
		const double ForceMag = 10.0;
        /// <summary>Time increment interval in seconds.</summary>
		public const double TimeDelta = 0.02;
		const double FourThirds = 4.0/3.0;

		// Some precalced angle constants.
		const double OneDegree			= Math.PI / 180.0;	//= 0.0174532;
		const double SixDegrees		    = Math.PI / 30.0;	//= 0.1047192;
		const double TwelveDegrees		= Math.PI / 15.0;	//= 0.2094384;
		const double TwentyFourDegrees  = Math.PI / 7.5;	//= 0.2094384;
		const double ThirtySixDegrees	= Math.PI / 5.0;	//= 0.628329;
		const double FiftyDegrees		= Math.PI / 3.6;	//= 0.87266;

		#endregion

        #region Class Variables

        // Domain parameters.
		double _trackLength;
		double _trackLengthHalf;
		int	_maxTimesteps;
		double _poleAngleThreshold;

        // Evaluator state.
        ulong _evalCount;
        bool _stopConditionSatisfied;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
		public SinglePoleBalancingEvaluator() : this(4.8, 100000, TwelveDegrees)
		{}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
		public SinglePoleBalancingEvaluator(double trackLength, int maxTimesteps, double poleAngleThreshold)
		{
			_trackLength = trackLength;
			_trackLengthHalf = trackLength / 2.0;
			_maxTimesteps = maxTimesteps;
			_poleAngleThreshold = poleAngleThreshold;
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
            _evalCount++;

			// Initialise state. 
            SinglePoleStateData state = new SinglePoleStateData();
            state._poleAngle = SixDegrees;

			// Run the pole-balancing simulation.
            int timestep = 0;
			for(; timestep < _maxTimesteps; timestep++)
			{
				// Provide state info to the black box inputs (normalised to +-1.0).
                box.InputSignalArray[0] = state._cartPosX / _trackLengthHalf;    // cart_pos_x range is +-trackLengthHalfed. Here we normalize it to [-1,1].
                box.InputSignalArray[1] = state._cartVelocityX / 0.75;             // cart_velocity_x is typically +-0.75
                box.InputSignalArray[2] = state._poleAngle / TwelveDegrees;        // pole_angle is +-twelve_degrees. Values outside of this range stop the simulation.
                box.InputSignalArray[3] = state._poleAngularVelocity;              // pole_angular_velocity is typically +-1.0 radians. No scaling required.

				// Activate the network.
                box.Activate();

                // Calculate state at next timestep given the black box's output action (push left or push right).
				SimulateTimestep(state, box.OutputSignalArray[0] > 0.5);
		
				// Check for failure state. Has the cart run off the ends of the track or has the pole
				// angle gone beyond the threshold.
				if(     (state._cartPosX < -_trackLengthHalf) || (state._cartPosX > _trackLengthHalf)
					||  (state._poleAngle > _poleAngleThreshold) || (state._poleAngle < -_poleAngleThreshold)) 
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

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {   
        }

        #endregion

        #region Private Methods

		/// <summary>
        /// Calculates a state update for the next timestep using current model state and a single 'action' from the
        /// controller. The action specifies if the controller is pushing the cart left or right. Note that this is a binary 
        /// action and therefore full force is always applied to the cart in some direction. This is the standard model for
        /// the single pole balancing task.
		/// </summary>
        /// <param name="state">Model state.</param>
		/// <param name="action">push direction, left(false) or right(true). Force magnitude is fixed.</param>
		private void SimulateTimestep(SinglePoleStateData state, bool action)
		{
			//float xacc,thetaacc,force,costheta,sintheta,temp;
			double force = action ? ForceMag : -ForceMag;
			double cosTheta = Math.Cos(state._poleAngle);
			double sinTheta = Math.Sin(state._poleAngle);
			double tmp = (force + (PoleMassLength * state._poleAngularVelocity * state._poleAngularVelocity * sinTheta)) / TotalMass;

			double thetaAcc = ((Gravity * sinTheta) - (cosTheta * tmp)) 
                            / (Length * (FourThirds - ((MassPole * cosTheta * cosTheta) / TotalMass)));
			  
			double xAcc  = tmp - ((PoleMassLength * thetaAcc * cosTheta) / TotalMass);
			  

			// Update the four state variables, using Euler's method.
			state._cartPosX				+= TimeDelta * state._cartVelocityX;
			state._cartVelocityX		+= TimeDelta * xAcc;
			state._poleAngle			+= TimeDelta * state._poleAngularVelocity;
			state._poleAngularVelocity	+= TimeDelta * thetaAcc;
            state._action = action;
		}

        #endregion
    }
}
