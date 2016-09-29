/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

namespace SharpNeat.Domains.FunctionRegression
{
    /// <summary>
    /// Represents a mathematical function with a 1 or more real valued inputs and a single output. 
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// Get the funection value for th specified function arguments.
        /// </summary>
        double GetValue(double[] args);
        /// <summary>
        /// Gets the number of inputs the function expects.
        /// </summary>
        int InputCount { get; }
    }
}
