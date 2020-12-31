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
    /// A sigmoid formed by two sub-sections of the y=x^2 curve.
    ///
    /// The extremes are implemented as per the leaky ReLU, i.e. there is a linear slop to
    /// ensure there is at least a gradient to follow at the extremes.
    /// </summary>
    public sealed class QuadraticSigmoid : IActivationFunction<double>
    {
        /// <summary>
        /// The activation function; scalar implementation.
        /// </summary>
        /// <param name="x">The single pre-activation level to pass through the function.</param>
        /// <returns>The activation function output value.</returns>
        public double Fn(double x)
        {
            const double t = 0.999;
            const double a = 0.00001;

            // Calc abs(x) and sign(x) with just a single conditional branch
            // (calling those functions individually results in two conditional branches).
            double sign = 1;
            if(x < 0)
            {
                x *= -1;
                sign = -1;
            }

            double y;
            if(x < t) {
                y = t - ((x - t) * (x - t));
            }
            else // if (x >= t)
            {
                y = t + ((x - t) * a);
            }

            return (y * sign * 0.5) + 0.5;
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
