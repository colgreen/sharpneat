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

// Note. This class contains a lot of dense arithmetic expressions; the inclusion of extra parentheses would make
// these much harder to read.
#pragma warning disable SA1407 // Arithmetic expressions should declare precedence.
#pragma warning disable SA1303 // Const field names should begin with upper-case letter
#pragma warning disable SA1308 // Variable names should not be prefixed

namespace SharpNeat.Tasks.CartPole.DoublePole;

/// <summary>
/// Represents the cart-pole physical model (with two poles); providing a model state update method
/// that employs a classic 4th order Runge-Kutta to project to the state at the next timestep.
/// </summary>
/// <remarks>
/// This cart-pole physics code is based on code from https://github.com/colgreen/cartpole-physics
/// which in turn is based on the cart-pole equations from this paper:
///    "Equations of Motion for the Cart and Pole Control Task"
///    <see href="https://sharpneat.sourceforge.io/research/cart-pole/cart-pole-equations.html"/>.
/// </remarks>
public sealed class CartDoublePolePhysicsRK4
{
    #region Constants

    /// <summary>
    /// Gravitational acceleration (in m/s^2). Here g is taken to be the directionless magnitude of the acceleration
    /// caused by gravity (i.e. approximately 9.8 m/s^2 for gravity on Earth). The direction of gravitational acceleration
    /// is taken into account in the formulation of the equations, therefore the sign of g is positive.
    /// </summary>
    const float g = 9.8f;
    /// <summary>
    /// Mass of pole 1 (in kilograms).
    /// </summary>
    const float m = 0.1f;
    /// <summary>
    /// Mass of pole 2 (in kilograms).
    /// </summary>
    const float m2 = 0.01f;
    /// <summary>
    /// Mass of the cart (in kilograms).
    /// </summary>
    const float m_c = 1f;
    /// <summary>
    /// Length of pole 1 (in metres). This is the full length of the pole, and not the half length as used widely
    /// elsewhere in the literature.
    /// </summary>
    const float l = 1f;
    /// <summary>
    /// Half length of pole 1.
    /// </summary>
    const float l_hat = l / 2f;
    /// <summary>
    /// Length of pole 2 (in metres). This is the full length of the pole, and not the half length as used widely
    /// elsewhere in the literature.
    /// </summary>
    const float l2 = 0.1f;
    /// <summary>
    /// Half length of pole 2.
    /// </summary>
    const float l2_hat = l2 / 2.0f;
    /// <summary>
    /// Coefficient of friction between the pole and the cart, i.e. friction at the pole's pivot joint.
    /// </summary>
    const float mu_p = 0.001f;
    /// <summary>
    /// Coefficient of friction between the cart and the track.
    /// </summary>
    const float mu_c = 0.1f;
    /// <summary>
    /// Combined mass of the cart and the two poles.
    /// </summary>
    const float M = m + m2 + m_c;

    /// <summary>
    /// The timestep increment, e.g. 0.01 for 10 millisecond increments.
    /// </summary>
    const float tau = 1f/32f; // 1/32nd of a second.
    const float tau_half = tau / 2f;

    #endregion

    #region Instance Fields

    /// <summary>
    /// The model state variables are:
    ///  [0] x-axis coordinate of the cart (metres).
    ///  [1] x-axis velocity of the cart (m/s).
    ///  [2] Pole 1 angle (radians); deviation from the vertical. Positive is clockwise.
    ///  [3] Pole 1 angular velocity (radians/s). Positive is clockwise.
    ///  [4] Pole 2 angle (radians); deviation from the vertical. Positive is clockwise.
    ///  [5] Pole 2 angular velocity (radians/s). Positive is clockwise.
    /// </summary>
    readonly float[] _state = new float[6];

    // Allocate re-usable working arrays to avoid memory allocation and garbage collection overhead.
    // These are the k1 to k4 gradients as defined by the Runge-Kutta 4th order method; and an
    // intermediate model state s.
    readonly float[] _k1 = new float[6];
    readonly float[] _k2 = new float[6];
    readonly float[] _k3 = new float[6];
    readonly float[] _k4 = new float[6];
    readonly float[] _s = new float[6];

