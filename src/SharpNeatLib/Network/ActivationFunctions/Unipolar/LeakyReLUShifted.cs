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
using Redzen.Numerics;

namespace SharpNeat.Network
{
    /// <summary>
    /// Leaky rectified linear activation unit (ReLU).
    /// Shifted on the x-axis so that x=0 gives y=0.5, in keeping with the logistic sigmoid.
    /// </summary>
    public class LeakyReLUShifted : IActivationFunction
    {
        /// <summary>
        /// Default instance provided as a public static field.
        /// </summary>
        public static readonly IActivationFunction __DefaultInstance = new SReLU();

        public string FunctionId => this.GetType().Name;

        public string FunctionString => "";

        public string FunctionDescription => "Leaky Rectified Linear Unit (ReLU) with X-axis translation.";

        public bool AcceptsAuxArgs => false;

        public double Calculate(double x, double[] auxArgs)
        {
            const double a = 0.001;
            const double offset = 0.5;

            double y;
            if (x+offset > 0.0) {
                y = x;
            } else {
                y = (x+offset) * a;
            }
            return y;
        }

        public float Calculate(float x, float[] auxArgs)
        {
            const float a = 0.001f;
            const float offset = 0.5f;

            double y;
            if (x+offset > 0f) {
                y = x;
            } else {
                y = (x+offset) * a;
            }
            return y;
        }

        public double[] GetRandomAuxArgs(XorShiftRandom rng, double connectionWeightRange)
        {
            throw new SharpNeatException("GetRandomAuxArgs() called on activation function that does not use auxiliary arguments.");
        }

        public void MutateAuxArgs(double[] auxArgs, XorShiftRandom rng, ZigguratGaussianSampler gaussianSampler, double connectionWeightRange)
        {
            throw new SharpNeatException("MutateAuxArgs() called on activation function that does not use auxiliary arguments.");
        }
    }
}
