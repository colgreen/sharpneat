// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.NeuralNets;

/// <summary>
/// Represents a factory for obtaining instances of <see cref="IActivationFunction{T}"/>.
/// </summary>
/// <typeparam name="T">Neural net numeric data type.</typeparam>
public interface IActivationFunctionFactory<T>
    where T : struct
{
    /// <summary>
    /// Get an activation function instance for the given activation function name/ID.
    /// </summary>
    /// <param name="name">Activation function name/ID.</param>
    /// <returns>An instance of <see cref="IActivationFunction{T}"/>.</returns>
    IActivationFunction<T> GetActivationFunction(string name);
}
