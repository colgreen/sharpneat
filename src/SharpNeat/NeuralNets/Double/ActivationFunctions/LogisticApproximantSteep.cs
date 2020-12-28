/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Runtime.CompilerServices;

namespace SharpNeat.NeuralNets.Double.ActivationFunctions
{
    /// <summary>
    /// The logistic function with a steepened slope, and implemented using a fast to compute approximation of exp().
    /// See:
    ///   https://stackoverflow.com/a/412988/15703
    ///   https://pdfs.semanticscholar.org/35d3/2b272879a2018a2d33d982639d4be489f789.pdf (A Fast, Compact Approximation of the Exponential Function).
    /// </summary>
    public sealed class LogisticApproximantSteep : IActivationFunction<double>
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public double Fn(double x)
        {
            return 1.0/(1.0 + ExpApprox(-4.9*x));
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

        // Fast exp approximation, from:
        // https://stackoverflow.com/a/412988/15703
        // https://pdfs.semanticscholar.org/35d3/2b272879a2018a2d33d982639d4be489f789.pdf (A Fast, Compact Approximation of the Exponential Function)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ExpApprox(double val)
        {
            long tmp = (long)((1512775 * val) + (1072693248 - 60801));
            return BitConverter.Int64BitsToDouble(tmp << 32);
        }
    }
}
