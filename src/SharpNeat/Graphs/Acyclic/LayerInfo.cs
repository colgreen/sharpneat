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

namespace SharpNeat.Network.Acyclic
{
    /// <summary>
    /// Represents a node and connection index that represent the last node and connection in a given layer
    /// in an acyclic graph.
    /// The nodes and connections on an acyclic graph are ordered by the layer they are in. For more details
    /// see AcyclicDirectedGraph.
    /// </summary>
    public readonly struct LayerInfo
    {
        /// <summary>
        /// Demarcates the position of the last node in the current layer.
        /// Specifically, this is the index+1 of the last node in the current layer.
        /// </summary>
        public int EndNodeIdx { get; }
        /// <summary>
        /// Demarcates the position of the last connection in the current layer.
        /// Specifically, this is the index+1 of the last connection in the current layer.
        /// </summary>     
        public int EndConnectionIdx { get; }

        #region Constructor

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="endNodeIdx">Demarcates the position of the last node in the current layer.</param>
        /// <param name="endConnectionIdx">Demarcates the position of the last connection in the current layer.</param>
        public LayerInfo(int endNodeIdx, int endConnectionIdx)
        {
            this.EndNodeIdx = endNodeIdx;
            this.EndConnectionIdx = endConnectionIdx;
        }

        #endregion
    }
}
