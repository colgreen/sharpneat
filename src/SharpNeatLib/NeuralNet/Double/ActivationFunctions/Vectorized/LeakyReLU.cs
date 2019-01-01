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
using System.Numerics;

namespace SharpNeat.NeuralNet.Double.ActivationFunctions.Vectorized
{
    /// <summary>
    /// Leaky rectified linear activation unit (ReLU).
    /// </summary>
    public class LeakyReLU : IActivationFunction<double>
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public double Fn(double x)
        {
            const double a = 0.001;

            double y;
            if (x > 0.0) {
                y = x;
            } else {
                y = x * a;
            }
            return y;
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
            // Init constants.
            var avec = new Vector<double>(0.001);
            int width = Vector<double>.Count;

            int i=startIdx;
            for(; i <= endIdx-width; i += width)
            {
                // Load values into a vector.
                var vec = new Vector<double>(v, i);

                // Apply max(val, 0) to each element in the vector.
                var maxResult = Vector.Max(vec, Vector<double>.Zero);

                // Apply min(val, 0) to each element in the vector.
                var minResult = Vector.Min(vec, Vector<double>.Zero);

                // Multiply by scaling factor 'a'.
                minResult *= avec;

                // Add minResult and maxResult.
                minResult += maxResult;

                // Copy the final result back into arr.
                minResult.CopyTo(w, i);
            }

            // Handle vectors with lengths not an exact multiple of vector width.
            for(; i < endIdx; i++) {
                w[i] = Fn(v[i]);
            }
        }
    }
}
