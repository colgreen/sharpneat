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
    /// Leaky rectified linear activation unit (ReLU).
    /// Shifted on the x-axis so that x=0 gives y=0.5, in keeping with the logistic sigmoid.
    /// </summary>
    public sealed class LeakyReLUShifted : IActivationFunction<double>
    {
        /// <summary>
        /// The activation function; scalar implementation.
        /// </summary>
        /// <param name="x">The single pre-activation level to pass through the function.</param>
        /// <returns>The activation function output value.</returns>
        public double Fn(double x)
        {
            const double a = 0.001;
            const double offset = 0.5;

            double y;
            if (x + offset > 0.0) {
                y = x + offset;
            } else {
                y = (x + offset) * a;
            }
            return y;
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
