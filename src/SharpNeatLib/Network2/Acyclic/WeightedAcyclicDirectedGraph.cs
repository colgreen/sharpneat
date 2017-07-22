
namespace SharpNeat.Network2.Acyclic
{
    public class WeightedAcyclicDirectedGraph<T> : AcyclicDirectedGraph
        where T : struct
    {
        public T[] WeightArray { get; }

        #region Constructor

        internal WeightedAcyclicDirectedGraph(
                DirectedConnection[] connArr,
                int nodeCount,
                LayerInfo[] layerArr,
                T[] weightArr) 
            : base(connArr, nodeCount, layerArr)
        {
            this.WeightArray = weightArr;
        }

        #endregion
    }
}
