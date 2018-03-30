using System;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a connection between two nodes. Used primarily as a key into a Dictionary that 
    /// uniquely identifies connections by their end points.
    /// </summary>
    public readonly struct DirectedConnection : IEquatable<DirectedConnection>, IComparable<DirectedConnection>
    {
        #region Auto Properties

        /// <summary>
        /// Connection source node ID.
        /// </summary>
        public int SourceId { get; }
        /// <summary>
        /// Connection target node ID.
        /// </summary>
        public int TargetId { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with the provided source and target node IDs.
        /// </summary>
        public DirectedConnection(int srcId, int tgtId)
        {
            this.SourceId = srcId;
            this.TargetId = tgtId;
        }

        /// <summary>
        /// Construct with the provided source and target node IDs.
        /// </summary>
        public DirectedConnection(DirectedConnection copyFrom)
        {
            this.SourceId = copyFrom.SourceId;
            this.TargetId = copyFrom.TargetId;
        }

        #endregion

        #region IEquatable / IComparable

        public bool Equals(DirectedConnection other)
        {
            return (this.SourceId == other.SourceId) 
                && (this.TargetId == other.TargetId);
        }

        public int CompareTo(DirectedConnection other)
        {
            // Compare source IDs.
            if (this.SourceId < other.SourceId) { return -1; }
            if (this.SourceId > other.SourceId) { return 1; }

            // Source IDs are equal; compare target IDs.
            if (this.TargetId < other.TargetId) { return -1; }
            if (this.TargetId > other.TargetId) { return 1; }
            return 0;
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if(obj is DirectedConnection) {
                return this.Equals((DirectedConnection)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Variant on FNV hash taken from: http://stackoverflow.com/a/263416/15703
            unchecked
            {
                int v = (int)2166136261;
                v = (v * 16777619) ^ SourceId;
                v = (v * 16777619) ^ TargetId;
                return v;
            }
        }

        public static bool operator ==(DirectedConnection x, DirectedConnection y)
        {
            return (x.SourceId == y.SourceId) 
                && (x.TargetId == y.TargetId);
        }

        public static bool operator !=(DirectedConnection x, DirectedConnection y)
        {
            return (x.SourceId != y.SourceId) 
                || (x.TargetId != y.TargetId);
        }

        public static bool operator <(DirectedConnection x, DirectedConnection y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator >(DirectedConnection x, DirectedConnection y)
        {
            return x.CompareTo(y) > 0;
        }

        #endregion
    }
}
