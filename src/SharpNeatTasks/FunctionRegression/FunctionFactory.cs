/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;

namespace SharpNeat.Tasks.FunctionRegression
{
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
            switch(fnId)
            {
                case FunctionId.Abs:
                    return (x) => Math.Abs(x);

                case FunctionId.Log:
                    return (x) => Math.Log(x);

                case FunctionId.Sin:
                    return (x) => Math.Sin(x);              

                case FunctionId.SinXSquared:
                    return (x) => (Math.Sin(x * x) * 0.4) + 0.5;

                case FunctionId.Waveform1:
                    return (x) => Math.Sin(x + Math.Sin(x));
            }
            throw new ArgumentException($"Unknown FunctionId type [{fnId}]");
        }
    }
}
