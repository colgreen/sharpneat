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
using System.Numerics;

namespace SharpNeat.NeuralNets.Double.ActivationFunctions.Vectorized
{
    /// <summary>
    /// Leaky rectified linear activation unit (ReLU).
    /// </summary>
    public sealed class LeakyReLU : IActivationFunction<double>
    {
        /// <summary>
        /// The activation function; scalar implementation.
        /// </summary>
        /// <param name="x">The single pre-activation level to pass through the function.</param>
        /// <returns>The activation function output value.</returns>
        public double Fn(double x)
        {
            const double a = 0.001;

            double y;
            if (x > 0.0) {
                y = x;
            } else {
                y = x * a;
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
            Fn(v, v);
        }

        /// <summary>
        /// The activation function; vector implementation with a separate output span.
        /// </summary>
        /// <param name="v">A span of pre-activation levels to pass through the function.</param>
        /// <param name="w">A span in which the post-activation levels are stored.</param>
        public void Fn(Span<double> v, Span<double> w)
        {
            // Init constants.
            var avec = new Vector<double>(0.001);
            int width = Vector<double>.Count;

            int i=0;
            for(; i <= v.Length - width; i += width)
            {
                // Load values into a vector.
                var vec = new Vector<double>(v[i..]);

                // Apply max(val, 0) to each element in the vector.
                var maxResult = Vector.Max(vec, Vector<double>.Zero);

                // Apply min(val, 0) to each element in the vector.
                var minResult = Vector.Min(vec, Vector<double>.Zero);

                // Multiply by scaling factor 'a'.
                minResult *= avec;

                // Add minResult and maxResult.
                minResult += maxResult;

                // Copy the final result back into arr.
                minResult.CopyTo(w[i..]);
            }

            // Handle vectors with lengths not an exact multiple of vector width.
            for(; i < v.Length; i++) {
                w[i] = Fn(v[i]);
            }
        }
    }
}
