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
