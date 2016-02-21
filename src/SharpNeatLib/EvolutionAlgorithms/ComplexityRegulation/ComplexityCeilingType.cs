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
    /// Complexity ceiling types.
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
