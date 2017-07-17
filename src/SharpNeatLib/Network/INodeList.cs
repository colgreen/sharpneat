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

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a list of INetworkNode's.
    /// Part of the INetworkDefinition type hierarchy.
    /// </summary>
    public interface INodeList : IEnumerable<INetworkNode>
    {
        /// <summary>
        /// Gets the INetworkNode at the specified index.
        /// </summary>
        INetworkNode this[int index] { get; }
        /// <summary>
        /// Gets the count of nodes in the list.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets the index of the INetworkNode with the specified ID. 
        /// Uses a binary search for fast searching, however this assumes the nodes are sorted by
        /// ID in ascending order.
        /// </summary>
        int BinarySearch(uint id);
        /// <summary>
        /// Indicates if the nodes are sorted by ID in ascending order, as required by BinarySearch().
        /// For debug purposes only. Don't call this method in normal circumstances as it is an
        /// expensive O(n) operation.
        /// </summary>
        bool IsSorted();
    }
}
