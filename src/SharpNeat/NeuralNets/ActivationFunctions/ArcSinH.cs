// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions;

/// <summary>
/// The ArcSinH function (inverse hyperbolic sine function).
/// </summary>
public sealed class ArcSinH : IActivationFunction<double>
{
    /// <inheritdoc/>
    public void Fn(ref double x)
    {
        // Scaling factor from:
        // https://www.reddit.com/r/MachineLearning/comments/6g5tg1/r_selfnormalizing_neural_networks_improved_elu/diwq7rb/
        x = 1.2567348023993685 * ((Asinh(x) + 1.0) * 0.5);
    }

    /// <inheritdoc/>
    public void Fn(ref double x, ref double y)
    {
        // Scaling factor from:
        // https://www.reddit.com/r/MachineLearning/comments/6g5tg1/r_selfnormalizing_neural_networks_improved_elu/diwq7rb/
        y = 1.2567348023993685 * ((Asinh(x) + 1.0) * 0.5);
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

    #region Private Static Methods

    /// <summary>
    /// Hyperbolic Area Sine.
    /// </summary>
    /// <param name="x">The real value.</param>
    /// <returns>The hyperbolic angle, i.e. the area of its hyperbolic sector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double Asinh(double x)
    {
        return Math.Log(x + Math.Sqrt((x * x) + 1.0), Math.E);
    }

    #endregion
}
