/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;

namespace SharpNeat.NeuralNet.Double.ActivationFunctions.Cppn
{
    /// <summary>
    /// Gaussian activation function. Output range is 0 to 1, that is, the tails of the Gaussian
    /// distribution curve tend towards 0 as abs(x) -> Infinity and the Gaussian peak is at x = 0.
    /// </summary>
    public sealed class Gaussian : IActivationFunction<double>
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public double Fn(double x)
        {
            return Math.Exp(-Math.Pow(x * 2.5, 2.0));
        }

        public void Fn(double[] v)
        {
            // Naive implementation.
            for(int i=0; i<v.Length; i++) {
                v[i] = Fn(v[i]);
            }
        }

        public void Fn(double[] v, int startIdx, int endIdx)
        {
            // Naive implementation.
            for(int i=startIdx; i < endIdx; i++) {
                v[i] = Fn(v[i]);
            }
        }

        public void Fn(double[] v, double[] w, int startIdx, int endIdx)
        {
            // Naive implementation.
            for(int i=startIdx; i < endIdx; i++) {
                w[i] = Fn(v[i]);
            }
        }
    }
}
