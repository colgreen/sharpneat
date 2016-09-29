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
    /// Represents a complexity regulation strategy. 
    /// 
    /// DetermineMode() is called once per generation. The strategy can determine (and return) which mode
    /// the overall search should be in by examining the stats passed to the method. Thus the simplest 
    /// strategy is to just return ComplexityRegulationMode.Complexifying; this results in no complexity 
    /// regulation.
    /// 
    /// Complexity regulation was known as phased search in SharpNEAT Version 1. For more information see:
    /// Phased Searching with NEAT: Alternating Between Complexification And Simplification, Colin Green, 2004
    /// (http://sharpneat.sourceforge.net/phasedsearch.html)
    /// </summary>
    public interface IComplexityRegulationStrategy
    {
        /// <summary>
        /// Determine which complexity regulation mode the search should be in given the provided
        /// NEAT algorithm stats.
        /// </summary>
        ComplexityRegulationMode DetermineMode(NeatAlgorithmStats stats);
    }
}
