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

namespace SharpNeat.NeuralNets.Double.ActivationFunctions.Cppn
{
    /// <summary>
    /// Bipolar sigmoid activation function. Output range is -1 to 1 instead of the more normal 0 to 1.
    /// </summary>
    public class BipolarSigmoid : IActivationFunction<double>
    {
        public double Fn(double x)
        {
            return (2.0 / (1.0 + Math.Exp(-4.9 * x))) - 1.0;
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
