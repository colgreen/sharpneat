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

namespace SharpNeat.NeuralNet.Double.ActivationFunctions
{
    /// <summary>
    /// A sigmoid formed by two sub-sections of the y=x^2 curve.
    /// 
    /// The extremes are implemented as per the leaky ReLU, i.e. there is a linear slop to 
    /// ensure there is at least a gradient to follow at the extremes.
    /// </summary>
    public class QuadraticSigmoid : IActivationFunction<double>
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public double Fn(double x)
        {
            const double t = 0.999;
            const double a = 0.00001;

            // Calc abs(x) and sign(x) with just a single conditional branch 
            // (calling those functions individually results in two conditional branches).
            double sign = 1;
            if(x < 0)
            {
                x = x * -1;  
                sign = -1;
            }

            double y = 0;
            if(x < t) {
                y = t - ((x - t) * (x - t));
            }
            else //if (x >= t) 
            {
                y = t + (x - t) * a;
            }

            return (y * sign * 0.5) + 0.5;
        }

        public void Fn(double[] v)
        {
            // Naive implementation.
            for(int i=0; i < v.Length; i++) {
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
