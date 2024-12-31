// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions;

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
        TScalar x2 = y * y;
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
        // Calc span bounds.
        ref TScalar vrefBound = ref Unsafe.Add(ref vref, len);

        // Loop over span elements, invoking the scalar activation fn for each.
        for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBound);
            vref = ref Unsafe.Add(ref vref, 1))
        {
            Fn(ref vref);
        }
    }

    /// <inheritdoc/>
    public void Fn(ref TScalar vref, ref TScalar wref, int len)
    {
        // Calc span bounds.
        ref TScalar vrefBound = ref Unsafe.Add(ref vref, len);

        // Loop over span elements, invoking the scalar activation fn for each.
        for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBound);
            vref = ref Unsafe.Add(ref vref, 1),
            wref = ref Unsafe.Add(ref wref, 1))
        {
            Fn(ref vref, ref wref);
        }
    }
}
