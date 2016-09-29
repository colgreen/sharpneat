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
