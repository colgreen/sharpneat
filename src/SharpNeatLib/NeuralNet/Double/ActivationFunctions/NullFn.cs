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

namespace SharpNeat.NeuralNet.Double.ActivationFunctions
{
    /// <summary>
    /// Null activation function. Returns zero regardless of input.
    /// </summary>
    public class NullFn : IActivationFunction<double>
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public double Fn(double x)
        {
            return 0.0;
        }

        public void Fn(double[] v)
        {
            Array.Clear(v, 0, v.Length);
        }

        public void Fn(double[] v, int startIdx, int endIdx)
        {
            Array.Clear(v, startIdx, endIdx - startIdx);
        }

        public void Fn(double[] v, double[] w, int startIdx, int endIdx)
        {
            Array.Clear(w, startIdx, endIdx - startIdx);
        }
    }
}
