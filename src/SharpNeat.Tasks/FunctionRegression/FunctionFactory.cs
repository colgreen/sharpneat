/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
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
            FunctionId.SinXSquared => (x) => (Math.Sin(x * x) * 0.4) + 0.5,
            FunctionId.Waveform1 => (x) => Math.Sin(x + Math.Sin(x)),
            _ => throw new ArgumentException($"Unknown FunctionId type [{fnId}]"),
        };
    }
}
