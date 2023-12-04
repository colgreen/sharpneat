// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Collections;
using System.Numerics;

namespace SharpNeat.NeuralNets;

/// <summary>
/// Default implementation of <see cref="IActivationFunctionFactory{T}"/>.
///
/// A factory class for obtaining instances of <see cref="IActivationFunction{T}"/>.
/// </summary>
/// <typeparam name="T">Neural net signal and weight data type.</typeparam>
public sealed class DefaultActivationFunctionFactory<T> : IActivationFunctionFactory<T>
    where T : struct
{
    // If true then hardware accelerated activation functions are used when available.
    readonly bool _enableHardwareAcceleration;

    // A dictionary of activation function instances keyed by class name.
    readonly Dictionary<string,IActivationFunction<T>> _fnByName = [];
    readonly object _lockObj;

    /// <summary>
    /// Construct with the provided options.
    /// </summary>
    /// <param name="enableHardwareAcceleration">If true then hardware accelerated activation functions are used when available.</param>
    public DefaultActivationFunctionFactory(bool enableHardwareAcceleration)
    {
        _enableHardwareAcceleration = enableHardwareAcceleration;
        _lockObj = ((ICollection)_fnByName).SyncRoot;
    }

    #region Public Methods

    /// <summary>
    /// Get an activation function instance for the given activation function name/ID.
    /// </summary>
    /// <param name="name">Activation function name/ID.</param>
    /// <returns>An instance of <see cref="IActivationFunction{T}"/>.</returns>
    public IActivationFunction<T> GetActivationFunction(string name)
    {
        lock(_lockObj)
        {
            // Check for an exiting instance in the activation function cache.
            if(_fnByName.TryGetValue(name, out IActivationFunction<T>? actFn))
                return actFn;

            // No entry in the cache, attempt to create a new instance.
            if(_enableHardwareAcceleration && Vector.IsHardwareAccelerated)
            {
                // Attempt to get a hardware accelerated instance.
                actFn = TryCreateVectorized(name);
            }

            // Attempt to get a non hardware-accelerated instance.
            actFn ??= TryCreate(name);

            // TODO: Add ability to register custom functions not defined in the core sharpneat assembly; as per
            // pull request https://github.com/colgreen/sharpneat/pull/40
            if(actFn is not null)
            {
                // Add to the cache for future use.
                _fnByName.Add(name, actFn);
                return actFn;
            }
        }

        throw new ArgumentException($"Unknown activation function name [{name}].", nameof(name));
    }

    #endregion

    #region Private Methods

    private IActivationFunction<T>? TryCreate(string name)
    {
        // Get the generic type parameter name (i.e. Float or Double).
        string valueType = GetType().GetGenericArguments()[0].Name;

        // Build fully namespaced type name.
        string fullName = $"SharpNeat.NeuralNets.{valueType}.ActivationFunctions.{name}";

        // Attempt to get an instance with the full name.
        var actFn = TryCreateFromFullName(fullName);
        if(actFn is not null)
            return actFn;

        // Attempt again in the CPPN sub-namespace.
        fullName = $"SharpNeat.NeuralNets.{valueType}.ActivationFunctions.Cppn.{name}";
        return TryCreateFromFullName(fullName);
    }

    private IActivationFunction<T>? TryCreateVectorized(string name)
    {
        // Get the generic type parameter name (i.e. Float or Double).
        string valueType = GetType().GetGenericArguments()[0].Name;

        // Build fully namespaced type name.
        string fullName = $"SharpNeat.NeuralNets.{valueType}.ActivationFunctions.Vectorized.{name}";

        // Attempt to get an instance with the full name.
        var actFn = TryCreateFromFullName(fullName);
        if(actFn is not null)
            return actFn;

        // Attempt again in the CPPN sub-namespace.
        fullName = $"SharpNeat.NeuralNets.{valueType}.ActivationFunctions.Cppn.Vectorized.{name}";
        return TryCreateFromFullName(fullName);
    }

    private static IActivationFunction<T>? TryCreateFromFullName(string fullName)
    {
        // Attempt to get an activation type with the specified name.
        Type? type = Type.GetType(fullName);

        // If no such type found then return null.
        if(type is null)
            return null;

        return (IActivationFunction<T>?)Activator.CreateInstance(type);
    }

    #endregion
}
