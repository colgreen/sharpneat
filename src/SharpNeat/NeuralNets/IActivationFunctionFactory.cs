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
    /// <summary>
    /// Represents a factory for obtaining instances of <see cref="IActivationFunction{T}"/>.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public interface IActivationFunctionFactory<T> where T : struct
    {
        /// <summary>
        /// Get an activation function instance for the given activation function name/ID.
        /// </summary>
        /// <param name="name">Activation function name/ID.</param>
        /// <returns>An instance of <see cref="IActivationFunction{T}"/>.</returns>
        IActivationFunction<T> GetActivationFunction(string name);
    }
}
