using System.Collections.Generic;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a connection between two nodes. Used primarily as a key into a Dictionary that 
    /// uniquely identifies connections by their end points.
    /// </summary>
    public struct ConnectionEndpointsInfo : IEqualityComparer<ConnectionEndpointsInfo>
    {
        #region Auto Properties

        /// <summary>
        /// Gets the source node ID.
        /// </summary>
        public uint SourceNodeId { get; }
        /// <summary>
        /// Gets the target node ID.
        /// </summary>
        public uint TargetNodeId { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided source and target node IDs.
        /// </summary>
        public ConnectionEndpointsInfo(uint sourceNodeId, uint targetNodeId)
        {
            this.SourceNodeId = sourceNodeId;
            this.TargetNodeId = targetNodeId;
        }

        #endregion

        #region IEqualityComparer<ConnectionEndpointsStruct> Members

        /// <summary>
        /// Implementation for IEqualityComparer.
        /// </summary>
        public bool Equals(ConnectionEndpointsInfo x, ConnectionEndpointsInfo y)
        {
            return (x.SourceNodeId == y.SourceNodeId) && (x.TargetNodeId == y.TargetNodeId);
        }

        /// <summary>
        /// Implementation for IEqualityComparer.
        /// </summary>
        public int GetHashCode(ConnectionEndpointsInfo obj)
        {
            // TODO: Just use the standard approach of multipying by prime numbers.
            // Drawing.Point uses x^y for a hash, but this is actually an extremely poor hash function
            // for a pair of coordinates. Here we swap the low and high 16 bits of one of the 
            // Id's to generate a much better hash for our (and most other likely) circumstances.
            return (int)(this.SourceNodeId ^ ((this.TargetNodeId>>16) + (this.TargetNodeId<<16)));   

            // ENHANCEMENT: Consider better hashes such as FNV or SuperFastHash
            // Also try this from Java's com.sun.hotspot.igv.data.Pair class.
            // return (int)(_srcNeuronId * 71u + _tgtNeuronId);
        }

        #endregion
    }
}
