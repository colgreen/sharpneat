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
using Redzen.Random;

namespace SharpNeat.Network
{
    /// <summary>
    /// y = max(-1, x).
    /// i.e. a rectified linear activation unit (ReLU) variant.
    /// </summary>
    public sealed class MaxMinusOne : IActivationFunction
    {
        /// <summary>
        /// Default instance provided as a public static field.
        /// </summary>
        public static readonly IActivationFunction __DefaultInstance = new MaxMinusOne();

        public string FunctionId => this.GetType().Name;

        public string FunctionString => "y = max(-1, x)";

        public string FunctionDescription => "A variant of Shifted Rectified Linear Unit (ReLU).";

        public bool AcceptsAuxArgs => false;

        public double Calculate(double x, double[] auxArgs)
        {
            double y;
            if (x > -1) {
                y = x;
            } else {
                y = -1;
            }
            return y;
        }

        public float Calculate(float x, float[] auxArgs)
        {
            float y;
            if (x > -1) {
                y = x;
            } else {
                y = -1;
            }
            return y;
        }

        public double[] GetRandomAuxArgs(IRandomSource rng, double connectionWeightRange)
        {
            throw new SharpNeatException("GetRandomAuxArgs() called on activation function that does not use auxiliary arguments.");
        }

        public void MutateAuxArgs(double[] auxArgs, IRandomSource rng, double connectionWeightRange)
        {
            throw new SharpNeatException("MutateAuxArgs() called on activation function that does not use auxiliary arguments.");
        }
    }
}
