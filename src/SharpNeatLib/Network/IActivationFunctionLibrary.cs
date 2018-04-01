/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

using SharpNeat.NeuralNet;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a library of activation functions. Primarily for use in HyperNEAT CPPNs which define
    /// a activation function per CPPN node.
    /// </summary>
    public interface IActivationFunctionLibrary
    {
        /// <summary>
        /// Gets an instance of an activation function with the specified index in the library.
        /// </summary>
        IActivationFunction<T> GetActivationFunction<T>(int idx) where T : struct;

        /// <summary>
        /// Gets an instance of an activation function with the specified index in the library.
        /// </summary>
        IActivationFunction<T> GetActivationFunction<T>(string id) where T : struct;
    }
}
