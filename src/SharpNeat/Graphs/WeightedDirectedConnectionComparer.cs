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

namespace SharpNeat.Graphs
{
    /// <summary>
    /// An <see cref="IComparer{T}"/> for comparing instances of <see cref="WeightedDirectedConnection{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeightedDirectedConnectionComparer<T> : IComparer<WeightedDirectedConnection<T>>
        where T : struct
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public readonly static WeightedDirectedConnectionComparer<T> Default = new WeightedDirectedConnectionComparer<T>();

        /// <summary>
        /// Compares two instances of <see cref="WeightedDirectedConnection{T}"/> and returns a value indicating
        /// whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />.</returns>
        public int Compare(
            WeightedDirectedConnection<T> x,
            WeightedDirectedConnection<T> y)
        {
            // Compare source IDs.
            if (x.SourceId < y.SourceId) {
                return -1;
            }
            if (x.SourceId > y.SourceId) {
                return 1;
            }

            // Source IDs are equal; compare target IDs.
            if (x.TargetId < y.TargetId) {
                return -1;
            }
            if (x.TargetId > y.TargetId) {
                return 1;
            }
            return 0;
        }
    }
}
