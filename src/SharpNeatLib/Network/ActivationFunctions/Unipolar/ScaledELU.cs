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

namespace SharpNeat.Network
{
    /// <summary>
    /// Scaled Exponential Linear Unit (SELU).
    /// 
    /// From:
    ///     Self-Normalizing Neural Networks
    ///     https://arxiv.org/abs/1706.02515
    /// 
    /// Original source code (including parameter values):
    ///     https://github.com/bioinf-jku/SNNs/blob/master/selu.py
    ///    
    /// </summary>
    public class ScaledELU : IActivationFunction
    {
        /// <summary>
        /// Default instance provided as a public static field.
        /// </summary>
        public static readonly IActivationFunction __DefaultInstance = new ScaledELU();

        public string FunctionId => this.GetType().Name;

        public string FunctionString => "";

        public string FunctionDescription => "Scaled Exponential Linear Unit (SELU)";

        public bool AcceptsAuxArgs => false;

        public double Calculate(double x, double[] auxArgs)
        {
            double alpha = 1.6732632423543772848170429916717;
            double scale = 1.0507009873554804934193349852946;

            double y;
            if(x >= 0) {
                y = scale*x;
            } 
            else {
                y = scale*(alpha*Math.Exp(x) - alpha);
            }

            return y;
        }

        public float Calculate(float x, float[] auxArgs)
        {
            float alpha = 1.6732632423543772848170429916717f;
            float scale = 1.0507009873554804934193349852946f;

            float y;
            if (x >= 0) {
                y = scale * x;
            }
            else {
                y = scale * (alpha * (float)Math.Exp(x) - alpha);
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
