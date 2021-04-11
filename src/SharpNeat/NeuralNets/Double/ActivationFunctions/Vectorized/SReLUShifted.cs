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
    /// S-shaped rectified linear activation unit (SReLU).
	/// Shifted on the x-axis so that x=0 gives y=0.5, in keeping with the logistic sigmoid.
    /// From:
    ///    https://en.wikipedia.org/wiki/Activation_function
    ///    https://arxiv.org/abs/1512.07030 [Deep Learning with S-shaped Rectified Linear Activation Units].
    /// </summary>
    public sealed class SReLUShifted : IActivationFunction<double>
    {
        /// <summary>
        /// The activation function; scalar implementation, accepting a single variable reference.
        /// The pre-activation level is read from <paramref name="x"/>; the post-activation result is stored to
        /// the same variable.
        /// </summary>
        /// <param name="x">The variable reference.</param>
        public void Fn(ref double x)
        {
            const double tl = 0.001; // threshold (left).
            const double tr = 0.999; // threshold (right).
            const double a = 0.00001;
            const double offset = 0.5;

            if(x + offset > tl && x + offset < tr) {
                x += offset;
            }
            else if(x + offset <= tl) {
                x = tl + ((x + offset - tl) * a);
            }
            else {
                x = tr + ((x + offset - tr) * a);
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
            const double tl = 0.001; // threshold (left).
            const double tr = 0.999; // threshold (right).
            const double a = 0.00001;
            const double offset = 0.5;

            if(x + offset > tl && x + offset < tr) {
                y = x + offset;
            }
            else if(x + offset <= tl) {
                y = tl + ((x + offset - tl) * a);
            }
            else {
                y = tr + ((x + offset - tr) * a);
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
        /// <param name="vref">A reference to the head of a span containing pre-activation levels to pass through the function.
        /// The resulting post-activation levels are written back to this same span.</param>
        /// <param name="len">The length of the span, i.e., the number elements in the span.</param>
        public void Fn(ref double vref, int len)
        {
            // Init constants.
            var vec_tl = new Vector<double>(0.001);
            var vec_tr = new Vector<double>(0.999);
            var vec_a = new Vector<double>(0.00001);
            var offsetVec = new Vector<double>(0.5);

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

                // Add offset.
                vec += offsetVec;

                // Calc values in left hand segment of y=f(x); i.e. x <= tl.
                var vec_left = vec - vec_tl;
                vec_left *= vec_a;
                vec_left += vec_tl;

                // Calc values in right hand segment of y=f(x); i.e. x >= tr.
                var vec_right = vec - vec_tr;
                vec_right *= vec_a;
                vec_right += vec_tr;

                // For each vector element select a value from the correct segment, i.e. vec, vec_left or vec_right.
                var vec_select_left = Vector.LessThan(vec_left, vec_tl);
                var vec_select_right = Vector.GreaterThan(vec_right, vec_tr);

                vec = Vector.ConditionalSelect(vec_select_right, vec_right, vec);
                vec = Vector.ConditionalSelect(vec_select_left, vec_left, vec);

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
        /// <param name="vref">A reference to the head of a span containing pre-activation levels to pass through the function.</param>
        /// <param name="wref">A reference to the head of a span in which the post-activation levels are stored.</param>
        /// <param name="len">The length of the spans, i.e., the number elements in the spans.</param>
        public void Fn(ref double vref, ref double wref, int len)
        {
            // Init constants.
            var vec_tl = new Vector<double>(0.001);
            var vec_tr = new Vector<double>(0.999);
            var vec_a = new Vector<double>(0.00001);
            var offsetVec = new Vector<double>(0.5);

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

                // Add offset.
                vec += offsetVec;

                // Calc values in left hand segment of y=f(x); i.e. x <= tl.
                var vec_left = vec - vec_tl;
                vec_left *= vec_a;
                vec_left += vec_tl;

                // Calc values in right hand segment of y=f(x); i.e. x >= tr.
                var vec_right = vec - vec_tr;
                vec_right *= vec_a;
                vec_right += vec_tr;

                // For each vector element select a value from the correct segment, i.e. vec, vec_left or vec_right.
                var vec_select_left = Vector.LessThan(vec_left, vec_tl);
                var vec_select_right = Vector.GreaterThan(vec_right, vec_tr);

                vec = Vector.ConditionalSelect(vec_select_right, vec_right, vec);
                vec = Vector.ConditionalSelect(vec_select_left, vec_left, vec);

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
