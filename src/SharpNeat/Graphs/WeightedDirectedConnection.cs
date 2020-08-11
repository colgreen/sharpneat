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

namespace SharpNeat.Graphs
{
    /// <summary>
    /// Represents a connection between two nodes, combined with a connection weight.
    /// </summary>
    /// <remarks>
    /// This type does not inherit from DirectedConnection as might be expected because these are structs (value types)
    /// and therefore inheritance is not possible.
    /// </remarks>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public readonly struct WeightedDirectedConnection<T>
        where T : struct
    {
        #region Auto Properties

        /// <summary>
        /// Connection source node ID.
        /// </summary>
        public int SourceId { get; }
        /// <summary>
        /// Connection target node ID.
        /// </summary>
        public int TargetId { get; }
        /// <summary>
        /// Connection weight.
        /// </summary>
        public T Weight { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with the provided source and target node IDs, and weight.
        /// </summary>
        public WeightedDirectedConnection(int srcId, int tgtId, T weight)
        {
            this.SourceId = srcId;
            this.TargetId = tgtId;
            this.Weight = weight;
        }

        #endregion
    }
}
