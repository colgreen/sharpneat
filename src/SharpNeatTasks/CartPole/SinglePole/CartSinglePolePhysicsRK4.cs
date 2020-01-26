/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;

namespace SharpNeat.Tasks.CartPole.SinglePole
{
    /// <summary>
    /// Represents the cart-pole physical model (with a single pole); providing a model state update method
    /// that employs a classic 4th order Runge-Kutta to project to the state at the next timestep.
    /// </summary>
    /// <remarks>
    /// This cart-pole physics code is based on code from https://github.com/colgreen/cartpole-physics
    /// which in turn is based on the cart-pole equations from this paper: 
    ///    "Equations of Motion for the Cart and Pole Control Task"
    ///    https://sharpneat.sourceforge.io/research/cart-pole/cart-pole-equations.html
    /// </remarks>
    public sealed class CartSinglePolePhysicsRK4
    {
        #region Constants

        /// <summary>
        /// Gravitational acceleration (in m/s^2). Here g is taken to be the directionless magnitude of the acceleration
        /// caused by gravity (i.e. approximately 9.8 m/s^2 for gravity on Earth). The direction of gravitational acceleration
        /// is taken into account in the formulation of the equations, therefore the sign of g is positive.
        /// </summary>
        const float g = 9.8f;
        /// <summary>
        /// Mass of the pole (in kilograms).
        /// </summary>
        const float m = 0.1f;
        /// <summary>
        /// Mass of the cart (in kilograms).
        /// </summary>
        const float m_c = 1f;
        /// <summary>
        /// Length of the pole (in metres). This is the full length of the pole, and not the half length as used widely 
        /// elsewhere in the literature.
        /// </summary>
        const float l = 1f;
        /// <summary>
        /// Half of the pole's length.
        /// </summary>
        const float l_hat = l / 2.0f;
        /// <summary>
        /// Coefficient of friction between the pole and the cart, i.e. friction at the pole's pivot joint.
        /// </summary>
        const float mu_p = 0.001f;
        /// <summary>
        /// Coefficient of friction between the cart and the track.
        /// </summary>
        const float mu_c = 0.1f;
        /// <summary>
        /// Combined mass of the cart and the pole.
        /// </summary>
        const float M = m + m_c;

        /// <summary>
        /// The timestep increment, e.g. 0.01 for 10 millisecond increments.
        /// </summary>
        const float tau = 0.0625f; // 1/16th of a second.
        const float tau_half = tau / 2f;

        #endregion

        #region Instance Fields

        /// <summary>
        /// The model state variables are:
        ///  [0] x-axis coordinate of the cart (metres).
        ///  [1] x-axis velocity of the cart (m/s).
        ///  [2] Pole angle (radians); deviation from the vertical. Positive is clockwise.
        ///  [3] Pole angular velocity (radians/s). Positive is clockwise.
        /// </summary>
        readonly float[] _state = new float[4];

        // Allocate re-usable working arrays to avoid memory allocation and garbage collection overhead.
        // These are the k1 to k4 gradients as defined by the Runge-Kutta 4th order method; and an 
        // intermediate model state s.
        readonly float[] _k1 = new float[4];
        readonly float[] _k2 = new float[4];
        readonly float[] _k3 = new float[4];
        readonly float[] _k4 = new float[4];
        readonly float[] _s = new float[4];

        #endregion

        #region Properties

        /// <summary>
        /// Get the cart-pole model state.
        /// </summary>
        /// <remarks>
        /// The model state variables are:
        ///  [0] x-axis coordinate of the cart (metres).
        ///  [1] x-axis velocity of the cart (m/s).
        ///  [2] Pole angle (radians); deviation from the vertical. Positive is clockwise.
        ///  [3] Pole angular velocity (radians/s). Positive is clockwise.
        /// </remarks>
        public float[] State => _state;

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset model state.
        /// </summary>
        /// <param name="cartPos">Cart position on the track.</param>
        /// <param name="poleAngle">Pole angle in radians.</param>
        public void ResetState(float cartPos, float poleAngle)
        {
            _state[0] = cartPos;
            _state[1] = 0f;
            _state[2] = poleAngle;
            _state[3] = 0f;
        }

