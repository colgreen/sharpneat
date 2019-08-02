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

namespace SharpNeat.Tasks.SinglePoleBalancing
{
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
		const double ForceMag = 10.0;               // Fixed magnitude of the force applied to the cart, in Newtons.
		const double Tau = 0.02;                    // Time increment.
		const double SixDegrees = Math.PI / 30.0;	//= 0.1047192 radians;

		#endregion

        #region Instance Fields [Variable Physics Model State]

		double _cartPosX;
		double _cartVelocityX;
		double _poleAngle;
		double _poleAngularVelocity;

        #endregion

        #region Conctructor

        public SinglePoleBalancingPhysics()
        {
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
        public void Update(double forceSign)
        {
            // Determine the force to be applied to the cart.
            // Note. The cart is always being pushed with a constant force, either to the left or right. This was the approach taken in the original
            // cart and pole experiment; it would be trivial to change this to use a variable force input instead.
			double force = Math.Sign(forceSign) * ForceMag;

            // Pre-calculate some reusable terms.
            double sinTheta = Math.Sin(_poleAngle);
			double cosTheta = Math.Cos(_poleAngle);
            double thetaVelocitySquaredSinTheta = _poleAngularVelocity * _poleAngularVelocity * sinTheta;

            // Calc angular acceleration of the pole.
            double theta_dot_dot = ((G * sinTheta) + (cosTheta * ((-force - HalfPoleMassLength * thetaVelocitySquaredSinTheta) * TotalMassReciprocal)))
                                 / (HalfPoleLength * (4/3 - (MassPole * cosTheta * cosTheta * TotalMassReciprocal)));
                
            // Calc angular acceleration of the cart.
            double x_dot_dot = (force + HalfPoleMassLength * (thetaVelocitySquaredSinTheta - theta_dot_dot * cosTheta)) * TotalMassReciprocal;

			// Update the four state variables, using Euler's method.
			_cartPosX				+= Tau * _cartVelocityX;
			_cartVelocityX		    += Tau * x_dot_dot;
			_poleAngle			    += Tau * _poleAngularVelocity;
			_poleAngularVelocity    += Tau * theta_dot_dot;
        }

        #endregion
    }
}
