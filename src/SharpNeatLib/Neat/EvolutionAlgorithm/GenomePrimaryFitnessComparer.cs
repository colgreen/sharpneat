/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Collections.Generic;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.EvolutionAlgorithm
{
    /// <summary>
    /// For sorting genomes by primary fitness, highest to lowest fitness.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public class GenomePrimaryFitnessComparer<T> : IComparer<NeatGenome<T>>
        where T : struct
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static readonly GenomePrimaryFitnessComparer<T> Singleton = new GenomePrimaryFitnessComparer<T>();

        /// <summary>
        /// Compares two genomes and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first genome to compare.</param>
        /// <param name="y">The second genome to compare.</param>
        /// <returns>A signed integer that indicates the relative sort order of the two genomes.</returns>
        public int Compare(NeatGenome<T> x, NeatGenome<T> y)
        {
            // Primary sort - highest fitness first.
            if(x.FitnessInfo.PrimaryFitness > y.FitnessInfo.PrimaryFitness) {
                return -1;
            }
            if(x.FitnessInfo.PrimaryFitness < y.FitnessInfo.PrimaryFitness) {
                return 1;
            }

            // Genomes are equal.
            return 0;
        }
    }
}
