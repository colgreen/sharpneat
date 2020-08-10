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
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace SharpNeat.NeuralNet
{
    /// <summary>
    /// Default implementation of <see cref="IActivationFunctionFactory{T}"/>.
    /// 
    /// A factory class for obtaining instances of <see cref="IActivationFunction{T}"/>.
    /// </summary>
    /// <typeparam name="T">Neural net signal and weight data type.</typeparam>
    public class DefaultActivationFunctionFactory<T> : IActivationFunctionFactory<T>
        where T : struct
    {
        #region Instance Fields

        /// <summary>
        /// If true then hardware accelerated activation functions are used when available.
        /// </summary>
        readonly bool _enableHardwareAcceleration = false;

        /// <summary>
        /// A dictionary of activation function instances keyed by class name.
        /// </summary>
        readonly Dictionary<string,IActivationFunction<T>> _fnByName = new Dictionary<string, IActivationFunction<T>>();
        readonly object _lockObj;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided options.
        /// </summary>
        /// <param name="enableHardwareAcceleration">If true then hardware accelerated activation functions are used when available.</param>
        public DefaultActivationFunctionFactory(bool enableHardwareAcceleration)
        {
            _enableHardwareAcceleration = enableHardwareAcceleration;
            _lockObj = ((IDictionary)_fnByName).SyncRoot;
        }

        #endregion

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
                if(_fnByName.TryGetValue(name, out IActivationFunction<T>? actFn)) {
                    return actFn;
                }

                // No entry in the cache, attempt to create a new instance.
                if(_enableHardwareAcceleration && Vector.IsHardwareAccelerated)
                {   
                    // Attempt to get a hardware accelerated instance.
                    actFn = TryCreateVectorized(name);
                }

                if(actFn is null)
                {   
                    // Attempt to get a non hardware-accelerated instance.
                    actFn = TryCreate(name);
                }

                // TODO: Add ability to register custom functions not defined in the core sharpneat assembly; as per 
                // pull request https://github.com/colgreen/sharpneat/pull/40
                if(actFn is object)
                {
                    // Add to the cache for future use.
                    _fnByName.Add(name, actFn);
                    return actFn;
                }
            }

            throw new Exception("Unknown activation function name [{name}].");
        }

        #endregion

        #region Private Methods

        private IActivationFunction<T>? TryCreate(string name)
        {
            // Get the generic type parameter name (i.e. Float or Double).
            string valueType = this.GetType().GetGenericArguments()[0].Name;

            // Build fully namespaced type name.
            string fullName = $"SharpNeat.NeuralNet.{valueType}.ActivationFunctions.{name}";

            // Attempt to get an instance with the full name.
            var actFn = TryCreateFromFullName(fullName);
            if(actFn is object) {
                return actFn;
            }

            // Attempt again in the CPPN sub-namespace.
            fullName = $"SharpNeat.NeuralNet.{valueType}.ActivationFunctions.Cppn.{name}";
            return TryCreateFromFullName(fullName);
        }

        private IActivationFunction<T>? TryCreateVectorized(string name)
        {
            // Get the generic type parameter name (i.e. Float or Double).
            string valueType = this.GetType().GetGenericArguments()[0].Name;

            // Build fully namespaced type name.
            string fullName = $"SharpNeat.NeuralNet.{valueType}.ActivationFunctions.Vectorized.{name}";

            // Attempt to get an instance with the full name.
            var actFn = TryCreateFromFullName(fullName);
            if(actFn is object) {
                return actFn;
            }

            // Attempt again in the CPPN sub-namespace.
            fullName = $"SharpNeat.NeuralNet.{valueType}.ActivationFunctions.Cppn.Vectorized.{name}";
            return TryCreateFromFullName(fullName);
        }

        private static IActivationFunction<T>? TryCreateFromFullName(string fullName)
        {
            // Attempt to get an activation type with the specified name.
            Type? type = Type.GetType(fullName);

            // If no such type found then return null.
            if(type is null) {
                return null;
            }

            return (IActivationFunction<T>?)Activator.CreateInstance(type);
        }

        #endregion
    }
}
