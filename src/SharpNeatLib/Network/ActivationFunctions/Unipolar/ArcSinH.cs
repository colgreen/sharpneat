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
using System;
using System.Runtime.CompilerServices;

namespace SharpNeat.Network
{
    /// <summary>
    /// Rectified linear activation unit (ReLU).
    /// </summary>
    public class ArcSinH : IActivationFunction
    {
        /// <summary>
        /// Default instance provided as a public static field.
        /// </summary>
        public static readonly IActivationFunction __DefaultInstance = new ArcSinH();

        public string FunctionId => this.GetType().Name;

        public string FunctionString => "";

        public string FunctionDescription => "Leaky Rectified Linear Unit (ReLU)";

        public bool AcceptsAuxArgs => false;

        public double Calculate(double x, double[] auxArgs)
        {
            // Scaling factor from:
            // https://www.reddit.com/r/MachineLearning/comments/6g5tg1/r_selfnormalizing_neural_networks_improved_elu/diwq7rb/

            return 1.2567348023993685 * ((Asinh(x) + 1.0) * 0.5);
        }

        public float Calculate(float x, float[] auxArgs)
        {
            // Scaling factor from:
            // https://www.reddit.com/r/MachineLearning/comments/6g5tg1/r_selfnormalizing_neural_networks_improved_elu/diwq7rb/

            return (float)(1.2567348023993685 * ((Asinh(x) + 1.0) * 0.5));
        }

        public double[] GetRandomAuxArgs(XorShiftRandom rng, double connectionWeightRange)
        {
            throw new SharpNeatException("GetRandomAuxArgs() called on activation function that does not use auxiliary arguments.");
        }

        public void MutateAuxArgs(double[] auxArgs, XorShiftRandom rng, ZigguratGaussianSampler gaussianSampler, double connectionWeightRange)
        {
            throw new SharpNeatException("MutateAuxArgs() called on activation function that does not use auxiliary arguments.");
        }

        #region Private Static Methods

        /// <summary>
        /// Hyperbolic Area Sine
        /// </summary>
        /// <param name="value">The real value.</param>
        /// <returns>The hyperbolic angle, i.e. the area of its hyperbolic sector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Asinh(double value)
        {
            return Math.Log(value + Math.Sqrt((value * value) + 1), Math.E);
        }

        #endregion
    }
}
