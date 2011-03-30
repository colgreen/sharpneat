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
using System.Collections.Generic;
using SharpNeat.Core;

namespace SharpNeat.SpeciationStrategies
{
    /// <summary>
    /// Static helper methods for speciation.
    /// </summary>
    public static class SpeciationUtils
    {
        /// <summary>
        /// Returns true if all of the species are empty.
        /// </summary>
        public static bool TestEmptySpecies<TGenome>(IList<Specie<TGenome>> specieList)
             where TGenome : class, IGenome<TGenome>
        {
            foreach(Specie<TGenome> specie in specieList) 
            {
                if(specie.GenomeList.Count != 0) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if all species contain at least 1 genome.
        /// </summary>
        public static bool TestPopulatedSpecies<TGenome>(IList<Specie<TGenome>> specieList)
            where TGenome : class, IGenome<TGenome>
        {
            foreach(Specie<TGenome> specie in specieList)
            {
                if(specie.GenomeList.Count == 0) {
                    return false;
                }
            }
            return true;
        }
    }
}
