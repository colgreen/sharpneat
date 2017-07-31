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

namespace SharpNeat.NeuralNets.Double.ActivationFunctions
{
    /// <summary>
    /// A sigmoid formed by two sub-sections of the y=x^2 curve.
    /// 
    /// The extremes are implemented as per the leaky ReLU, i.e. there is a linear slop to 
    /// ensure there is at least a gradient to follow at the extremes.
    /// </summary>
    public class QuadraticSigmoid : IActivationFunction<double>
    {
        public string Id => "QuadraticSigmoid";

        public double Fn(double x)
        {
            const double t = 0.999;
            const double a = 0.00001;

            double sign = Math.Sign(x);
            x = Math.Abs(x);

            double y = 0;
            if(x >= 0 && x < t) {
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
    }
}
