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
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Neat.ComplexityRegulation
{
    /// <summary>
    /// A complexity regulation strategy that remains fixed in complexifying mode throughout
    /// the lifetime of the evolution algorithm.
    /// </summary>
    public sealed class NullComplexityRegulationStrategy : IComplexityRegulationStrategy
    {
        #region IComplexityRegulationStrategy

        /// <summary>
        /// Gets the current complexity regulation mode.
        /// </summary>
        public ComplexityRegulationMode CurrentMode => ComplexityRegulationMode.Complexifying;

        /// <summary>
        /// Update the complexity regulation mode that the evolution algorithm should be in.
        /// </summary>
        /// <param name="eaStats">Evolution algorithm statistics.</param>
        /// <param name="popStats">Population statistics.</param>
        /// <returns>The updated mode.</returns>
        public ComplexityRegulationMode UpdateMode(EvolutionAlgorithmStatistics eaStats, PopulationStatistics popStats)
        {
            // This is the null strategy, therefore do nothing.
            return ComplexityRegulationMode.Complexifying;
        }

        #endregion
    }
}
