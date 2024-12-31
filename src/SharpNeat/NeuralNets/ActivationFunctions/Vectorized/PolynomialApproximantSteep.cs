// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions.Vectorized;

#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter

/// <summary>
/// A very close approximation of the logistic function that avoids use of exp() and is therefore
/// typically much faster to compute, while giving an almost identical sigmoid curve.
///
/// This function was obtained from:
///    http://stackoverflow.com/a/34448562/15703
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
/// </summary>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public sealed class PolynomialApproximantSteep<TScalar> : IActivationFunction<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    static readonly TScalar a = TScalar.CreateChecked(4.9);
    static readonly TScalar b = TScalar.CreateChecked(0.555);
    static readonly TScalar c = TScalar.CreateChecked(0.143);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x)
    {
        x *= a;
        TScalar x2 = x * x;
        TScalar e = TScalar.One + TScalar.Abs(x) + (x2 * b) + (x2 * x2 * c);

        TScalar f = (x > TScalar.Zero) ? (TScalar.One / e) : e;
        x = TScalar.One / (TScalar.One + f);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x, ref TScalar y)
    {
        y = x * a;
        TScalar x2 = x * x;
        TScalar e = TScalar.One + TScalar.Abs(y) + (x2 * b) + (x2 * x2 * c);

        TScalar f = (x > TScalar.Zero) ? (TScalar.One / e) : e;
        y = TScalar.One / (TScalar.One + f);
    }

    /// <inheritdoc/>
    public void Fn(Span<TScalar> v)
    {
        Fn(ref MemoryMarshal.GetReference(v), v.Length);
    }

    /// <inheritdoc/>
    public void Fn(ReadOnlySpan<TScalar> v, Span<TScalar> w)
    {
        // Obtain refs to the spans, and call on to the unsafe ref based overload.
        Fn(
            ref MemoryMarshal.GetReference(v),
            ref MemoryMarshal.GetReference(w),
            v.Length);
    }

    /// <inheritdoc/>
    public void Fn(ref TScalar vref, int len)
    {
        // Init constant vectors.
        var veca = new Vector<TScalar>(a);
        var vecb = new Vector<TScalar>(b);
        var vecc = new Vector<TScalar>(c);

        // Calc span bounds.
        ref TScalar vrefBound = ref Unsafe.Add(ref vref, len);
        ref TScalar vrefBoundVec = ref Unsafe.Subtract(ref vrefBound, Vector<TScalar>.Count - 1);

        // Loop SIMD vector sized segments.
        for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBoundVec);
            vref = ref Unsafe.Add(ref vref, Vector<TScalar>.Count))
        {
            // Load values into a vector.
            // The odd code pattern is taken from the Vector<T> constructor's source code.
            var vec = Unsafe.ReadUnaligned<Vector<TScalar>>(
                ref Unsafe.As<TScalar, byte>(ref vref));

            vec *= veca;
            var vec_x2 = vec * vec;
            var vec_e = Vector<TScalar>.One + Vector.Abs(vec) + (vec_x2 * vecb) + (vec_x2 * vec_x2 * vecc);
            var vec_e_recip = Vector<TScalar>.One / vec_e;
            var vec_f_select = Vector.GreaterThan(vec, Vector<TScalar>.Zero);
            var vec_f = Vector.ConditionalSelect(vec_f_select, vec_e_recip, vec_e);
            var vec_result = Vector<TScalar>.One / (Vector<TScalar>.One + vec_f);

            // Store the result in the post-activations span.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<TScalar, byte>(ref vref),
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
    public void Fn(ref TScalar vref, ref TScalar wref, int len)
    {
        // Init constant vectors.
        var veca = new Vector<TScalar>(a);
        var vecb = new Vector<TScalar>(b);
        var vecc = new Vector<TScalar>(c);

        // Calc span bounds.
        ref TScalar vrefBound = ref Unsafe.Add(ref vref, len);
        ref TScalar vrefBoundVec = ref Unsafe.Subtract(ref vrefBound, Vector<TScalar>.Count - 1);

        // Loop SIMD vector sized segments.
        for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBoundVec);
            vref = ref Unsafe.Add(ref vref, Vector<TScalar>.Count),
            wref = ref Unsafe.Add(ref wref, Vector<TScalar>.Count))
        {
            // Load values into a vector.
            // The odd code pattern is taken from the Vector<T> constructor's source code.
            var vec = Unsafe.ReadUnaligned<Vector<TScalar>>(
                ref Unsafe.As<TScalar, byte>(ref vref));

            vec *= veca;
            var vec_x2 = vec * vec;
            var vec_e = Vector<TScalar>.One + Vector.Abs(vec) + (vec_x2 * vecb) + (vec_x2 * vec_x2 * vecc);
            var vec_e_recip = Vector<TScalar>.One / vec_e;
            var vec_f_select = Vector.GreaterThan(vec, Vector<TScalar>.Zero);
            var vec_f = Vector.ConditionalSelect(vec_f_select, vec_e_recip, vec_e);
            var vec_result = Vector<TScalar>.One / (Vector<TScalar>.One + vec_f);

            // Store the result in the post-activations span.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<TScalar, byte>(ref wref),
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
