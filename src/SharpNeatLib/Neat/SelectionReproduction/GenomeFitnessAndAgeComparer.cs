using System;
using System.Collections.Generic;
using System.Text;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.SelectionReproduction
{
    /// <summary>
    /// Sort genomes, highest fitness first, then secondary sorted by age (youngest first).
    /// This sort order is used by the selection routines to select the fittest and youngest genomes.
    /// </summary>
    public class GenomeFitnessAndAgeComparer<T> : IComparer<NeatGenome<T>>
        where T : struct
    {
        public static readonly GenomeFitnessAndAgeComparer<T> Singleton = new GenomeFitnessAndAgeComparer<T>();

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
    }
}
