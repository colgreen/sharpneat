

namespace SharpNeat.Network.Acyclic
{
    public class AcyclicDirectedGraph : DirectedGraph
    {
        public LayerInfo[] LayerArray { get; }

        #region Constructor

        internal AcyclicDirectedGraph(
            ConnectionIdArrays connIdArrays,
            int inputCount,
            int outputCount,
            int nodeCount,
            LayerInfo[] layerArr)
        : base(connIdArrays, inputCount, outputCount, nodeCount)
        {
            this.LayerArray = layerArr;
        }

        #endregion
    }
}
