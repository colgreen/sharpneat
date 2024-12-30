// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace SharpNeat.NeuralNets;

/// <summary>
/// Represents a factory for obtaining instances of <see cref="IActivationFunction{T}"/>.
/// </summary>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public interface IActivationFunctionFactory<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <summary>
    /// Get an activation function instance for the given activation function name/ID.
    /// </summary>
    /// <param name="name">Activation function name/ID.</param>
    /// <returns>An instance of <see cref="IActivationFunction{T}"/>.</returns>
    IActivationFunction<TScalar> GetActivationFunction(string name);
}
