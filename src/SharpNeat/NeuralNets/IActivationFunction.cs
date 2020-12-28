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

namespace SharpNeat.NeuralNets
{
    // TODO: Convert to use Span<T> instead of T[].

    /// <summary>
    /// Vectorized activation function.
    /// </summary>
    /// <param name="v">A vector of pre-activation levels to pass through the function.
    /// The resulting post-activation levels are written back to this same array/vector.</param>
    /// <typeparam name="T">Vector element numeric data type.</typeparam>
    public delegate void VecFn<T>(T[] v)
        where T : struct;

    /// <summary>
    /// Vectorized activation function with activity limited to a defined sub-range/segment of the vector.
    /// </summary>
    /// <param name="v">A vector of pre-activation levels to pass through the function.
    /// The resulting post-activation levels are written back to this same array/vector.</param>
    /// <param name="startIdx">Start index.</param>
    /// <param name="endIdx">End index (exclusive).</param>
    /// <typeparam name="T">Vector element numeric data type.</typeparam>
    public delegate void VecFnSegment<T>(T[] v, int startIdx, int endIdx)
        where T : struct;

    /// <summary>
    /// Vectorized activation function with activity limited to a defined sub-range/segment of the vector,
    /// and post-activation levels stored in a separate supplied vector.
    /// </summary>
    /// <param name="v">A vector of pre-activation levels to pass through the function.</param>
    /// <param name="w">A vector in which the post-activation levels are stored.</param>
    /// <param name="startIdx">Start index.</param>
    /// <param name="endIdx">End index (exclusive).</param>
    /// <typeparam name="T">Vector element numeric data type.</typeparam>
    public delegate void VecFnSegment2<T>(T[] v, T[] w, int startIdx, int endIdx)
        where T : struct;

    /// <summary>
    /// Represents the node/neuron activation function of a neural network.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public interface IActivationFunction<T> where T : struct
    {
        /// <summary>
        /// The activation function.
        /// </summary>
        /// <param name="x">The single pre-activation level to pass through the function.</param>
        /// <returns>The activation function output value.</returns>
        T Fn(T x);

        /// <summary>
        /// Vectorized activation function.
        /// </summary>
        /// <param name="v">A vector of pre-activation levels to pass through the function.
        /// The resulting post-activation levels are written back to this same array/vector.</param>
        void Fn(T[] v);

        /// <summary>
        /// Vectorized activation function with activity limited to a defined sub-range/segment of the vector.
        /// </summary>
        /// <param name="v">A vector of pre-activation levels to pass through the function.
        /// The resulting post-activation levels are written back to this same array/vector.</param>
        /// <param name="startIdx">Start index.</param>
        /// <param name="endIdx">End index (exclusive).</param>
        void Fn(T[] v, int startIdx, int endIdx);

        /// <summary>
        /// Vectorized activation function with activity limited to a defined sub-range/segment of the vector,
        /// and post-activation levels stored in a separate supplied vector.
        /// </summary>
        /// <param name="v">A vector of pre-activation levels to pass through the function.</param>
        /// <param name="w">A vector in which the post-activation levels are stored.</param>
        /// <param name="startIdx">Start index.</param>
        /// <param name="endIdx">End index (exclusive).</param>
        void Fn(T[] v, T[] w, int startIdx, int endIdx);
    }
}
