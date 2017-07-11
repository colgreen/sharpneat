using System;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a connection between two nodes. Used primarily as a key into a Dictionary that 
    /// uniquely identifies connections by their end points.
    /// </summary>
    public struct ConnectionEndpoints : IEquatable<ConnectionEndpoints> 
    {
        #region Auto Properties

        /// <summary>
        /// Gets the source node ID.
        /// </summary>
        public uint SourceId { get; }
        /// <summary>
        /// Gets the target node ID.
        /// </summary>
        public uint TargetId { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided source and target node IDs.
        /// </summary>
        public ConnectionEndpoints(uint srcId, uint tgtId)
        {
            this.SourceId = srcId;
            this.TargetId = tgtId;
        }

        #endregion

        #region IEquatable

        public bool Equals(ConnectionEndpoints other)
        {
            return (this.SourceId == other.SourceId) 
                && (this.TargetId == other.TargetId);
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if(obj is ConnectionEndpoints) {
                return this.Equals((ConnectionEndpoints)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Variant on FNV hash taken from: http://stackoverflow.com/a/263416/15703
            unchecked
            {
                int v = (int)2166136261;
                v = (v * 16777619) ^ (int)SourceId;
                v = (v * 16777619) ^ (int)TargetId;
                return v;
            }
        }

        #endregion
    }
}
