// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions;

/// <summary>
/// The ArcSinH function (inverse hyperbolic sine function).
/// </summary>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public sealed class ArcSinH<TScalar> : IActivationFunction<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    static readonly TScalar K = TScalar.CreateChecked(1.2567348023993685);
    static readonly TScalar Half = TScalar.CreateChecked(0.5);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x)
    {
        // Scaling factor from:
        // https://www.reddit.com/r/MachineLearning/comments/6g5tg1/r_selfnormalizing_neural_networks_improved_elu/diwq7rb/
        x = K * ((Asinh(x) + TScalar.One) * Half);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x, ref TScalar y)
    {
        // Scaling factor from:
        // https://www.reddit.com/r/MachineLearning/comments/6g5tg1/r_selfnormalizing_neural_networks_improved_elu/diwq7rb/
        y = K * ((Asinh(x) + TScalar.One) * Half);
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

    /// <summary>
    /// Hyperbolic Area Sine.
    /// </summary>
    /// <param name="x">The real value.</param>
    /// <returns>The hyperbolic angle, i.e. the area of its hyperbolic sector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TScalar Asinh(TScalar x)
    {
        // Notes.
        // This formula is actually the definition of arcsinh, i.e., this is exact.
        // In benchmarks this was about twice as fast as calling TScalar.Asinh() for double precision floats on a
        // Ryzen 7 PRO 5750GE; behaviour may be different on other CPUs and for other data types.
        return TScalar.Log(x + TScalar.Sqrt((x * x) + TScalar.One));
    }
}
