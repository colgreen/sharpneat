/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2022 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
namespace SharpNeat.NeuralNets;

/// <summary>
/// The activation function.
/// </summary>
/// <param name="vref">>A reference to the head of a span containing pre-activation levels to pass through the function.
/// The resulting post-activation levels are written back to this same span.</param>
/// <param name="len">The length of the span, i.e., the number elements in the span.</param>
/// <typeparam name="T">Activation function numeric data type.</typeparam>
public delegate void VecFn<T>(ref T vref, int len)
    where T : struct;

/// <summary>
/// The activation function; unsafe memory span implementation with a separate input and output spans.
/// </summary>
/// <param name="vref">A reference to the head of a span containing pre-activation levels to pass through the function.</param>
/// <param name="wref">A reference to the head of a span in which the post-activation levels are stored.</param>
/// <param name="len">The length of the spans, i.e., the number elements in the spans.</param>
/// <typeparam name="T">Activation function numeric data type.</typeparam>
public delegate void VecFn2<T>(ref T vref, ref T wref, int len)
    where T : struct;

/// <summary>
/// Represents a node/neuron activation function.
/// </summary>
/// <typeparam name="T">Activation function numeric data type.</typeparam>
public interface IActivationFunction<T> where T : struct
{
    /// <summary>
    /// The activation function; scalar implementation, accepting a single variable reference.
    /// The pre-activation level is read from <paramref name="x"/>; the post-activation result is stored to
    /// the same variable.
    /// </summary>
    /// <param name="x">The variable reference.</param>
    void Fn(ref T x);

    /// <summary>
    /// The activation function; scalar implementation, accepting a single variable reference.
    /// The pre-activation level is read from <paramref name="x"/>; the post-activation result is stored to
    /// <paramref name="y"/>.
    /// </summary>
    /// <param name="x">The pre-activation variable reference.</param>
    /// <param name="y">The post-activation variable reference.</param>
    void Fn(ref T x, ref T y);

    /// <summary>
    /// The activation function; span implementation.
    /// </summary>
    /// <param name="v">A span of pre-activation levels to pass through the function.
    /// The resulting post-activation levels are written back to this same span.</param>
    void Fn(Span<T> v);

    /// <summary>
    /// The activation function; span implementation with a separate input and output spans.
    /// </summary>
    /// <param name="v">A span of pre-activation levels to pass through the function.</param>
    /// <param name="w">A span in which the post-activation levels are stored.</param>
    void Fn(ReadOnlySpan<T> v, Span<T> w);

    /// <summary>
    /// The activation function; unsafe memory span implementation.
    /// </summary>
    /// <param name="vref">A reference to the head of a span containing pre-activation levels to pass through the function.
    /// The resulting post-activation levels are written back to this same span.</param>
    /// <param name="len">The length of the span, i.e., the number elements in the span.</param>
    void Fn(ref T vref, int len);

    /// <summary>
    /// The activation function; unsafe memory span implementation with a separate input and output spans.
    /// </summary>
    /// <param name="vref">A reference to the head of a span containing pre-activation levels to pass through the function.</param>
    /// <param name="wref">A reference to the head of a span in which the post-activation levels are stored.</param>
    /// <param name="len">The length of the spans, i.e., the number elements in the spans.</param>
    void Fn(ref T vref, ref T wref, int len);
}
