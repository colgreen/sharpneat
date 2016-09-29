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

using System.Collections.Generic;
using Redzen.Numerics;
using SharpNeat.Utility;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a library of activation functions. Primarily for use in HyperNEAT.
    /// </summary>
    public interface IActivationFunctionLibrary
    {
        /// <summary>
        /// Gets the function with the specified integer ID.
        /// </summary>
        IActivationFunction GetFunction(int id);

        /// <summary>
        /// Randomly select a function based on each function's selection probability.
        /// </summary>
        ActivationFunctionInfo GetRandomFunction(XorShiftRandom rng);

        /// <summary>
        /// Gets a list of all functions in the library.
        /// </summary>
        IList<ActivationFunctionInfo> GetFunctionList();
    }
}