    #endregion

    #region Properties

    /// <summary>
    /// Get the cart-pole model state.
    /// </summary>
    /// <remarks>
    /// The model state variables are:
    ///  [0] x-axis coordinate of the cart (metres).
    ///  [1] x-axis velocity of the cart (m/s).
    ///  [2] Pole 1 angle (radians); deviation from the vertical. Positive is clockwise.
    ///  [3] Pole 1 angular velocity (radians/s). Positive is clockwise.
    ///  [4] Pole 2 angle (radians); deviation from the vertical. Positive is clockwise.
    ///  [5] Pole 2 angular velocity (radians/s). Positive is clockwise.
    /// </remarks>
    public float[] State => _state;

    #endregion

    #region Public Methods

    /// <summary>
    /// Reset model state.
    /// </summary>
    /// <param name="cartPos">Cart position on the track.</param>
    /// <param name="poleAngle1">Pole 1 angle in radians.</param>
    /// <param name="poleAngle2">Pole 2 angle in radians.</param>
    public void ResetState(float cartPos, float poleAngle1, float poleAngle2)
    {
        _state[0] = cartPos;
        _state[1] = 0f;
        _state[2] = poleAngle1;
        _state[3] = 0f;
        _state[4] = poleAngle2;
        _state[5] = 0f;
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
        CalcAccelerations(_state, f, out float xa, out float thetaa1, out float thetaa2);
        // Store a set of model state gradients, e.g. state[0] is the cart x position, therefore gradient[0] is
        // cart x-axis velocity; and state[1] is cart x-axis velocity, therefore gradient[1] is cart x-axis acceleration, etc.
        _k1[0] = _state[1]; // Cart velocity.
        _k1[1] = xa;        // Cart acceleration.
        _k1[2] = _state[3]; // Pole 1 angular velocity.
        _k1[3] = thetaa1;   // Pole 1 angular acceleration.
        _k1[4] = _state[5]; // Pole 2 angular velocity.
        _k1[5] = thetaa2;   // Pole 2 angular acceleration.

        // Project the initial state to new state s2, using the k1 gradients.
        // I.e. multiply each gradient (which is a rate of change) by a time increment (half tau), to give a model state increment;
        // and then add the increments to the initial state to get a new state for time t + tau/2.
        MultiplyAdd(_s, _state, _k1, tau_half);

        // Calc the cart and pole accelerations for the s2 state, and store the k2 gradients
        CalcAccelerations(_s, f, out xa, out thetaa1, out thetaa2);
        _k2[0] = _s[1];
        _k2[1] = xa;
        _k2[2] = _s[3];
        _k2[3] = thetaa1;
        _k2[4] = _s[5];
        _k2[5] = thetaa2;

        // Project the initial state to new state s3, using the k2 gradients.
        MultiplyAdd(_s, _state, _k2, tau_half);

        // Calc the cart and pole accelerations for the s3 state, and store the k3 gradients
        CalcAccelerations(_s, f, out xa, out thetaa1, out thetaa2);
        _k3[0] = _s[1];
        _k3[1] = xa;
        _k3[2] = _s[3];
        _k3[3] = thetaa1;
        _k3[4] = _s[5];
        _k3[5] = thetaa2;

        // Project the initial state to new state s4, using the k3 gradients.
        MultiplyAdd(_s, _state, _k3, tau);

        // Calc the cart and pole accelerations for the s4 state, and store the k4 gradients
        CalcAccelerations(_s, f, out xa, out thetaa1, out thetaa2);
        _k4[0] = _s[1];
        _k4[1] = xa;
        _k4[2] = _s[3];
        _k4[3] = thetaa1;
        _k4[4] = _s[5];
        _k4[5] = thetaa2;

        // Project _state to its new state, using a weighted sum over gradients k1, k2, k3, k4.
        for(int i=0; i < 6; i++)
        {
            _state[i] += (_k1[i] + 2f*_k2[i] + 2f*_k3[i] + _k4[i]) * (tau / 6f);
        }
    }

