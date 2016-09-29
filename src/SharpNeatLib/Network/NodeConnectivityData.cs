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
    /// Stored the IDs of nodes that connect in and out of a given node.
    /// </summary>
    public class NodeConnectivityData
    {
        /// <summary>
        /// Node ID.
        /// </summary>
        public readonly uint _id;
        /// <summary>
        /// A set of node IDs representing nodes that connect in to a given node.
        /// </summary>
        public readonly HashSet<uint> _srcNodes;
        /// <summary>
        /// A set of node IDs representing nodes that a given node connects out to.
        /// </summary>
        public readonly HashSet<uint> _tgtNodes;

        #region Constructors

        /// <summary>
        /// Construct with empty source/target node sets.
        /// </summary>
        public NodeConnectivityData(uint id)
        {
            _id = id;
            _srcNodes = new HashSet<uint>();
            _tgtNodes = new HashSet<uint>();
        }

        /// <summary>
        /// Construct with the provided source/target node sets.
        /// </summary>
        public NodeConnectivityData(uint id, HashSet<uint> srcNodes, HashSet<uint> tgtNodes)
        {
            _id = id;
            _srcNodes = srcNodes;
            _tgtNodes = tgtNodes;
        }

        #endregion
    }
}
