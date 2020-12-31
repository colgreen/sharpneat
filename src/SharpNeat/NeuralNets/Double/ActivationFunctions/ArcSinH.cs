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
using System.Runtime.CompilerServices;

namespace SharpNeat.NeuralNets.Double.ActivationFunctions
{
    /// <summary>
    /// The ArcSinH function (inverse hyperbolic sine function).
    /// </summary>
    public sealed class ArcSinH : IActivationFunction<double>
    {
        /// <summary>
        /// The activation function; scalar implementation.
        /// </summary>
        /// <param name="x">The single pre-activation level to pass through the function.</param>
        /// <returns>The activation function output value.</returns>
        public double Fn(double x)
        {
            // Scaling factor from:
            // https://www.reddit.com/r/MachineLearning/comments/6g5tg1/r_selfnormalizing_neural_networks_improved_elu/diwq7rb/
            return 1.2567348023993685 * ((Asinh(x) + 1.0) * 0.5);
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

        #region Private Static Methods

        /// <summary>
        /// Hyperbolic Area Sine.
        /// </summary>
        /// <param name="value">The real value.</param>
        /// <returns>The hyperbolic angle, i.e. the area of its hyperbolic sector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Asinh(double value)
        {
            return Math.Log(value + Math.Sqrt((value * value) + 1), Math.E);
        }

        #endregion
    }
}
