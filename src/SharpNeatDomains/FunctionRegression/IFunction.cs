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
    /// A mathematical function that has a single input parameter.
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// Get the function value for the given function argument.
        /// </summary>
        double GetValue(double x);
    }
}
