// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions.Vectorized;

/// <summary>
/// Leaky rectified linear activation unit (ReLU).
/// </summary>
public sealed class LeakyReLU : IActivationFunction<double>
{
    /// <inheritdoc/>
    public void Fn(ref double x)
    {
        const double a = 0.001;
        if(x < 0.0)
            x *= a;
    }

    /// <inheritdoc/>
    public void Fn(ref double x, ref double y)
    {
        const double a = 0.001;
        y = x;
        if(x < 0.0)
            y *= a;
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
        // Init constant vector.
        var avec = new Vector<double>(0.001);

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

            // Compare each element with zero to build a mask.
            var maskVec = Vector.GreaterThanOrEqual(vec, Vector<double>.Zero);

            // Compute a * vec for the negative case.
            var negativeVec = avec * vec;

            // Select vec if >= 0, or negativeVec if < 0.
            var resultVec = Vector.ConditionalSelect(maskVec, vec, negativeVec);

            // Store the result back to vref.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<double,byte>(ref vref),
                resultVec);
        }

        // Handle vectors with lengths not an exact multiple of vector width.
        for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBound);
            vref = ref Unsafe.Add(ref vref, 1))
        {
            Fn(ref vref);
        }
    }

    /// <inheritdoc/>
    public void Fn(ref double vref, ref double wref, int len)
    {
        // Init constant vector.
        var avec = new Vector<double>(0.001);

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
                ref Unsafe.As<double,byte>(ref vref));

            // Compare each element with zero to build a mask.
            var maskVec = Vector.GreaterThanOrEqual(vec, Vector<double>.Zero);

            // Compute a * vec for the negative case.
            var negativeVec = avec * vec;

            // Select vec if >= 0, or negativeVec if < 0.
            var resultVec = Vector.ConditionalSelect(maskVec, vec, negativeVec);

            // Store the result in the post-activations span.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<double,byte>(ref wref),
                resultVec);
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
