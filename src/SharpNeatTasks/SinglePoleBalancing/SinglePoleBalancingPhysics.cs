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
using Redzen.Random;

namespace SharpNeat.Tasks.SinglePoleBalancing
{
    /// <summary>
    /// Physics engine/model for the Single Pole Balancing task.
    /// </summary>
    public class SinglePoleBalancingPhysics
    {
		#region Constants

		// Some physics model constants.
		const double G = 9.8;
		const double MassCart = 1.0;
		const double MassPole = 0.1;
        const double TotalMassReciprocal = 1.0 / (MassPole + MassCart);
		const double HalfPoleLength = 0.5;
		const double HalfPoleMassLength = MassPole * HalfPoleLength;
        const double CartFriction = 0.01;           // Coefficient of friction of cart on track.
        const double PoleFriction = 0.0018;         // Coefficient of friction of pole.
		const double Tau = 0.01;                    // Time increment.
		const double SixDegrees = Math.PI / 30.0;	//= 0.1047192 radians;

		#endregion

        #region Instance Fields

        readonly IRandomSource _rng;

        // Physics model state variables.
		double _x;          // Cart position on the track.
		double _x_dot;      // Cart velocity (x_dot).
		double _theta;      // Pole angle (radians).
		double _theta_dot;  // Pole anglular velocity (radians/s)

        #endregion

        #region Conctructor

        public SinglePoleBalancingPhysics()
        {
            _rng = RandomDefaults.CreateRandomSource();
            ResetState();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Cart position (meters from origin).
        /// </summary>
        public double CartPosX => _x;
        /// <summary>
        /// Cart velocity (m/s).
        /// </summary>
        public double CartVelocity => _x_dot;
        /// <summary>
        /// Pole angle (radians). Straight up = 0.
        /// </summary>
        public double PoleAngle => _theta;
        /// <summary>
        /// Pole angular velocity (radians/sec).
        /// </summary>
        public double PoleAngularVelocity => _theta_dot;

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset the physics model state back to the predefined starting condition.
        /// </summary>
        public void ResetState()
        {
            _x = 0.0;
            _x_dot = 0.0;
            _theta = SixDegrees;
            _theta_dot = 0.0;
        }

        /// <summary>
        /// Update the physics model state by one timestep.
        /// </summary>
        /// <param name="force">The force (in Newtons) to be applied to the cart in this timestep. A positive force pushes the cart to the right, a negative force pushes left.</param>
        public void Update(double force)
        {
            const double dt = Tau;

            // Inject some random noise into the force variable to better model a real world physical system;
            // without this the pole may become perfectly balanced at which point the the controller can just output exactly zero force. 
            // Avoiding that scenario may be why the original/canonical single pole balncing task used bang-bang control
            // (see https://en.wikipedia.org/wiki/Bang%E2%80%93bang_control).
            // Inject noise in the interval [-0.01,0.01]
            force += (_rng.NextDouble()-0.5) * 0.02;

            // Pre-calculate some reusable terms.
            double sinTheta = Math.Sin(_theta);
			double cosTheta = Math.Cos(_theta);
            double thetaDotSqrSinTheta = _theta_dot * _theta_dot * sinTheta;

            // Calc angular acceleration of the pole.
            double theta_dot_dot = 
                ((G * sinTheta) + (cosTheta * ((-force - HalfPoleMassLength * thetaDotSqrSinTheta) * TotalMassReciprocal)) - ((PoleFriction * _theta_dot) / HalfPoleMassLength))
              / (HalfPoleLength * (4/3 - (MassPole * cosTheta * cosTheta * TotalMassReciprocal)));
                
            // Calc acceleration of the cart.
            double x_dot_dot = (force + HalfPoleMassLength * (thetaDotSqrSinTheta - theta_dot_dot * cosTheta) - (CartFriction * Math.Sign(_x_dot))) * TotalMassReciprocal;

			// Update the four state variables using Euler's method.
			_x			+= dt * _x_dot;
			_x_dot		+= dt * x_dot_dot;
			_theta		+= dt * _theta_dot;
			_theta_dot  += dt * theta_dot_dot;
        }

        #endregion
    }
}
