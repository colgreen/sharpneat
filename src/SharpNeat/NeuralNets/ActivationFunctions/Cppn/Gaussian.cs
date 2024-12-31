// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions.Cppn;

/// <summary>
/// Gaussian activation function. Output range is 0 to 1, that is, the tails of the Gaussian
/// distribution curve tend towards 0 as abs(x) -> Infinity and the Gaussian peak is at x = 0.
/// </summary>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public sealed class Gaussian<TScalar> : IActivationFunction<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    static readonly TScalar Two = TScalar.CreateChecked(2.0);
    static readonly TScalar TwoPointFive = TScalar.CreateChecked(2.5);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x)
    {
        x = TScalar.Exp(-TScalar.Pow(x * TwoPointFive, Two));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x, ref TScalar y)
    {
        y = TScalar.Exp(-TScalar.Pow(x * TwoPointFive, Two));
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
