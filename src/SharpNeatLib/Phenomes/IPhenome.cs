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
namespace SharpNeat.Phenomes
{
    /// <summary>
    /// Represents an abstract phenome, with input and output vectors, and an activation method.
    /// 
    /// Typically a phenome will be a neural network, whereby we set the input vector, activate the network, and
    /// read its output vector. However in principle a phenome could be any kind of information processing system
    /// such as a C# program or a genetic programming tree.
    /// </summary>
    public interface IPhenome<T> where T : struct
    {
        /// <summary>
        /// Gets the number of inputs to the phenome.
        /// </summary>
        int InputCount { get; }

        /// <summary>
        /// Gets the number of outputs from the phenome.
        /// </summary>
        int OutputCount { get; }

        /// <summary>
        /// Gets an array of input values that feed into the phenome. 
        /// </summary>
        IVector<T> InputVector { get; }

        /// <summary>
        /// Gets an array of output values that feed out from the phenome. 
        /// </summary>
        IVector<T> OutputVector { get; }

        /// <summary>
        /// Activate the phenome. This is a request for the phenome to accept its inputs and produce output signals
        /// ready for reading from OutputVector.
        /// </summary>
        void Activate();

        /// <summary>
        /// Reset any internal state.
        /// </summary>
        void ResetState();
    }
}
