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

using System;
using System.Collections.Generic;
using Redzen.Numerics;
using SharpNeat.NeuralNets;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a library of activation functions. Primarily for use in HyperNEAT.
    /// </summary>
    public interface IActivationFunctionLibrary
    {
        /// <summary>
        /// Gets the function with the specified index in the library.
        /// </summary>
        IActivationFunction<double> GetFunction(int idx);

        /// <summary>
        /// Gets the function with the specified ID string.
        /// </summary>
        IActivationFunction<double> GetFunction(string id);

        /// <summary>
        /// Randomly select a function based on each function's selection probability.
        /// </summary>
        IActivationFunction<double> GetRandomFunction(IRandomSource rng);

        /// <summary>
        /// Randomly select a function based on each function's selection probability.
        /// Returns the index of the function in the function library.
        /// </summary>
        int GetRandomFunctionIndex(IRandomSource rng);

        /// <summary>
        /// Gets a list of all functions in the library.
        /// </summary>
        IList<IActivationFunction<double>> GetFunctionList();
    }
}
