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

namespace SharpNeat.NeuralNets
{
    /// <summary>
    /// Vectorized activation function.
    /// </summary>
    /// <param name="v">A span of pre-activation levels to pass through the function.
    /// The resulting post-activation levels are written back to this same span.</param>
    /// <typeparam name="T">Vector element numeric data type.</typeparam>
    public delegate void VecFn<T>(Span<T> v)
        where T : struct;

    /// <summary>
    /// Vectorized activation function.
    /// </summary>
    /// <param name="v">A span of pre-activation levels to pass through the function.</param>
    /// <param name="w">A span in which the post-activation levels are stored.</param>
    /// <typeparam name="T">Vector element numeric data type.</typeparam>
    public delegate void VecFn2<T>(ReadOnlySpan<T> v, Span<T> w)
        where T : struct;

    /// <summary>
    /// Represents the node/neuron activation function of a neural network.
    /// </summary>
    /// <typeparam name="T">Activation function numeric data type.</typeparam>
    public interface IActivationFunction<T> where T : struct
    {
        /// <summary>
        /// The activation function; scalar implementation.
        /// </summary>
        /// <param name="x">The single pre-activation level to pass through the function.</param>
        /// <returns>The activation function output value.</returns>
        T Fn(T x);

        /// <summary>
        /// The activation function; vector implementation.
        /// </summary>
        /// <param name="v">A span of pre-activation levels to pass through the function.
        /// The resulting post-activation levels are written back to this same span.</param>
        void Fn(Span<T> v);

        /// <summary>
        /// The activation function; vector implementation with a separate output span.
        /// </summary>
        /// <param name="v">A span of pre-activation levels to pass through the function.</param>
        /// <param name="w">A span in which the post-activation levels are stored.</param>
        void Fn(ReadOnlySpan<T> v, Span<T> w);
    }
}
