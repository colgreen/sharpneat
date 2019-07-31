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
using System.Text;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.SinglePoleBalancing
{
    public class SinglePoleBalancingEvaluator : IPhenomeEvaluator<IBlackBox<double>>
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
        /// <summary>Simulation time increment in seconds.</summary>
		public const double TimeDelta = 0.02;
		const double FourThirds = 4.0/3.0;

		// Some precalced angle constants (in radians).
		const double OneDegree			= Math.PI / 180.0;	//= 0.0174532;
		const double SixDegrees		    = Math.PI / 30.0;	//= 0.1047192;
		const double TwelveDegrees		= Math.PI / 15.0;	//= 0.2094384;
		const double TwentyFourDegrees  = Math.PI / 7.5;	//= 0.2094384;
		const double ThirtySixDegrees	= Math.PI / 5.0;	//= 0.628329;
		const double FiftyDegrees		= Math.PI / 3.6;	//= 0.87266;

		#endregion

        #region Instance Fields

        // Domain parameters.
		readonly double _trackLength;
		readonly double _trackLengthHalf;
		readonly int	_maxTimesteps;
		readonly double _poleAngleThreshold;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
		public SinglePoleBalancingEvaluator() 
            : this(4.8, 100000, TwelveDegrees)
		{}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
		public SinglePoleBalancingEvaluator(
            double trackLength,
            int maxTimesteps,
            double poleAngleThreshold)
		{
			_trackLength = trackLength;
			_trackLengthHalf = trackLength / 2.0;
			_maxTimesteps = maxTimesteps;
			_poleAngleThreshold = poleAngleThreshold;
		}

		#endregion

        #region Public Methods

        public FitnessInfo Evaluate(IBlackBox<double> box)
        {
			// Initialise state. 
            SinglePoleStateData state = new SinglePoleStateData();
            state._poleAngle = SixDegrees;

			// Run the pole-balancing simulation.
            int timestep = 0;
			for(; timestep < _maxTimesteps; timestep++)
			{
				// Provide state info to the black box inputs (normalised to +-1.0).
                box.InputVector[0] = 1.0; // Bias input.
                box.InputVector[1] = state._cartPosX / _trackLengthHalf;    // cart_pos_x range is +-trackLengthHalfed. Here we normalize it to [-1,1].
                box.InputVector[2] = state._cartVelocityX / 0.75;           // cart_velocity_x is typically +-0.75
                box.InputVector[3] = state._poleAngle / TwelveDegrees;      // pole_angle is +-twelve_degrees. Values outside of this range stop the simulation.
                box.InputVector[4] = state._poleAngularVelocity;            // pole_angular_velocity is typically +-1.0 radians. No scaling required.

				// Activate the network.
                box.Activate();

                // Calculate state at next timestep given the black box's output action (push left or push right).
				SimulateTimestep(state, box.OutputVector[0] > 0.5);
		
				// Check for failure state. I.e. has the cart run off the ends of the track, or has the pole
				// angle exceeded a defined threshold.
				if(     (state._cartPosX < -_trackLengthHalf) || (state._cartPosX > _trackLengthHalf)
					||  (state._poleAngle > _poleAngleThreshold) || (state._poleAngle < -_poleAngleThreshold)) 
                {
					break;
                }
			}

            //if(timestep == _maxTimesteps) {
            //    _stopConditionSatisfied = true;
            //}

            // The controller's fitness is defined as the number of timesteps that elapsed before failure.
            double fitness = timestep;
            return new FitnessInfo(fitness);
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
		private void SimulateTimestep(
            SinglePoleStateData state,
            bool action)
		{
			// TODO: Review this maths.
            // TODO: Allow a continuous control input; currently the force has a fixed magnitude and we control only the direction (push cart left or right).
            // TODO: Performance tune. E.g. SinglePoleStateData is a reference type with a ref passed on the stack, this is not ideal. Consider cheap approximations to Cos and Sin (see Simon Goodwin approximations).

			double force = action ? ForceMag : -ForceMag;
			double cosTheta = Math.Cos(state._poleAngle);
			double sinTheta = Math.Sin(state._poleAngle);
			double tmp = (force + (PoleMassLength * state._poleAngularVelocity * state._poleAngularVelocity * sinTheta)) / TotalMass;

			double thetaAcc = ((Gravity * sinTheta) - (cosTheta * tmp)) 
                            / (Length * (FourThirds - ((MassPole * cosTheta * cosTheta) / TotalMass)));
			  
			double xAcc  = tmp - ((PoleMassLength * thetaAcc * cosTheta) / TotalMass);
			  

            // TODO: Consider using the classic Runge-Kutta method instead (see https://en.wikipedia.org/wiki/Runge%E2%80%93Kutta_methods)
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
