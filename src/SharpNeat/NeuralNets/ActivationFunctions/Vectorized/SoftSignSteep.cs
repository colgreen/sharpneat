// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions.Vectorized;

/// <summary>
/// The softsign sigmoid.
/// This is a variant of softsign that has a steeper slope at and around the origin that
/// is intended to be a similar slope to that of LogisticFunctionSteep.
/// </summary>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public sealed class SoftSignSteep<TScalar> : IActivationFunction<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    static readonly TScalar Half = TScalar.CreateChecked(0.5);
    static readonly TScalar Two = TScalar.CreateChecked(2.0);
    static readonly TScalar PointTwo = TScalar.CreateChecked(0.2);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x)
    {
        x = Half + (x / (Two * (PointTwo + TScalar.Abs(x))));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x, ref TScalar y)
    {
        y = Half + (x / (Two * (PointTwo + TScalar.Abs(x))));
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
        // Init constants.
        var vecTwo = new Vector<TScalar>(Two);
        var vecFifth = new Vector<TScalar>(PointTwo);
        var vecHalf = new Vector<TScalar>(Half);

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

            // Calc the softsign sigmoid.
            vec = vecHalf + (vec / (vecTwo * (vecFifth + Vector.Abs(vec))));

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
        // Init constants.
        var vecTwo = new Vector<TScalar>(Two);
        var vecFifth = new Vector<TScalar>(PointTwo);
        var vecHalf = new Vector<TScalar>(Half);

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

            // Calc the softsign sigmoid.
            vec = vecHalf + (vec / (vecTwo * (vecFifth + Vector.Abs(vec))));

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
