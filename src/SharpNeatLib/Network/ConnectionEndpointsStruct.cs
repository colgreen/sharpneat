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

using System;
using System.Collections.Generic;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a connection between two nodes. Used primarily as a key into a
    /// Dictionary that uniquely identifies connections by their end points.
    /// </summary>
    public struct ConnectionEndpointsStruct : IEquatable<ConnectionEndpointsStruct> 
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

        #region IEquatable

        public bool Equals(ConnectionEndpointsStruct other)
        {
            return (this.SourceNodeId == other.SourceNodeId) 
                && (this.TargetNodeId == other.TargetNodeId);
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if(obj is ConnectionEndpointsStruct) {
                return this.Equals((ConnectionEndpointsStruct)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Variant on FNV hash taken from: http://stackoverflow.com/a/263416/15703
            unchecked
            {
                int v = (int)2166136261;
                v = (v * 16777619) ^ (int)SourceNodeId;
                v = (v * 16777619) ^ (int)TargetNodeId;
                return v;
            }
        }

        #endregion
    }
}
