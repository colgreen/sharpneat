using System;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a connection between two nodes, combined with a connection weight.
    /// </summary>
    /// <remarks>
    /// This type does not inherit from DirectedConnection as might be expected because these are structs (value types)
    /// and therefore inheritance is not possible.
    /// </remarks>
    public struct WeightedDirectedConnection<T>
        where T : struct
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
        /// <summary>
        /// Connection weight.
        /// </summary>
        public T Weight { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with the provided source and target node IDs, and weight.
        /// </summary>
        public WeightedDirectedConnection(int srcId, int tgtId, T weight)
        {
            this.SourceId = srcId;
            this.TargetId = tgtId;
            this.Weight = weight;
        }

        #endregion
    }
}
