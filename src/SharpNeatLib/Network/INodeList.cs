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
