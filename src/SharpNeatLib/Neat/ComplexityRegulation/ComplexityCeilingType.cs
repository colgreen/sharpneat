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

namespace SharpNeat.Neat.ComplexityRegulation
{
    /// <summary>
    /// Complexity regulation ceiling type.
    /// </summary>
    public enum ComplexityCeilingType
    {
        /// <summary>
        /// Defines an absolute ceiling on complexity.
        /// </summary>
        Absolute,
        /// <summary>
        /// Defines a relative ceiling on complexity. E.g. relative to the complexity
        /// at the end of the most recent simplification phase.
        /// </summary>
        Relative
    }
}
