// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNeat.NeuralNets.ActivationFunctions.Vectorized;

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
        // Init constants.
        var vec_tl = new Vector<double>(0.001);
        var vec_tr = new Vector<double>(0.999);
        var vec_a = new Vector<double>(0.00001);

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

            // Calc values in left hand segment of y=f(x); i.e. x <= tl.
            var vec_left = vec - vec_tl;
            vec_left *= vec_a;
            vec_left += vec_tl;

            // Calc values in right hand segment of y=f(x); i.e. x >= tr.
            var vec_right = vec - vec_tr;
            vec_right *= vec_a;
            vec_right += vec_tr;

            // For each vector element select a value from the correct segment, i.e. vec, vec_left or vec_right.
            var vec_select_left = Vector.LessThan(vec_left, vec_tl);
            var vec_select_right = Vector.GreaterThan(vec_right, vec_tr);

            vec = Vector.ConditionalSelect(vec_select_right, vec_right, vec);
            vec = Vector.ConditionalSelect(vec_select_left, vec_left, vec);

            // Store the result in the post-activations span.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<double, byte>(ref vref),
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
    public void Fn(ref double vref, ref double wref, int len)
    {
        // Init constants.
        var vec_tl = new Vector<double>(0.001);
        var vec_tr = new Vector<double>(0.999);
        var vec_a = new Vector<double>(0.00001);

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
                ref Unsafe.As<double, byte>(ref vref));

            // Calc values in left hand segment of y=f(x); i.e. x <= tl.
            var vec_left = vec - vec_tl;
            vec_left *= vec_a;
            vec_left += vec_tl;

            // Calc values in right hand segment of y=f(x); i.e. x >= tr.
            var vec_right = vec - vec_tr;
            vec_right *= vec_a;
            vec_right += vec_tr;

            // For each vector element select a value from the correct segment, i.e. vec, vec_left or vec_right.
            var vec_select_left = Vector.LessThan(vec_left, vec_tl);
            var vec_select_right = Vector.GreaterThan(vec_right, vec_tr);

            vec = Vector.ConditionalSelect(vec_select_right, vec_right, vec);
            vec = Vector.ConditionalSelect(vec_select_left, vec_left, vec);

            // Store the result in the post-activations span.
            Unsafe.WriteUnaligned(
                ref Unsafe.As<double, byte>(ref wref),
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
