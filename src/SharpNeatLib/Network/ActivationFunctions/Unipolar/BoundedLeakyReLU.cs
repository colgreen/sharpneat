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

namespace SharpNeat.Network.ActivationFunctions.Unipolar
{
    /// <summary>
    /// See https://en.wikipedia.org/wiki/Rectifier_(neural_networks)
    /// A fast activation function that has a simple non-linearity at x=0.
    /// </summary>
    public class BoundedLeakyReLU : IActivationFunction
    {
        /// <summary>
        /// Default instance provided as a public static field.
        /// </summary>
        public static readonly IActivationFunction __DefaultInstance = new BoundedLeakyReLU();

        public string FunctionId => this.GetType().Name;

        public string FunctionString => "";

        public string FunctionDescription => "Leaky Rectified Linear Unit (ReLU)";

        public bool AcceptsAuxArgs => false;

        public double Calculate(double x, double[] auxArgs)
        {
            const double A = 0.01;
            const double B = 1-A;

            if(x > A && x < B) {
                return x;
            }
            else if(x < A) 
            {
                // Trim output at zero.
                if(x < -100.0) {
                    return 0.0;
                }
                return A + x * 0.0001;
            }
            else 
            {
                // Trim output at one.
                if(x > 100.0) {
                    return 1.0;
                }
                return B + x * 0.0001;
            }
        }

        public float Calculate(float x, float[] auxArgs)
        {
            const float A = 0.01f;
            const float B = 1-A;

            if(x > A && x < B) {
                return x;
            }
            else if(x < A) 
            {
                // Trim output at zero.
                if(x < -100f) {
                    return 0f;
                }
                return A + x * 0.0001f;
            }
            else 
            {
                // Trim output at one.
                if(x > 100f) {
                    return 1f;
                }
                return B + x * 0.0001f;
            }
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
