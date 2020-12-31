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

namespace SharpNeat.NeuralNets.Double.ActivationFunctions
{
    /// <summary>
    /// A very close approximation of the logistic function that avoids use of exp() and is therefore
    /// typically much faster to compute, while giving an almost identical sigmoid curve.
    ///
    /// This function was obtained from:
    ///    http://stackoverflow.com/a/34448562/15703
    ///
    ///
    /// This might be based on the Pade approximant:
    ///   https://en.wikipedia.org/wiki/Pad%C3%A9_approximant
    ///   https://math.stackexchange.com/a/107666
    ///
    /// Or perhaps the maple minimax approximation:
    ///   http://www.maplesoft.com/support/helpJP/Maple/view.aspx?path=numapprox/minimax
    ///
    /// This is a variant that has a steeper slope at and around the origin that is intended to be a similar
    /// slope to that of LogisticFunctionSteep.
    ///
    /// </summary>
    public sealed class PolynomialApproximantSteep : IActivationFunction<double>
    {
        /// <summary>
        /// The activation function; scalar implementation.
        /// </summary>
        /// <param name="x">The single pre-activation level to pass through the function.</param>
        /// <returns>The activation function output value.</returns>
        public double Fn(double x)
        {
            x *= 4.9;
            double x2 = x*x;
            double e = 1.0 + Math.Abs(x) + (x2 * 0.555) + (x2 * x2 * 0.143);

            double f = (x > 0) ? (1.0 / e) : e;
            return 1.0 / (1.0 + f);
        }

        /// <summary>
        /// The activation function; vector implementation.
        /// </summary>
        /// <param name="v">A span of pre-activation levels to pass through the function.
        /// The resulting post-activation levels are written back to this same span.</param>
        public void Fn(Span<double> v)
        {
            // Naive implementation.
            for(int i=0; i < v.Length; i++) {
                v[i] = Fn(v[i]);
            }
        }

        /// <summary>
        /// The activation function; vector implementation with a separate output span.
        /// </summary>
        /// <param name="v">A span of pre-activation levels to pass through the function.</param>
        /// <param name="w">A span in which the post-activation levels are stored.</param>
        public void Fn(Span<double> v, Span<double> w)
        {
            // Naive implementation.
            for(int i=0; i < v.Length; i++) {
                w[i] = Fn(v[i]);
            }
        }
    }
}
