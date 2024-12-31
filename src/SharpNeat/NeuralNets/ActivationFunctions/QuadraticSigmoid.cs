// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions;

#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter

/// <summary>
/// A sigmoid formed by two sub-sections of the y=x^2 curve.
///
/// The extremes are implemented as per the leaky ReLU, i.e. there is a linear slop to
/// ensure there is at least a gradient to follow at the extremes.
/// </summary>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public sealed class QuadraticSigmoid<TScalar> : IActivationFunction<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    static readonly TScalar Half = TScalar.CreateChecked(0.5);
    static readonly TScalar t = TScalar.CreateChecked(0.999);
    static readonly TScalar a = TScalar.CreateChecked(0.00001);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x)
    {
        // Calc abs(x) and sign(x) with just a single conditional branch
        // (calling those functions individually results in two conditional branches).
        TScalar sign = TScalar.One;
        TScalar y = x;

        if(y < TScalar.Zero)
        {
            y *= TScalar.NegativeOne;
            sign = TScalar.NegativeOne;
        }

        if(y < t)
        {
            y = t - ((y - t) * (y - t));
        }
        else // if (x >= t)
        {
            y = t + ((y - t) * a);
        }

        x = (y * sign * Half) + Half;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x, ref TScalar y)
    {
        // Calc abs(x) and sign(x) with just a single conditional branch
        // (calling those functions individually results in two conditional branches).
        TScalar sign = TScalar.One;
        y = x;

        if(y < TScalar.Zero)
        {
            y *= TScalar.NegativeOne;
            sign = TScalar.NegativeOne;
        }

        if(y < t)
        {
            y = t - ((y - t) * (y - t));
        }
        else // if (x >= t)
        {
            y = t + ((y - t) * a);
        }

        y = (y * sign * Half) + Half;
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
