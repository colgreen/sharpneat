// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions.Vectorized;

/// <summary>
/// Rectified linear activation unit (ReLU).
/// </summary>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public sealed class ReLU<TScalar> : IActivationFunction<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x)
    {
        // Test for specific IEEE754 types and use performant bit tricks where possible.
        // Note. The type checks here are resolved/eliminated at compile time, and therefore do not impact runtime performance.
        if(typeof(TScalar) == typeof(double))
        {
            double xd = double.CreateChecked(x);

            // Calculate the equivalent of:
            //
            //    return x < 0.0 ? 0.0 : x;
            //
            // The approach used here uses bit manipulation of the double precision bits to achieve faster performance. The
            // performance improvement is due to the avoidance of the conditional branch.

            // Get the bits of the double as a signed long (noting that the high bit is the sign bit for both double and
            // long).
            long xlong = Unsafe.As<double,long>(ref xd);

            // Shift xlong right 63 bits. This shifts all of the value bits out of the value; these bits are replaced with
            // the sign bit (which is how shift right works for signed types). Therefore, if xlong was negative then all
            // the bits are set to 1 (including the sign bit), otherwise they are all set to zero.
            // We then take the complement (flip all the bits), and bitwise AND the result with the original value of xlong.
            // This means that we AND xlong with zeros when x is negative, and AND with all ones when the x is positive,
            // thus achieving the ReLU function without using a conditional branch.
            x = TScalar.CreateChecked(BitConverter.Int64BitsToDouble(xlong & ~(xlong >> 63)));
        }
        else if(typeof(TScalar) == typeof(float))
        {
            float xf = float.CreateChecked(x);
            int xint = Unsafe.As<float,int>(ref xf);
            x = TScalar.CreateChecked(BitConverter.Int32BitsToSingle(xint & ~(xint >> 31)));
        }
        else
        {
            if(x < TScalar.Zero)
                x = TScalar.Zero;
        }
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x, ref TScalar y)
    {
        if(typeof(TScalar) == typeof(double))
        {
            double xd = double.CreateChecked(x);
            long xlong = Unsafe.As<double,long>(ref xd);
            y = TScalar.CreateChecked(BitConverter.Int64BitsToDouble(xlong & ~(xlong >> 63)));
        }
        else if(typeof(TScalar) == typeof(float))
        {
            float xf = float.CreateChecked(x);
            int xint = Unsafe.As<float,int>(ref xf);
            y = TScalar.CreateChecked(BitConverter.Int32BitsToSingle(xint & ~(xint >> 31)));
        }
        else
        {
            y = x;
            if(y < TScalar.Zero)
                y = TScalar.Zero;
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
            vec = Vector.Max(vec, Vector<TScalar>.Zero);

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
            vec = Vector.Max(vec, Vector<TScalar>.Zero);

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
