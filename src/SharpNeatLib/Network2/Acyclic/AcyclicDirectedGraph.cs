

namespace SharpNeat.Network2.Acyclic
{
    public class AcyclicDirectedGraph : DirectedGraph
    {
        public LayerInfo[] LayerArray { get; }

        #region Constructor

        internal AcyclicDirectedGraph(
            DirectedConnection[] connArr,
            int inputCount,
            int outputCount,
            int nodeCount,
            LayerInfo[] layerArr)
        : base(connArr, inputCount, outputCount, nodeCount)
        {
            this.LayerArray = layerArr;
        }

        #endregion
    }
}
