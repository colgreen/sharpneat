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

namespace SharpNeat.NeuralNet.Double.ActivationFunctions.Cppn
{
    /// <summary>
    /// Linear activation function with clipping. By 'clipping' we mean the output value is linear between
    /// x = -1 and x = 1. Below -1 and above +1 the output is clipped at -1 and +1 respectively.
    /// </summary>
    public sealed class Linear : IActivationFunction<double>
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public double Fn(double x)
        {
            if(x < -1.0) {
                return -1.0;
            }
            if (x > 1.0) {
                return 1.0;
            }
            return x;
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
