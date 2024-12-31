// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions.Vectorized;

/// <summary>
/// max(-1, x,) function.
/// </summary>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public sealed class MaxMinusOne<TScalar> : IActivationFunction<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x)
    {
        if(x < TScalar.NegativeOne)
            x = TScalar.NegativeOne;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x, ref TScalar y)
    {
        y = x;

        if(y < TScalar.NegativeOne)
            y = TScalar.NegativeOne;
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
        // Init constant vector.
        var minusOneVec = new Vector<TScalar>(TScalar.NegativeOne);

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

            // Apply max(val, 0) to each element in the vector.
            vec = Vector.Max(vec, minusOneVec);

            // Store the result in the post-activations span.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<TScalar, byte>(ref vref),
                vec);
        }

        // Handle vectors with lengths not an exact multiple of vector width.
        for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBound);
            vref = ref Unsafe.Add(ref vref, 1))
        {
            Fn(ref vref);
        }
    }

    /// <inheritdoc/>
    public void Fn(ref TScalar vref, ref TScalar wref, int len)
    {
        // Init constant vector.
        var minusOneVec = new Vector<TScalar>(TScalar.NegativeOne);

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

            // Apply max(val, 0) to each element in the vector.
            vec = Vector.Max(vec, minusOneVec);

            // Store the result in the post-activations span.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<TScalar, byte>(ref wref),
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
