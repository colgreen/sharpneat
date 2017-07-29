
namespace SharpNeat.Network.Acyclic
{
    /// <summary>
    /// Represents a node and connection index that represent the last node and connection in a given layer
    /// in an acyclic graph.
    /// The nodes and connections on an acyclic graph are ordered by the layer they are in. For more details
    /// see AcyclicDirectedGraph.
    /// </summary>
    public struct LayerInfo
    {
        /// <summary>
        /// Demarks the last node in the current layer.
        /// Specifically, this is the index+1 of the last node in the current layer.
        /// </summary>
        public int EndNodeIdx { get; }
        /// <summary>
        /// Demarks the last connection in the current layer.
        /// Specifically, this is the index+1 of the last connection in the current layer.
        /// </summary>     
        public int EndConnectionIdx { get; }

        #region Constructor

        public LayerInfo(int endNodeIdx, int endConnectionIdx)
        {
            this.EndNodeIdx = endNodeIdx;
            this.EndConnectionIdx = endConnectionIdx;
        }

        #endregion
    }
}
