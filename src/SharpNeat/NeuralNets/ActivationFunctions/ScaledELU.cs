// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions;

/// <summary>
/// Scaled Exponential Linear Unit (SELU).
///
/// From:
///     Self-Normalizing Neural Networks
///     https://arxiv.org/abs/1706.02515
///
/// Original source code (including parameter values):
///     <see href="https://github.com/bioinf-jku/SNNs/blob/master/selu.py"/>.
///
/// </summary>
public sealed class ScaledELU : IActivationFunction<double>
{
    /// <inheritdoc/>
    public void Fn(ref double x)
    {
        const double alpha = 1.6732632423543772848170429916717;
        const double scale = 1.0507009873554804934193349852946;

        if(x >= 0)
        {
            x = scale * x;
        }
        else
        {
            x = scale * ((alpha * Math.Exp(x)) - alpha);
        }
    }

    /// <inheritdoc/>
    public void Fn(ref double x, ref double y)
    {
        const double alpha = 1.6732632423543772848170429916717;
        const double scale = 1.0507009873554804934193349852946;

        if(x >= 0)
        {
            y = scale * x;
        }
        else
        {
            y = scale * ((alpha * Math.Exp(x)) - alpha);
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
