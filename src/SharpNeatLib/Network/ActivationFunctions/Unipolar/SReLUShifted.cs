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
    /// S-shaped rectified linear activation unit (SReLU). Shifted on the x-axis so that x=0 gives y=0.5, in keeping with the logistic sigmoid.
    /// From:
    ///    https://en.wikipedia.org/wiki/Activation_function
    ///    https://arxiv.org/abs/1512.07030 [Deep Learning with S-shaped Rectified Linear Activation Units]
    ///    
    /// </summary>
    public class SReLUShifted : IActivationFunction
    {
        /// <summary>
        /// Default instance provided as a public static field.
        /// </summary>
        public static readonly IActivationFunction __DefaultInstance = new SReLUShifted();

        public string FunctionId => this.GetType().Name;

        public string FunctionString => "";

        public string FunctionDescription => "S-Shaped Rectified Linear Unit (ReLU) with X-axis translation.";

        public bool AcceptsAuxArgs => false;

        public double Calculate(double x, double[] auxArgs)
        {
            const double tl = 0.001; // threshold (left).
            const double tr = 0.999; // threshold (right).
            const double a = 0.00001;
            const double offset = 0.5;

            double y;
            if(x+offset > tl && x+offset < tr) {
                y = x;
            }
            else if(x+offset <= tl) {
                y = tl + ((x+offset) - tl) * a;
            }
            else {
                y = tr + ((x+offset) - tr) * a;
            }

            return y;
        }

        public float Calculate(float x, float[] auxArgs)
        {
            const float tl = 0.001f; // threshold (left).
            const float tr = 0.999f; // threshold (right).
            const float a = 0.00001f;
            const float offset = 0.5f;

            float y;
            if (x + offset > tl && x + offset < tr) {
                y = x+offset;
            }
            else if (x + offset <= tl) {
                y = tl + ((x + offset) - tl) * a;
            } 
            else {
                y = tr + ((x + offset) - tr) * a;
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