        /// <summary>
        /// Update the model state. I.e. move the state forward by one timestep.
        /// </summary>
        /// <param name="f">The external horizontal force applied to the cart.</param>
        /// <remarks>This implementation of Update() uses classic 4th order Runge-Kutta;  this is considerably more
        /// accurate that Euler's method or 2nd order Runge-Kutta for a given timestep size.</remarks>
        public void Update(float f)
        {
            // Calc the cart and pole accelerations for the current/initial model state.
            CalcAccelerations(_state, f, out float xa, out float thetaa);

            // Store a set of model state gradients, e.g. state[0] is the cart x position, therefore gradient[0] is 
            // cart x-axis velocity; and state[1] is cart x-axis velocity, therefore gradient[1] is cart x-axis acceleration, etc.
            _k1[0] = _state[1]; // Cart velocity.
            _k1[1] = xa;        // Cart acceleration.
            _k1[2] = _state[3]; // Pole angular velocity.
            _k1[3] = thetaa;    // Pole angular acceleration.

            // Project the initial state to new state s2, using the k1 gradients.
            // I.e. multiply each gradient (which is a rate of change) by a time increment (half tau), to give a model state increment;
            // and then add the increments to the initial state to get a new state for time t + tau/2.
            MultiplyAdd(_s, _state, _k1, tau_half);

            // Calc the cart and pole accelerations for the s2 state, and store the k2 gradients.
            CalcAccelerations(_s, f, out xa, out thetaa);
            _k2[0] = _s[1];
            _k2[1] = xa;
            _k2[2] = _s[3];
            _k2[3] = thetaa;

            // Project the initial state to new state s3, using the k2 gradients.
            MultiplyAdd(_s, _state, _k2, tau_half);

            // Calc the cart and pole accelerations for the s3 state, and store the k3 gradients.
            CalcAccelerations(_s, f, out xa, out thetaa);
            _k3[0] = _s[1];
            _k3[1] = xa;
            _k3[2] = _s[3];
            _k3[3] = thetaa;

            // Project the initial state to new state s4, using the k3 gradients.
            MultiplyAdd(_s, _state, _k3, tau);

            // Calc the cart and pole accelerations for the s4 state, and store the k4 gradients.
            CalcAccelerations(_s, f, out xa, out thetaa);
            _k4[0] = _s[1];
            _k4[1] = xa;
            _k4[2] = _s[3];
            _k4[3] = thetaa;

            // Project _state to its new state, using a weighted sum over gradients k1, k2, k3, k4.
            for(int i=0; i < 4; i++)
            {
                _state[i] += (_k1[i] + 2f*_k2[i] + 2f*_k3[i] + _k4[i]) * (tau / 6f);
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Calculate cart acceleration, and pole angular acceleration for a given model state and external horizontal force applied to the cart.
        /// </summary>
        /// <param name="state">The cart-pole model state.
        /// <param name="f">The external horizontal force applied to the cart.</param>
        /// <param name="xa">Returns the cart's horizontal acceleration.</param>
        /// <param name="thetaa">Returns the pole's angular acceleration.</param>
        private static void CalcAccelerations(
            float[] state,
            float f,
            out float xa,
            out float thetaa)
        {
            // Note. This code is primarily written for clarity rather than execution speed, hence it is probably amenable to being optimised somewhat.

            // Extract state into named variables.
            float xv = state[1];
            float theta = state[2];
            float thetav = state[3];

            // Precompute some reused values.
            float sin_theta = MathF.Sin(theta);
            float cos_theta = MathF.Cos(theta);
            float cos_theta_sqr = cos_theta * cos_theta;
            float thetav_sqr = thetav * thetav;

            // Calc cart horizontal acceleration.
            xa = (m*g*sin_theta*cos_theta - (7f/3f)*(f + m*l_hat * thetav_sqr * sin_theta - mu_c*xv) - ((mu_p*thetav*cos_theta)/l_hat)) / (m*cos_theta_sqr - (7f/3f)*M);

            // Calc pole angular acceleration.
            thetaa = (3f/(7f*l_hat)) * (g*sin_theta - xa*cos_theta - (mu_p * thetav)/(m*l_hat));
        }

        /// <summary>
        /// Fused multiply-add.
        /// </summary>
        /// <param name="dest">Destination array. The results are stored in this array.</param>
        /// <param name="add">The elements in this array are pointwise added to the destination array.</param>
        /// <param name="a">An array to multiple by a scalar.</param>
        /// <param name="scalar">A scalar to multiply array a by.</param>
        private static void MultiplyAdd(
            float[] dest,
            float[] add,
            float[] a,
            float scalar)
        {
            // ENHANCEMENT: Consider vectorizing.
            // Vectorizing this may not be worth it as there are only 4 values, hence only a single vector op will be executed at most,
            // and if Vector<double>.Count is greater than four then we have to pad our arrays with zeros to match the wider vectors.
            // However, System.Runtime.Intrinsics.X86.Vector128<float> might be a good choice here (for x86 platforms with vector support!).

            // Notes. 
            // A constant bound of 4 is used instead of using the length of one of the arrays; this makes it easier for the JITter to decide
            // whether to unroll the loop or not. We do not manually unroll the loop because at time of writing that resulted in the jitter
            // generating far more instructions; let's just leave it to the jitter to decide whether to unroll or not.
            for(int i=0; i < 4; i++) {
                dest[i] = add[i] + (a[i] * scalar);
            }
        }

        #endregion
    }
}
