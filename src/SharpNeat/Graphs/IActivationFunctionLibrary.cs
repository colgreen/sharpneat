// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.NeuralNets.ActivationFunctions;

namespace SharpNeat.Graphs;

/// <summary>
/// Represents a library of activation functions. Primarily for use in HyperNEAT CPPNs which define
/// a activation function per CPPN node.
/// </summary>
public interface IActivationFunctionLibrary
{
    /// <summary>
    /// Gets an instance of an activation function with the specified index in the library.
    /// </summary>
    /// <param name="idx">Activation function index.</param>
    /// <typeparam name="TScalar">Activation function data type.</typeparam>
    /// <returns>An instance of <see cref="IActivationFunction{T}"/> from the library.</returns>
    IActivationFunction<TScalar> GetActivationFunction<TScalar>(int idx)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>;

    /// <summary>
    /// Gets an instance of an activation function with the specified ID in the library.
    /// </summary>
    /// <param name="id">Activation function ID.</param>
    /// <typeparam name="TScalar">Activation function data type.</typeparam>
    /// <returns>An instance of <see cref="IActivationFunction{T}"/> from the library.</returns>
    IActivationFunction<TScalar> GetActivationFunction<TScalar>(string id)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>;
}
