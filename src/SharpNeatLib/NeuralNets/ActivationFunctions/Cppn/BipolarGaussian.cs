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

namespace SharpNeat.NeuralNets.Cppn
{
    /// <summary>
    /// Bipolar Gaussian activation function. Output range is -1 to 1, that is, the tails of the Gaussian
    /// distribution curve tend towards -1 as abs(x) -> Infinity and the Gaussian peak is at y = 1.
    /// </summary>
    public class BipolarGaussian : IActivationFunction<double>
    {
        public string Id => "BipolarGaussian";

        public double Fn(double x)
        {
            return (2.0 * Math.Exp(-Math.Pow(x * 2.5, 2.0))) - 1.0;
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
