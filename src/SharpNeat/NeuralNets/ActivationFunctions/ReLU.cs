// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions;

/// <summary>
/// Rectified linear activation unit (ReLU).
/// </summary>
public sealed class ReLU : IActivationFunction<double>
{
    /// <inheritdoc/>
    public void Fn(ref double x)
    {
        // Calculate the equivalent of:
        //
        //    return x < 0.0 ? 0.0 : x;
        //
        // The approach used here uses bit manipulation of the double precision bits to achieve faster performance. The
        // performance improvement is due to the avoidance of the conditional branch.

        // Get the bits of the double as a signed long (noting that the high bit is the sign bit for both double and
        // long).
        long xlong = Unsafe.As<double,long>(ref x);

        // Shift xlong right 63 bits. This shifts all of the value bits out of the value; these bits are replaced with
        // the sign bit (which is how shift right works for signed types). Therefore, if xlong was negative then all
        // the bits are set to 1 (including the sign bit), otherwise they are all set to zero.
        // We then take the complement (flip all the bits), and bitwise AND the result with the original value of xlong.
        // This means that we AND xlong with zeros when x is negative, and AND with all ones when the x is positive,
        // thus achieving the ReLU function without using a conditional branch.
        x = BitConverter.Int64BitsToDouble(xlong & ~(xlong >> 63));
    }

    /// <inheritdoc/>
    public void Fn(ref double x, ref double y)
    {
        long xlong = Unsafe.As<double,long>(ref x);
        y = BitConverter.Int64BitsToDouble(xlong & ~(xlong >> 63));
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
        // Calc span bounds.
        ref double vrefBound = ref Unsafe.Add(ref vref, len);

        // Loop over span elements, invoking the scalar activation fn for each.
        for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBound);
            vref = ref Unsafe.Add(ref vref, 1))
        {
            Fn(ref vref);
        }
    }

    /// <inheritdoc/>
    public void Fn(ref double vref, ref double wref, int len)
    {
        // Calc span bounds.
        ref double vrefBound = ref Unsafe.Add(ref vref, len);

        // Loop over span elements, invoking the scalar activation fn for each.
        for(; Unsafe.IsAddressLessThan(ref vref, ref vrefBound);
            vref = ref Unsafe.Add(ref vref, 1),
            wref = ref Unsafe.Add(ref wref, 1))
        {
            Fn(ref vref, ref wref);
        }
    }
}
