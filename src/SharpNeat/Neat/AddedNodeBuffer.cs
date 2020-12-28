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
using Redzen.Structures;
using SharpNeat.Graphs;

namespace SharpNeat.Neat
{
    // ENHANCEMENT: Consider moving the most recently used structure to the head of the buffer to increase its lifespan.
    /// <summary>
    /// Stores a history of previously added nodes, keyed by the connection that was split to create the node (i.e.
    /// the source and target node IDs of the connection).
    ///
    /// Used when adding new nodes to check if an identical node (connection split) has been added to a genome elsewhere
    /// in the population. This allows re-use of the same innovation ID for like nodes.
    /// </summary>
    public sealed class AddedNodeBuffer
    {
        readonly KeyedCircularBuffer<DirectedConnection,int> _buffer;

        #region Constructor

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="capacity">Capacity of the history buffer.</param>
        public AddedNodeBuffer(int capacity)
        {
            _buffer = new KeyedCircularBuffer<DirectedConnection,int>(capacity);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Register a new added node.
        /// </summary>
        /// <param name="conn">The connection that was split in order to add the node.</param>
        /// <param name="addedNodeId">The ID of the added node.</param>
        public void Register(in DirectedConnection conn, int addedNodeId)
        {
            _buffer.Enqueue(in conn, addedNodeId);
        }

        /// <summary>
        /// Get the node innovation ID from a previous 'add node' mutation based on splitting a connection with the given ID.
        /// </summary>
        /// <param name="conn">The connection to look-up.</param>
        /// <param name="addedNodeId">Returns a node innovation ID, if one was found.</param>
        /// <returns>True if a node was found, otherwise false.</returns>
        public bool TryLookup(in DirectedConnection conn, out int addedNodeId)
        {
            return _buffer.TryGetValue(in conn, out addedNodeId);
        }

        #endregion
    }
}
