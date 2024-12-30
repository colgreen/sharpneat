// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace SharpNeat.NeuralNets;

/// <summary>
/// The activation function.
/// </summary>
/// <param name="vref">>A reference to the head of a span containing pre-activation levels to pass through the function.
/// The resulting post-activation levels are written back to this same span.</param>
/// <param name="len">The length of the span, i.e., the number elements in the span.</param>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public delegate void VecFn<TScalar>(ref TScalar vref, int len)
    where TScalar : unmanaged;

/// <summary>
/// The activation function; unsafe memory span implementation with a separate input and output spans.
/// </summary>
/// <param name="vref">A reference to the head of a span containing pre-activation levels to pass through the function.</param>
/// <param name="wref">A reference to the head of a span in which the post-activation levels are stored.</param>
/// <param name="len">The length of the spans, i.e., the number elements in the spans.</param>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public delegate void VecFn2<TScalar>(ref TScalar vref, ref TScalar wref, int len)
    where TScalar : unmanaged;

/// <summary>
/// Represents a node/neuron activation function.
/// </summary>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public interface IActivationFunction<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <summary>
    /// The activation function; scalar implementation, accepting a single variable reference.
    /// The pre-activation level is read from <paramref name="x"/>; the post-activation result is stored to
    /// the same variable.
    /// </summary>
    /// <param name="x">The variable reference.</param>
    void Fn(ref TScalar x);

    /// <summary>
    /// The activation function; scalar implementation, accepting a single variable reference.
    /// The pre-activation level is read from <paramref name="x"/>; the post-activation result is stored to
    /// <paramref name="y"/>.
    /// </summary>
    /// <param name="x">The pre-activation variable reference.</param>
    /// <param name="y">The post-activation variable reference.</param>
    void Fn(ref TScalar x, ref TScalar y);

    /// <summary>
    /// The activation function; span implementation.
    /// </summary>
    /// <param name="v">A span of pre-activation levels to pass through the function.
    /// The resulting post-activation levels are written back to this same span.</param>
    void Fn(Span<TScalar> v);

    /// <summary>
    /// The activation function; span implementation with a separate input and output spans.
    /// </summary>
    /// <param name="v">A span of pre-activation levels to pass through the function.</param>
    /// <param name="w">A span in which the post-activation levels are stored.</param>
    void Fn(ReadOnlySpan<TScalar> v, Span<TScalar> w);

    /// <summary>
    /// The activation function; unsafe memory span implementation.
    /// </summary>
    /// <param name="vref">A reference to the head of a span containing pre-activation levels to pass through the function.
    /// The resulting post-activation levels are written back to this same span.</param>
    /// <param name="len">The length of the span, i.e., the number elements in the span.</param>
    void Fn(ref TScalar vref, int len);

    /// <summary>
    /// The activation function; unsafe memory span implementation with separate input and output spans.
    /// </summary>
    /// <param name="vref">A reference to the head of a span containing pre-activation levels to pass through the function.</param>
    /// <param name="wref">A reference to the head of a span in which the post-activation levels are stored.</param>
    /// <param name="len">The length of the spans, i.e., the number elements in the spans.</param>
    void Fn(ref TScalar vref, ref TScalar wref, int len);
}
