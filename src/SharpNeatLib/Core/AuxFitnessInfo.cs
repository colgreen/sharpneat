/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2011 Colin Green (sharpneat@gmail.com)
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
namespace SharpNeat.Core
{
    /// <summary>
    /// Auxiliary fitness info, i.e. for evaluation metrics other than the
    /// primary fitness metric but that nonetheless we are interested in observing.
    /// </summary>
    public struct AuxFitnessInfo
    {
        /// <summary>
        /// Auxiliary metric name.
        /// </summary>
        public string _name;
        /// <summary>
        /// Auxiliary metric value.
        /// </summary>
        public double _value;

        /// <summary>
        /// Construct with the provided name-value pair.
        /// </summary>
        public AuxFitnessInfo(string name, double value)
        {
            _name = name;
            _value = value;
        }
    }
}
