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
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.Double.ActivationFunctions.Vectorized
{
    /// <summary>
    /// Rectified linear activation unit (ReLU).
    /// </summary>
    public sealed class ReLU : IActivationFunction<double>
    {
        /// <summary>
        /// The activation function; scalar implementation, accepting a single variable reference.
        /// The pre-activation level is read from <paramref name="x"/>; the post-activation result is stored to
        /// the same variable.
        /// </summary>
        /// <param name="x">The variable reference.</param>
        public void Fn(ref double x)
        {
            if(x < 0.0) {
                x = 0.0;
            }
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
            y = x;

            if(x < 0.0) {
                y = 0.0;
            }
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
            Fn( ref MemoryMarshal.GetReference(v),
                ref MemoryMarshal.GetReference(w),
                v.Length);
        }

        /// <summary>
        /// The activation function; unsafe memory span implementation.
        /// </summary>
        /// <param name="vref">>A reference to the head of a span containing pre-activation levels to pass through the function.
        /// The resulting post-activation levels are written back to this same span.</param>
        /// <param name="len">The length of the span, i.e., the number elements in the span.</param>
        public void Fn(ref double vref, int len)
        {
            // Calc span bounds.
            ref double vrefBound = ref Unsafe.Add(ref vref, len);
            ref double vrefBoundVec = ref Unsafe.Subtract(ref vrefBound, Vector<double>.Count - 1);

            // Loop SIMD vector sized segments.
            for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBoundVec);
                vref = ref Unsafe.Add(ref vref, Vector<double>.Count))
            {
                // Load values into a vector.
                // The odd code pattern is taken from the Vector<T> constructor's source code.
                var vec = Unsafe.ReadUnaligned<Vector<double>>(
                    ref Unsafe.As<double, byte>(ref vref));

                // Apply max(val, 0) to each element in the vector.
                vec = Vector.Max(vec, Vector<double>.Zero);

                // Store the result in the post-activations span.
                Unsafe.WriteUnaligned(
                    ref Unsafe.As<double, byte>(ref vref),
                    vec);
            }

            // Handle vectors with lengths not an exact multiple of vector width.
            for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBound);
                vref = ref Unsafe.Add(ref vref, 1))
            {
                Fn(ref vref);
            }
        }

        /// <summary>
        /// The activation function; unsafe memory span implementation with a separate input and output spans.
        /// </summary>
        /// <param name="vref">>A reference to the head of a span containing pre-activation levels to pass through the function.</param>
        /// <param name="wref">>A reference to the head of a span in which the post-activation levels are stored.</param>
        /// <param name="len">The length of the spans, i.e., the number elements in the spans.</param>
        public void Fn(ref double vref, ref double wref, int len)
        {
            // Calc span bounds.
            ref double vrefBound = ref Unsafe.Add(ref vref, len);
            ref double vrefBoundVec = ref Unsafe.Subtract(ref vrefBound, Vector<double>.Count - 1);

            // Loop SIMD vector sized segments.
            for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBoundVec);
                vref = ref Unsafe.Add(ref vref, Vector<double>.Count),
                wref = ref Unsafe.Add(ref wref, Vector<double>.Count))
            {
                // Load values into a vector.
                // The odd code pattern is taken from the Vector<T> constructor's source code.
                var vec = Unsafe.ReadUnaligned<Vector<double>>(
                    ref Unsafe.As<double, byte>(ref vref));

                // Apply max(val, 0) to each element in the vector.
                vec = Vector.Max(vec, Vector<double>.Zero);

                // Store the result in the post-activations span.
                Unsafe.WriteUnaligned(
                    ref Unsafe.As<double, byte>(ref wref),
                    vec);
            }

            // Handle vectors with lengths not an exact multiple of vector width.
            for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBound);
                vref = ref Unsafe.Add(ref vref, 1),
                wref = ref Unsafe.Add(ref wref, 1))
            {
                Fn(ref vref, ref wref);
            }
        }
    }
}
