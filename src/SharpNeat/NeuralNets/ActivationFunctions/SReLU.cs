// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions;

/// <summary>
/// S-shaped rectified linear activation unit (SReLU).
/// From:
///    https://en.wikipedia.org/wiki/Activation_function
///    https://arxiv.org/abs/1512.07030 [Deep Learning with S-shaped Rectified Linear Activation Units].
/// </summary>
public sealed class SReLU : IActivationFunction<double>
{
    /// <inheritdoc/>
    public void Fn(ref double x)
    {
        const double tl = 0.001; // threshold (left).
        const double tr = 0.999; // threshold (right).
        const double a = 0.00001;

        if(x > tl && x < tr)
        {
            return;
        }
        else if(x <= tl)
        {
            x = tl + ((x - tl) * a);
        }
        else
        {
            x = tr + ((x - tr) * a);
        }
    }

    /// <inheritdoc/>
    public void Fn(ref double x, ref double y)
    {
        const double tl = 0.001; // threshold (left).
        const double tr = 0.999; // threshold (right).
        const double a = 0.00001;

        y = x;

        if(y > tl && y < tr)
        {
            return;
        }
        else if(y <= tl)
        {
            y = tl + ((y - tl) * a);
        }
        else
        {
            y = tr + ((y - tr) * a);
        }
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
