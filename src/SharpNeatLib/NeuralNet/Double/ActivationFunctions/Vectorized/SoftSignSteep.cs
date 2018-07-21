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
using System.Numerics;

namespace SharpNeat.NeuralNet.Double.ActivationFunctions.Vectorized
{
    /// <summary>
    /// The softsign sigmoid.
    /// This is a variant of softsign that has a steeper slope at and around the origin that 
    /// is intended to be a similar slope to that of LogisticFunctionSteep.
    /// </summary>
    public class SoftSignSteep : IActivationFunction<double>
    {
        /// <summary>
        /// Calculates the output value for the specified input value.
        /// </summary>
        public double Fn(double x)
        {
            return 0.5 + (x / (2.0 * ( 0.2 + Math.Abs(x))));
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
            var vecTwo = new Vector<double>(2.0);
            var vecFifth = new Vector<double>(0.2);
            var vecHalf = new Vector<double>(0.5);

            int width = Vector<double>.Count;

            int i=startIdx;
            for(; i <= endIdx-width; i += width)
            {
                // Load values into a vector.
                var vec = new Vector<double>(v, i);

                // Calc the softsign sigmoid.
                vec = vecHalf + (vec / (vecTwo * (vecFifth + Vector.Abs(vec))));

                // Copy the final result into w.
                vec.CopyTo(w, i);
            }

            // Handle vectors with lengths not an exact multiple of vector width.
            for(; i < endIdx; i++) {
                w[i] = Fn(v[i]);
            }
        }
    }
}
