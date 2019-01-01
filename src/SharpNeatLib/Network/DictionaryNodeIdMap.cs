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
using System.Diagnostics;
using System.Linq;

namespace SharpNeat.Network
{
    /// <summary>
    /// An INodeIdMap implementation based on a dictionary keyed by node ID and a fixed node count.
    /// </summary>
    /// <remarks>
    /// The fixed nodes count defines the identity mapping (i.e. x maps to x) for IDs from 0 to count-1,
    /// i.e. it's a cheap way of describing those mappings rather than including them in the dictionary,
    /// which is relatively expensive to populate and query.
    /// 
    /// Input nodes are *always* fixed, i.e. they exist in a contiguous run of IDs starting at zero.
    /// In cyclic networks the output nodes are also fixed, starting directly after the input node IDs.
    /// In acyclic networks the outputs are not fixed, and are therefore mapped by the dictionary.
    /// </remarks>
    public class DictionaryNodeIdMap : INodeIdMap
    {
        readonly int _fixedNodeCount;
        readonly Dictionary<int,int> _nodeIdxById;

        #region Constructor

        /// <summary>
        /// Construct with the given pre-built dictionary, and a fixed node count.
        /// </summary>
        /// <param name="fixedNodeCount">Fixed node count.</param>
        /// <param name="nodeIdxById">A pre-built dictionary of node ID to index mappings.</param>
        public DictionaryNodeIdMap(
            int fixedNodeCount, 
            Dictionary<int,int> nodeIdxById)
        {
            // The dictionary should not contain any mappings from IDs in the fixed ID range.
            Debug.Assert(nodeIdxById.Keys.All(x => x >= fixedNodeCount));

            _fixedNodeCount = fixedNodeCount;
            _nodeIdxById = nodeIdxById;
        }

        #endregion

        #region INodeIdMap

        /// <summary>
        /// Gets the total number of mapped node IDs.
        /// </summary>
        public int Count
        {
            get => _fixedNodeCount + _nodeIdxById.Count;
        }

        /// <summary>
        /// Map a given node ID
        /// </summary>
        /// <param name="id">A node ID.</param>
        /// <returns>The mapped to ID.</returns>
        public int Map(int id)
        {
            // Input node IDs are always at the head of the array, and are fixed.
            // Output nodes may also be included in the fixed node count (see class remarks).
            if (id < _fixedNodeCount)
            {
                return id;
            }
            // Hidden nodes have mappings stored in a dictionary.
            return _nodeIdxById[id];
        }

        #endregion
    }
}
