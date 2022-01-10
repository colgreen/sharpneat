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
using System.Runtime.InteropServices;

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
        /// <summary>
        /// The activation function; scalar implementation, accepting a single variable reference.
        /// The pre-activation level is read from <paramref name="x"/>; the post-activation result is stored to
        /// the same variable.
        /// </summary>
        /// <param name="x">The variable reference.</param>
        public void Fn(ref double x)
        {
            x = 1.0 / (1.0 + ExpApprox(-4.9 * x));
        }

        /// <summary>
        /// The activation function; scalar implementation, accepting a single variable reference.
        /// The pre-activation level is read from <paramref name="x"/>; the post-activation result is stored to
        /// <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The pre-activation variable reference.</param>
        /// <param name="y">The post-activation variable reference.</param>
        public void Fn(ref double x, ref double y)
        {
            y = 1.0 / (1.0 + ExpApprox(-4.9 * x));
        }

        /// <summary>
        /// The activation function; span implementation.
        /// </summary>
        /// <param name="v">A span of pre-activation levels to pass through the function.
        /// The resulting post-activation levels are written back to this same span.</param>
        public void Fn(Span<double> v)
        {
            Fn(ref MemoryMarshal.GetReference(v), v.Length);
        }

        /// <summary>
        /// The activation function; span implementation with a separate input and output spans.
        /// </summary>
        /// <param name="v">A span of pre-activation levels to pass through the function.</param>
        /// <param name="w">A span in which the post-activation levels are stored.</param>
        public void Fn(ReadOnlySpan<double> v, Span<double> w)
        {
            // Obtain refs to the spans, and call on to the unsafe ref based overload.
            Fn(ref MemoryMarshal.GetReference(v),
                ref MemoryMarshal.GetReference(w),
                v.Length);
        }

        /// <summary>
        /// The activation function; unsafe memory span implementation.
        /// </summary>
        /// <param name="vref">A reference to the head of a span containing pre-activation levels to pass through the function.
        /// The resulting post-activation levels are written back to this same span.</param>
        /// <param name="len">The length of the span, i.e., the number elements in the span.</param>
        public void Fn(ref double vref, int len)
        {
            // Calc span bounds.
            ref double vrefBound = ref Unsafe.Add(ref vref, len);

            // Loop over span elements, invoking the scalar activation fn for each.
            for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBound);
                vref = ref Unsafe.Add(ref vref, 1))
            {
                Fn(ref vref);
            }
        }

        /// <summary>
        /// The activation function; unsafe memory span implementation with a separate input and output spans.
        /// </summary>
        /// <param name="vref">A reference to the head of a span containing pre-activation levels to pass through the function.</param>
        /// <param name="wref">A reference to the head of a span in which the post-activation levels are stored.</param>
        /// <param name="len">The length of the spans, i.e., the number elements in the spans.</param>
        public void Fn(ref double vref, ref double wref, int len)
        {
            // Calc span bounds.
            ref double vrefBound = ref Unsafe.Add(ref vref, len);

            // Loop over span elements, invoking the scalar activation fn for each.
            for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBound);
                vref = ref Unsafe.Add(ref vref, 1),
                wref = ref Unsafe.Add(ref wref, 1))
            {
                Fn(ref vref, ref wref);
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
