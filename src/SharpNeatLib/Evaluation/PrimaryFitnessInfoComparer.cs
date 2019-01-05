/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

using System.Collections.Generic;

namespace SharpNeat.Evaluation
{
    /// <summary>
    /// For comparing the primary fitness of two <see cref="FitnessInfo"/> instances.
    /// </summary>
    public class PrimaryFitnessInfoComparer : IComparer<FitnessInfo>
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static PrimaryFitnessInfoComparer Singleton = new PrimaryFitnessInfoComparer();

        /// <summary>
        /// Compares two instances of <see cref="FitnessInfo"/> and returns a value indicating
        /// whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />.</returns>
        public int Compare(FitnessInfo x, FitnessInfo y)
        {
            return x.PrimaryFitness.CompareTo(y.PrimaryFitness);
        }
    }
}
