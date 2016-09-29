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
    /// Represents a connection between two nodes. Used primarily as a key into a
    /// Dictionary that uniquely identifies connections by their end points.
    /// </summary>
    public struct ConnectionEndpointsStruct : IEqualityComparer<ConnectionEndpointsStruct>
    {
        readonly uint _srcNodeId;
        readonly uint _tgtNodeId;

        #region Constructor

        /// <summary>
        /// Construct with the provided source and target node IDs.
        /// </summary>
        public ConnectionEndpointsStruct(uint sourceNodeId, uint targetNodeId)
        {
            _srcNodeId = sourceNodeId;
            _tgtNodeId = targetNodeId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the source node ID.
        /// </summary>
        public uint SourceNodeId
        {
            get { return _srcNodeId; }
        }

        /// <summary>
        /// Gets the target node ID.
        /// </summary>
        public uint TargetNodeId
        {
            get { return _tgtNodeId; }
        }

        #endregion

        #region IEqualityComparer<ConnectionEndpointsStruct> Members

        /// <summary>
        /// Implementation for IEqualityComparer.
        /// </summary>
        public bool Equals(ConnectionEndpointsStruct x, ConnectionEndpointsStruct y)
        {
            return (x._srcNodeId == y._srcNodeId) && (x._tgtNodeId == y._tgtNodeId);
        }

        /// <summary>
        /// Implementation for IEqualityComparer.
        /// </summary>
        public int GetHashCode(ConnectionEndpointsStruct obj)
        {
            // Drawing.Point uses x^y for a hash, but this is actually an extremely poor hash function
            // for a pair of coordinates. Here we swap the low and high 16 bits of one of the 
            // Id's to generate a much better hash for our (and most other likely) circumstances.
            return (int)(_srcNodeId ^ ((_tgtNodeId>>16) + (_tgtNodeId<<16)));   
         
            // ENHANCEMENT: Consider better hashes such as FNV or SuperFastHash
            // Also try this from Java's com.sun.hotspot.igv.data.Pair class.
            // return (int)(_srcNeuronId * 71u + _tgtNeuronId);
        }

        #endregion
    }
}
