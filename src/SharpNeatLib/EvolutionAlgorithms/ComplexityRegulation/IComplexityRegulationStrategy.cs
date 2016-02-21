/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
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
