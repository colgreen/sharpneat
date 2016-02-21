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
using System;

namespace SharpNeat.SpeciationStrategies
{
    internal struct GenomeDistancePair<TGenome> : IComparable<GenomeDistancePair<TGenome>>
    {
        internal double _distance;
        internal TGenome _genome;

        internal GenomeDistancePair(double distance, TGenome genome)
        {
            _distance = distance;
            _genome = genome;
        }

        public int CompareTo(GenomeDistancePair<TGenome> other)
        {
            // Sorts in descending order.
            // Just remember, -1 means we don't change the order of x and y.
            if(_distance > other._distance) {
                return -1;
            }
            if(_distance < other._distance) {
                return 1;
            }
            return 0;
        }
    }
}
