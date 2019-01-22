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
using Redzen.Random;

namespace SharpNeat.Network
{
    /// <summary>
    /// Sine activation function with doubled period.
    /// </summary>
    public sealed class Sine : IActivationFunction
    {
        /// <summary>
        /// Default instance provided as a public static field.
        /// </summary>
        public static readonly IActivationFunction __DefaultInstance = new Sine();

        /// <summary>
        /// Gets the unique ID of the function. Stored in network XML to identify which function a network or neuron 
        /// is using.
        /// </summary>
        public string FunctionId
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// Gets a human readable string representation of the function. E.g 'y=1/x'.
        /// </summary>
        public string FunctionString
        {
            get { return "y = sin(2*x)"; }
        }

        /// <summary>
        /// Gets a human readable verbose description of the activation function.
        /// </summary>
        public string FunctionDescription
        {
            get { return "Sine function with doubled period.\r\nEffective xrange->[-Inf,Inf] yrange[-1,1]"; }
        }       

        /// <summary>
        /// Gets a flag that indicates if the activation function accepts auxiliary arguments.
        /// </summary>
        public bool AcceptsAuxArgs 
        { 
            get { return false; }
        } 

        /// <summary>
        /// Calculates the output value for the specified input value and optional activation function auxiliary arguments.
        /// </summary>
        public double Calculate(double x, double[] auxArgs)
        {
            return Math.Sin(2.0 * x);
        }

        /// <summary>
        /// Calculates the output value for the specified input value and optional activation function auxiliary arguments.
        /// This single precision overload of Calculate() will be used in neural network code 
        /// that has been specifically written to use floats instead of doubles.
        /// </summary>
        public float Calculate(float x, float[] auxArgs)
        {   // ENHANCEMENT: Search for a math lib that operates on single precision floats.
            return (float)Math.Sin(2f * x);
        }

        /// <summary>
        /// For activation functions that accept auxiliary arguments; generates random initial values for aux arguments for newly
        /// added nodes (from an 'add neuron' mutation).
        /// </summary>
        public double[] GetRandomAuxArgs(IRandomSource rng, double connectionWeightRange)
        {
            throw new SharpNeatException("GetRandomAuxArgs() called on activation function that does not use auxiliary arguments.");
        }

        /// <summary>
        /// Genetic mutation for auxiliary argument data.
        /// </summary>
        public void MutateAuxArgs(double[] auxArgs, IRandomSource rng, double connectionWeightRange)
        {
            throw new SharpNeatException("MutateAuxArgs() called on activation function that does not use auxiliary arguments.");
        }
    }
}
