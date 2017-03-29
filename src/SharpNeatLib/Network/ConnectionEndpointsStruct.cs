using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a connection between two nodes. Used primarily as a key into a
    /// Dictionary that uniquely identifies connections by their end points.
    /// </summary>
    public struct ConnectionEndpointsStruct : IEqualityComparer<ConnectionEndpointsStruct>
    {
        #region Auto Properties

        /// <summary>
        /// Gets the source node ID.
        /// </summary>
        public uint SourceNodeId { get; private set; }
        /// <summary>
        /// Gets the target node ID.
        /// </summary>
        public uint TargetNodeId { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided source and target node IDs.
        /// </summary>
        public ConnectionEndpointsStruct(uint sourceNodeId, uint targetNodeId)
        {
            this.SourceNodeId = sourceNodeId;
            this.TargetNodeId = targetNodeId;
        }

        #endregion

        #region IEqualityComparer<ConnectionEndpointsStruct> Members

        /// <summary>
        /// Implementation for IEqualityComparer.
        /// </summary>
        public bool Equals(ConnectionEndpointsStruct x, ConnectionEndpointsStruct y)
        {
            return (x.SourceNodeId == y.SourceNodeId) && (x.TargetNodeId == y.TargetNodeId);
        }

        /// <summary>
        /// Implementation for IEqualityComparer.
        /// </summary>
        public int GetHashCode(ConnectionEndpointsStruct obj)
        {
            // TODO: Just use the standard appraoch of multipying by prime numbers.
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
