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

		// Some physical model constants.
		const double G = 9.81;
		const double MassCart = 1.0;
		const double MassPole = 0.1;
        const double TotalMassReciprocal = 1.0 / (MassPole + MassCart);
		const double HalfPoleLength = 0.5;
		const double HalfPoleMassLength = (MassPole * HalfPoleLength);
		const double MaxForce = 10.0;               // Maximum force applied to the cart, in Newtons.
		const double Tau = 0.01;                    // Time increment.
		const double SixDegrees = Math.PI / 30.0;	//= 0.1047192 radians;

		#endregion

        #region Instance Fields [Variable Physics Model State]

		double _cartPosX;
		double _cartVelocityX;
		double _poleAngle;
		double _poleAngularVelocity;
        IRandomSource _rng;

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
        public double CartPosX => _cartPosX;
        /// <summary>
        /// Cart velocity (m/s).
        /// </summary>
        public double CartVelocityX => _cartVelocityX;
        /// <summary>
        /// Pole angle (radians). Straight up = 0.
        /// </summary>
        public double PoleAngle => _poleAngle;
        /// <summary>
        /// Pole angular velocity (radians/sec).
        /// </summary>
        public double PoleAngularVelocity => _poleAngularVelocity;

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset the physics model state back to the predefined starting condition.
        /// </summary>
        public void ResetState()
        {
            _cartPosX = 0.0;
            _cartVelocityX = 0.0;
            _poleAngle = SixDegrees;
            _poleAngularVelocity = 0.0;
        }

        /// <summary>
        /// Update the physics model state by one timestep.
        /// </summary>
        /// <param name="forceSign">If positive then push the cart to the right in this timestep; otherwise push the cart to the left. Only the sign on this valus is used.</param>
        public void Update(double force)
        {
            // Determine the force to be applied to the cart.
            // Note. The cart is always being pushed with a constant force, either to the left or right. This was the approach taken in the original
            // cart and pole experiment; it would be trivial to change this to use a variable force input instead.

            // Clip input value to interval [-1,1], then multiple by MaxForce.
            ClipForce(ref force);
            force *= MaxForce;

            // In addition, we inject some random noise into the force variable to beter model a real world physical system;
            // without this the pole may become perfectly balanced at which point the the controller can just output exactly zero force. 
            // Avoiding that scenario may be why the original/canonical single pole balncing task used bang-bang control
            // (see https://en.wikipedia.org/wiki/Bang%E2%80%93bang_control).
            // Note that this can cause the force to slightly exceed MaxForce.
            // Inject noise in the interval [-0.01,0.01]
            force += (_rng.NextDouble()-0.5) * 0.02;

            // Pre-calculate some reusable terms.
            double sinTheta = Math.Sin(_poleAngle);
			double cosTheta = Math.Cos(_poleAngle);
            double thetaVelocitySquaredSinTheta = _poleAngularVelocity * _poleAngularVelocity * sinTheta;

            // Calc angular acceleration of the pole.
            double theta_dot_dot = ((G * sinTheta) + (cosTheta * ((-force - HalfPoleMassLength * thetaVelocitySquaredSinTheta) * TotalMassReciprocal)))
                                 / (HalfPoleLength * (4/3 - (MassPole * cosTheta * cosTheta * TotalMassReciprocal)));
                
            // Calc angular acceleration of the cart.
            double x_dot_dot = (force + HalfPoleMassLength * (thetaVelocitySquaredSinTheta - theta_dot_dot * cosTheta)) * TotalMassReciprocal;

			// Update the four state variables using Euler's method.
			_cartPosX				+= Tau * _cartVelocityX;
			_cartVelocityX		    += Tau * x_dot_dot;
			_poleAngle			    += Tau * _poleAngularVelocity;
			_poleAngularVelocity    += Tau * theta_dot_dot;
        }

        #endregion

        #region Private Static Methods

        private static void ClipForce(ref double x)
        {
            if(x < -1) x = -1.0;
            else if(x > 1.0) x = 1.0;
        }

        #endregion
    }
}
