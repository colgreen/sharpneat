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
    /// Statistics resulting from the comparison of two NEAT genomes.
    /// </summary>
    public class CorrelationStatistics
    {
        int _matchingGeneCount;
        int _disjointConnectionGeneCount;
        int _excessConnectionGeneCount;
        double _connectionWeightDelta;

        #region Properties

        /// <summary>
        /// Gets or sets the number of matching connection genes between the two comparison genomes.
        /// </summary>
        public int MatchingGeneCount
        {
            get { return _matchingGeneCount; }
            set { _matchingGeneCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of disjoint connection genes between the two comparison genomes.
        /// </summary>
        public int DisjointConnectionGeneCount
        {
            get { return _disjointConnectionGeneCount; }
            set { _disjointConnectionGeneCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of excess connection genes between the two comparison genomes.
        /// </summary>
        public int ExcessConnectionGeneCount
        {
            get { return _excessConnectionGeneCount; }
            set { _excessConnectionGeneCount = value; }
        }

        /// <summary>
        /// Gets or sets the cumulative total of absolute weight differences between all of the connection genes that matched up.
        /// </summary>
        public double ConnectionWeightDelta
        {
            get { return _connectionWeightDelta; }
            set { _connectionWeightDelta = value; }
        }

        #endregion
    }
}