    #endregion

    #region Private Static Methods

    /// <summary>
    /// Calculate cart acceleration, and pole angular acceleration for a given model state and external horizontal force applied to the cart.
    /// </summary>
    /// <param name="state">The cart-pole model state. The model state variables are:
    ///  [0] x-axis coordinate of the cart (metres).
    ///  [1] x-axis velocity of the cart (m/s).
    ///  [2] Pole 1 angle (radians). Clockwise deviation from the vertical.
    ///  [3] Pole 1 angular velocity (radians/s). Positive is clockwise.
    ///  [4] Pole 2 angle (radians). Clockwise deviation from the vertical.
    ///  [5] Pole 2 angular velocity (radians/s). Positive is clockwise.
    /// </param>
    /// <param name="f">The external horizontal force applied to the cart.</param>
    /// <param name="xa">Returns the cart's horizontal acceleration.</param>
    /// <param name="thetaa1">Returns pole 1's angular acceleration.</param>
    /// <param name="thetaa2">Returns pole 2's angular acceleration.</param>
    private static void CalcAccelerations(
        float[] state,
        float f,
        out float xa,
        out float thetaa1,
        out float thetaa2)
    {
        // Note. This code is primarily written for clarity rather than execution speed, hence it is probably amenable to being optimised somewhat.

        // Extract state into named variables.
        float xv = state[1];
        float theta = state[2];
        float thetav = state[3];
        float theta2 = state[4];
        float thetav2 = state[5];

        // Precompute some reused values (pole 1).
        float sin_theta = MathF.Sin(theta);
        float cos_theta = MathF.Cos(theta);
        float cos_theta_sqr = cos_theta * cos_theta;
        float thetav_sqr = thetav * thetav;

        // Precompute some reused values (pole 2).
        float sin_theta2 = MathF.Sin(theta2);
        float cos_theta2 = MathF.Cos(theta2);
        float cos_theta2_sqr = cos_theta2 * cos_theta2;
        float thetav2_sqr = thetav2 * thetav2;

        // Calc cart horizontal acceleration.
        xa = (g * ((m*sin_theta*cos_theta) + (m2*sin_theta2*cos_theta2))
            - (7f/3f) * (f +(m*l_hat*thetav_sqr*sin_theta) + (m2*l2_hat*thetav2_sqr*sin_theta2) - mu_c*xv)
            - (((mu_p*thetav*cos_theta)/l_hat) + ((mu_p*thetav2*cos_theta2)/l2_hat)))
            / ((m*cos_theta_sqr) + (m2*cos_theta2_sqr) - (7f/3f)*M);

        // Calc pole 1 angular acceleration.
        thetaa1 = (3f/(7f*l_hat)) * (g*sin_theta - xa*cos_theta - ((mu_p * thetav)/(m*l_hat)));

        // Calc pole 2 angular acceleration.
        thetaa2 = (3f/(7f*l2_hat)) * (g*sin_theta2 - xa*cos_theta2 - ((mu_p * thetav2)/(m2*l2_hat)));
    }

    /// <summary>
    /// Fused multiply-add.
    /// </summary>
    /// <param name="dest">Destination array. The results are stored in this array.</param>
    /// <param name="add">The elements in this array are pointwise added to the destination array.</param>
    /// <param name="a">An array to multiply by a scalar.</param>
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
        // A constant bound of 6 is used instead of using the length of one of the arrays; this makes it easier for the JITter to decide
        // whether to unroll the loop or not. We do not manually unroll the loop because at time of writing that resulted in the jitter
        // generating far more instructions; let's just leave it to the jitter to decide whether to unroll or not.
        for(int i=0; i < 6; i++)
        {
            dest[i] = MathF.FusedMultiplyAdd(a[i], scalar, add[i]);
        }
    }

    #endregion
}
