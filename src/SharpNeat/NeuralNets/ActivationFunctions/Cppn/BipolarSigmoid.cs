// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.Double.ActivationFunctions.Cppn;

/// <summary>
/// Bipolar sigmoid activation function. Output range is -1 to 1 instead of the more normal 0 to 1.
/// </summary>
public sealed class BipolarSigmoid : IActivationFunction<double>
{
    /// <inheritdoc/>
    public void Fn(ref double x)
    {
        x = (2.0 / (1.0 + Math.Exp(-4.9 * x))) - 1.0;
    }

    /// <inheritdoc/>
    public void Fn(ref double x, ref double y)
    {
        y = (2.0 / (1.0 + Math.Exp(-4.9 * x))) - 1.0;
    }

    /// <inheritdoc/>
    public void Fn(Span<double> v)
    {
        // Naive implementation.
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
