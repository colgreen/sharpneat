// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Tasks.FunctionRegression;

/// <summary>
/// Static factory methods for creating functions for the function regression task.
/// </summary>
public static class FunctionFactory
{
    /// <summary>
    /// Get an instance of <see cref="Func{T, TResult}"/> for the specified function type.
    /// </summary>
    /// <param name="fnId">Function ID.</param>
    /// <returns>An instance of <see cref="Func{T, TResult}"/>.</returns>
    public static Func<double,double> GetFunction(FunctionId fnId)
    {
        return fnId switch
        {
            FunctionId.Abs => (x) => Math.Abs(x),
            FunctionId.Log => (x) => Math.Log(x),
            FunctionId.Sin => (x) => Math.Sin(x),
            FunctionId.BeatSinewave => (x) => Math.Sin(x) + Math.Sin(x * 1.2),
            FunctionId.SinXSquared => (x) => (Math.Sin(x * x) * 0.4) + 0.5,
            FunctionId.Waveform1 => (x) => Math.Sin(x + Math.Sin(x)),
            _ => throw new ArgumentException($"Unknown FunctionId type [{fnId}]"),
        };
    }
}
