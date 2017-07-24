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
using System.Runtime.CompilerServices;

namespace SharpNeat.NeuralNets
{
    public class ArcSinH : IActivationFunction<double>
    {
        public string Id => "ArcSinH";

        public double Fn(double x)
        {
            // Scaling factor from:
            // https://www.reddit.com/r/MachineLearning/comments/6g5tg1/r_selfnormalizing_neural_networks_improved_elu/diwq7rb/
            return 1.2567348023993685 * ((Asinh(x) + 1.0) * 0.5);
        }

        public void Fn(double[] v)
        {
            // Naive implementation.
            for(int i=0; i<v.Length; i++) {
                v[i]= Fn(v[i]);
            }
        }

        public void Fn(double[] v, int startIdx, int endIdx)
        {
            // Naive implementation.
            for(int i=startIdx; i<endIdx; i++) {
                v[i]= Fn(v[i]);
            }
        }

        public void Fn(double[] v, double[] w, int startIdx, int endIdx)
        {
            // Naive implementation.
            for(int i=startIdx; i<endIdx; i++) {
                w[i]= Fn(v[i]);
            }
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
