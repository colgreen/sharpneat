// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter

namespace SharpNeat.NeuralNets.ActivationFunctions.Vectorized;

/// <summary>
/// A sigmoid formed by two sub-sections of the y=x^2 curve.
///
/// The extremes are implemented as per the leaky ReLU, i.e. there is a linear slop to
/// ensure there is at least a gradient to follow at the extremes.
/// </summary>
/// <typeparam name="TScalar">Activation function data type.</typeparam>
public sealed class QuadraticSigmoid<TScalar> : IActivationFunction<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    static readonly TScalar Half = TScalar.CreateChecked(0.5);
    static readonly TScalar t = TScalar.CreateChecked(0.999);
    static readonly TScalar a = TScalar.CreateChecked(0.00001);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x)
    {
        // Calc abs(x) and sign(x) with just a single conditional branch
        // (calling those functions individually results in two conditional branches).
        TScalar sign = TScalar.One;
        TScalar y = x;

        if(y < TScalar.Zero)
        {
            y *= TScalar.NegativeOne;
            sign = TScalar.NegativeOne;
        }

        if(y < t)
        {
            y = t - ((y - t) * (y - t));
        }
        else // if (x >= t)
        {
            y = t + ((y - t) * a);
        }

        x = (y * sign * Half) + Half;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fn(ref TScalar x, ref TScalar y)
    {
        // Calc abs(x) and sign(x) with just a single conditional branch
        // (calling those functions individually results in two conditional branches).
        TScalar sign = TScalar.One;
        y = x;

        if(y < TScalar.Zero)
        {
            y *= TScalar.NegativeOne;
            sign = TScalar.NegativeOne;
        }

        if(y < t)
        {
            y = t - ((y - t) * (y - t));
        }
        else // if (x >= t)
        {
            y = t + ((y - t) * a);
        }

        y = (y * sign * Half) + Half;
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
        var vec_t = new Vector<TScalar>(t);
        var vec_a = new Vector<TScalar>(a);
        var vec_half = new Vector<TScalar>(Half);

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

            // Determine the absolute value of each element.
            var vec_abs = Vector.Abs(vec);

            // Determine the sign of each element (true indicates a non-negative value).
            var vec_sign_flag = Vector.Equals(vec, vec_abs);
            var vec_sign = Vector.ConditionalSelect(vec_sign_flag, Vector<TScalar>.One, new Vector<TScalar>(TScalar.NegativeOne));

            // Handle abs values in the interval [0,t)
            var vec_x_minus_t = vec_abs - vec_t;
            var vec_inner = vec_t - (vec_x_minus_t * vec_x_minus_t);

            // Handle abs values outside of the interval [0,t).
            var vec_outer = vec_t + (vec_x_minus_t * vec_a);

            // Select a value from vec_inner or vec_outer.
            var vec_select_inner = Vector.LessThan(vec_abs, vec_t);
            var vec_y = Vector.ConditionalSelect(vec_select_inner, vec_inner, vec_outer);

            // Apply final calc stage.
            vec_y = (vec_y * vec_sign * vec_half) + vec_half;

            // Store the result in the post-activations span.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<TScalar, byte>(ref vref),
                vec_y);
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
        var vec_t = new Vector<TScalar>(t);
        var vec_a = new Vector<TScalar>(a);
        var vec_half = new Vector<TScalar>(Half);

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

            // Determine the absolute value of each element.
            var vec_abs = Vector.Abs(vec);

            // Determine the sign of each element (true indicates a non-negative value).
            var vec_sign_flag = Vector.Equals(vec, vec_abs);
            var vec_sign = Vector.ConditionalSelect(vec_sign_flag, Vector<TScalar>.One, new Vector<TScalar>(TScalar.NegativeOne));

            // Handle abs values in the interval [0,t)
            var vec_x_minus_t = vec_abs - vec_t;
            var vec_inner = vec_t - (vec_x_minus_t * vec_x_minus_t);

            // Handle abs values outside of the interval [0,t).
            var vec_outer = vec_t + (vec_x_minus_t * vec_a);

            // Select a value from vec_inner or vec_outer.
            var vec_select_inner = Vector.LessThan(vec_abs, vec_t);
            var vec_y = Vector.ConditionalSelect(vec_select_inner, vec_inner, vec_outer);

            // Apply final calc stage.
            vec_y = (vec_y * vec_sign * vec_half) + vec_half;

            // Store the result in the post-activations span.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<TScalar, byte>(ref wref),
                vec_y);
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
