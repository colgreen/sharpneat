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

namespace SharpNeat.Core
{
    /// <summary>
    /// Sort genomes, highest fitness first. Genomes with equal fitness are secondary sorted by age 
    /// (youngest first). Used by the selection routines to select the fittest and youngest genomes.
    /// </summary>
    public class GenomeFitnessComparer<TGenome> : IComparer<TGenome>
        where TGenome : class, IGenome<TGenome>
    {
        /// <summary>
        /// Pre-built comparer.
        /// </summary>
        public static readonly GenomeFitnessComparer<TGenome> Singleton = new GenomeFitnessComparer<TGenome>();

        #region IComparer<TGenome> Members

        /// <summary>
        /// Sort genomes, highest fitness first. Genomes with equal fitness are secondary sorted by age (youngest first).
        /// Used by the selection routines to select the fittest and youngest genomes.
        /// </summary>
        public int Compare(TGenome x, TGenome y)
        {
            // Primary sort - highest fitness first.
            if(x.EvaluationInfo.Fitness > y.EvaluationInfo.Fitness) {
                return -1;
            }
            if(x.EvaluationInfo.Fitness < y.EvaluationInfo.Fitness) {
                return 1;
            }

            // Fitnesses are equal.
            // Secondary sort - youngest first. Younger genomes have a *higher* BirthGeneration.
            if(x.BirthGeneration > y.BirthGeneration) {
                return -1;
            }
            if(x.BirthGeneration < y.BirthGeneration) {
                return 1;
            }

            // Genomes are equal.
            return 0;
        }

        #endregion
    }
}
