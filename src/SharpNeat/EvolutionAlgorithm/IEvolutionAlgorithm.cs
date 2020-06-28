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

namespace SharpNeat.EvolutionAlgorithm
{
    /// <summary>
    /// Represents a generational evolution algorithm.
    /// </summary>
    public interface IEvolutionAlgorithm
    {
        /// <summary>
        /// Gets the current evolution algorithm statistics.
        /// </summary>
        EvolutionAlgorithmStatistics Stats { get; }

        /// <summary>
        /// Initialise the evolutionary algorithm.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Perform one generation of the evolutionary algorithm.
        /// </summary>
        void PerformOneGeneration();
    }
}
