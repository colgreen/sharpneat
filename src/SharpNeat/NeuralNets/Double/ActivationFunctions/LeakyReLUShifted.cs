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
    /// Leaky rectified linear activation unit (ReLU).
    /// Shifted on the x-axis so that x=0 gives y=0.5, in keeping with the logistic sigmoid.
    /// </summary>
    public sealed class LeakyReLUShifted : IActivationFunction<double>
    {
        /// <summary>
        /// The activation function; scalar implementation, accepting a single variable reference.
        /// The pre-activation level is read from <paramref name="x"/>; the post-activation result is stored to
        /// the same variable.
        /// </summary>
        /// <param name="x">The variable reference.</param>
        public void Fn(ref double x)
        {
            const double a = 0.001;
            const double offset = 0.5;

            x += offset;

            if(x < 0.0)
                x *= a;
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
            const double a = 0.001;
            const double offset = 0.5;

            y = x + offset;

            if(y < 0.0)
                y *= a;
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
    }
}
