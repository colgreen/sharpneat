// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions.Vectorized;

/// <summary>
/// A very close approximation of the logistic function that avoids use of exp() and is therefore
/// typically much faster to compute, while giving an almost identical sigmoid curve.
///
/// This function was obtained from:
///    http://stackoverflow.com/a/34448562/15703
///
///
/// This might be based on the Pade approximant:
///   https://en.wikipedia.org/wiki/Pad%C3%A9_approximant
///   https://math.stackexchange.com/a/107666
///
/// Or perhaps the maple minimax approximation:
///   http://www.maplesoft.com/support/helpJP/Maple/view.aspx?path=numapprox/minimax
///
/// This is a variant that has a steeper slope at and around the origin that is intended to be a similar
/// slope to that of LogisticFunctionSteep.
///
/// </summary>
public sealed class PolynomialApproximantSteep : IActivationFunction<double>
{
    /// <inheritdoc/>
    public void Fn(ref double x)
    {
        x *= 4.9;
        double x2 = x * x;
        double e = 1.0 + Math.Abs(x) + (x2 * 0.555) + (x2 * x2 * 0.143);

        double f = (x > 0) ? (1.0 / e) : e;
        x = 1.0 / (1.0 + f);
    }

    /// <inheritdoc/>
    public void Fn(ref double x, ref double y)
    {
        y = x * 4.9;
        double x2 = y * y;
        double e = 1.0 + Math.Abs(y) + (x2 * 0.555) + (x2 * x2 * 0.143);

        double f = (x > 0) ? (1.0 / e) : e;
        y = 1.0 / (1.0 + f);
    }

    /// <inheritdoc/>
    public void Fn(Span<double> v)
    {
        Fn(ref MemoryMarshal.GetReference(v), v.Length);
    }

    /// <inheritdoc/>
    public void Fn(ReadOnlySpan<double> v, Span<double> w)
    {
        // Obtain refs to the spans, and call on to the unsafe ref based overload.
        Fn(
            ref MemoryMarshal.GetReference(v),
            ref MemoryMarshal.GetReference(w),
            v.Length);
    }

    /// <inheritdoc/>
    public void Fn(ref double vref, int len)
    {
        // Init constant vectors.
        var vec_4_9 = new Vector<double>(4.9);
        var vec_0_555 = new Vector<double>(0.555);
        var vec_0_143 = new Vector<double>(0.143);

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

            vec *= vec_4_9;
            var vec_x2 = vec * vec;
            var vec_e = Vector<double>.One + Vector.Abs(vec) + (vec_x2 * vec_0_555) + (vec_x2 * vec_x2 * vec_0_143);
            var vec_e_recip = Vector<double>.One / vec_e;
            var vec_f_select = Vector.GreaterThan(vec, Vector<double>.Zero);
            var vec_f = Vector.ConditionalSelect(vec_f_select, vec_e_recip, vec_e);
            var vec_result = Vector<double>.One / (Vector<double>.One + vec_f);

            // Store the result in the post-activations span.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<double, byte>(ref vref),
                vec_result);
        }

        // Handle vectors with lengths not an exact multiple of vector width.
        for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBound);
            vref = ref Unsafe.Add(ref vref, 1))
        {
            Fn(ref vref, ref vref);
        }
    }

    /// <inheritdoc/>
    public void Fn(ref double vref, ref double wref, int len)
    {
        // Init constant vectors.
        var vec_4_9 = new Vector<double>(4.9);
        var vec_0_555 = new Vector<double>(0.555);
        var vec_0_143 = new Vector<double>(0.143);

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

            vec *= vec_4_9;
            var vec_x2 = vec * vec;
            var vec_e = Vector<double>.One + Vector.Abs(vec) + (vec_x2 * vec_0_555) + (vec_x2 * vec_x2 * vec_0_143);
            var vec_e_recip = Vector<double>.One / vec_e;
            var vec_f_select = Vector.GreaterThan(vec, Vector<double>.Zero);
            var vec_f = Vector.ConditionalSelect(vec_f_select, vec_e_recip, vec_e);
            var vec_result = Vector<double>.One / (Vector<double>.One + vec_f);

            // Store the result in the post-activations span.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<double, byte>(ref wref),
                vec_result);
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
