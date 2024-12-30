// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace SharpNeat.Tasks.FunctionRegression;

/// <summary>
/// Static factory methods for creating functions for the function regression task.
/// </summary>
public static class FunctionFactory
{
    /// <summary>
    /// Get an instance of <see cref="Func{T, TResult}"/> for the specified function type.
    /// </summary>
    /// <typeparam name="TScalar">Function data type.</typeparam>
    /// <param name="fnId">Function ID.</param>
    /// <returns>An instance of <see cref="Func{T, TResult}"/>.</returns>
    public static Func<TScalar, TScalar> GetFunction<TScalar>(FunctionId fnId)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        return fnId switch
        {
            FunctionId.Abs => (x) => TScalar.Abs(x),
            FunctionId.Log => (x) => TScalar.Log(x),
            FunctionId.Sin => (x) => TScalar.Sin(x),
            FunctionId.BeatSinewave => (x) => TScalar.Sin(x) + TScalar.Sin(x * TScalar.CreateChecked(1.2)),
            FunctionId.SinXSquared => (x) => (TScalar.Sin(x * x) * TScalar.CreateChecked(0.4)) + TScalar.CreateChecked(0.5),
            FunctionId.Waveform1 => (x) => TScalar.Sin(x + TScalar.Sin(x)),
            _ => throw new ArgumentException($"Unknown FunctionId type [{fnId}]"),
        };
    }
}
