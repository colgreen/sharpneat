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
    /// Complexity regulation modes.
    /// 
    /// Represents two variations in the overall search strategy - complexifying and simplifying.
    /// That is, allowing genomes to complexify and reducing their complexity to trim away excess 
    /// and/or redundant structure in the population to reinvigorate a search.
    ///
    /// For more information see:
    /// Phased Searching with NEAT: Alternating Between Complexification And Simplification, Colin Green, 2004
    /// (http://sharpneat.sourceforge.net/phasedsearch.html)
    /// </summary>
    public enum ComplexityRegulationMode
    {
        /// <summary>
        /// Search by allowing genomes to complexify (add new structure).
        /// </summary>
        Complexifying = 0,
        /// <summary>
        /// Search by simplifying genomes (removing structure).
        /// </summary>
        Simplifying = 1
    }
}
