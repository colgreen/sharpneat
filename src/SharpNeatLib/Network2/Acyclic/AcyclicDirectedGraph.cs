

namespace SharpNeat.Network2.Acyclic
{
    public class AcyclicDirectedGraph : DirectedGraph
    {
        public LayerInfo[] LayerArray { get; }

        #region Constructor

        internal AcyclicDirectedGraph(
                DirectedConnection[] connArr,
                int nodeCount,
                LayerInfo[] layerArr)
            : base(connArr, nodeCount)
        {
            this.LayerArray = layerArr;
        }

        #endregion
    }
}
