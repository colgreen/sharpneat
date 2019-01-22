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
    ///Leaky rectified linear activation unit (ReLU).
    /// </summary>
    public sealed class LeakyReLU : IActivationFunction
    {
        /// <summary>
        /// Default instance provided as a public static field.
        /// </summary>
        public static readonly IActivationFunction __DefaultInstance = new LeakyReLU();

        public string FunctionId => this.GetType().Name;

        public string FunctionString => "";

        public string FunctionDescription => "Leaky Rectified Linear Unit (ReLU)";

        public bool AcceptsAuxArgs => false;

        public double Calculate(double x, double[] auxArgs)
        {
            const double a = 0.001;

            double y;
            if (x > 0.0) {
                y = x;
            } else {
                y = x * a;
            }
            return y;
        }

        public float Calculate(float x, float[] auxArgs)
        {
            const float a = 0.001f;

            float y;
            if (x > 0.0) {
                y = x;
            } else {
                y = x * a;
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
