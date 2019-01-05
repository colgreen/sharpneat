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
using System.Numerics;

namespace SharpNeat.NeuralNet.Double.ActivationFunctions.Vectorized
{
    /// <summary>
    /// Rectified linear activation unit (ReLU).
    /// </summary>
    public sealed class ReLU : IActivationFunction<double>
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public double Fn(double x)
        {
            return Math.Max(x, 0.0);
        }

        public void Fn(double[] v)
        {
            Fn(v, v, 0, v.Length);
        }

        public void Fn(double[] v, int startIdx, int endIdx)
        {
            Fn(v, v, startIdx, endIdx);
        }

        public void Fn(double[] v, double[] w, int startIdx, int endIdx)
        {
            int width = Vector<double>.Count;

            int i=startIdx;
            for(; i <= endIdx-width; i += width)
            {
                // Load values into a vector.
                var vec = new Vector<double>(v, i);

                // Apply max(val, 0) to each element in the vector.
                var vecResult = Vector.Max(vec, Vector<double>.Zero);

                // Copy the result back into arr.
                vecResult.CopyTo(w, i);
            }

            // Handle vectors with lengths not an exact multiple of vector width.
            for(; i < endIdx; i++) {
                w[i] = Fn(v[i]);
            }
        }
    }
}
