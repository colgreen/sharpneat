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

namespace SharpNeat.EvolutionAlgorithms.ComplexityRegulation
{
    /// <summary>
    /// Null strategy. Fixed to Complexifying mode.
    /// </summary>
    public class NullComplexityRegulationStrategy : IComplexityRegulationStrategy
    {
        /// <summary>
        /// Determine which complexity regulation mode the search should be in given the provided
        /// NEAT algorithm stats.
        /// </summary>
        public ComplexityRegulationMode DetermineMode(NeatAlgorithmStats stats)
        {
            return ComplexityRegulationMode.Complexifying;
        }
    }
}
