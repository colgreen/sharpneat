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

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// An enumeration of connection gene correlation types.
    /// Mismatched genes can be disjoint or excess. This distinction is defined by original
    /// NEAT but is probably redundant for most genome comparison/distance metrics.
    /// </summary>
    public enum CorrelationItemType
    {
        /// <summary>
        /// A match between two connections in two distinct genomes.
        /// </summary>
        Match,
        /// <summary>
        /// A connection with no match in the other genome (that we are comparing with) and that has 
        /// an innovation ID less than the highest innovation ID in the other genome,
        /// </summary>
        Disjoint,
        /// <summary>
        /// A connection with no match in the other genome (that we are comparing with) and that has 
        /// an innovation ID higher than the highest innovation ID in the other genome.
        /// </summary>
        Excess
    }
}
