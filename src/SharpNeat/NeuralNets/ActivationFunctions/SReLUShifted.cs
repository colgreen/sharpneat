// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions;

#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter

/// <summary>
/// S-shaped rectified linear activation unit (SReLU).
/// Shifted on the x-axis so that x=0 gives y=0.5, in keeping with the logistic sigmoid.
/// From:
///    https://en.wikipedia.org/wiki/Activation_function
///    https://arxiv.org/abs/1512.07030 [Deep Learning with S-shaped Rectified Linear Activation Units].
/// </summary>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public sealed class SReLUShifted<TScalar> : IActivationFunction<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    static readonly TScalar tl = TScalar.CreateChecked(0.001); // threshold (left).
    static readonly TScalar tr = TScalar.CreateChecked(0.999); // threshold (right).
    static readonly TScalar a = TScalar.CreateChecked(0.00001);
    static readonly TScalar offset = TScalar.CreateChecked(0.5);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x)
    {
        if(x + offset > tl && x + offset < tr)
        {
            x += offset;
        }
        else if(x + offset <= tl)
        {
            x = tl + ((x + offset - tl) * a);
        }
        else
        {
            x = tr + ((x + offset - tr) * a);
        }
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x, ref TScalar y)
    {
        if(x + offset > tl && x + offset < tr)
        {
            y = x + offset;
        }
        else if(x + offset <= tl)
        {
            y = tl + ((x + offset - tl) * a);
        }
        else
        {
            y = tr + ((x + offset - tr) * a);
        }
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
