
namespace SharpNeat.Network2.Acyclic
{
    public class WeightedAcyclicDirectedGraph<T> : AcyclicDirectedGraph
        where T : struct
    {
        public T[] WeightArray { get; }

        #region Constructor

        internal WeightedAcyclicDirectedGraph(
            DirectedConnection[] connArr,
            int inputCount,
            int outputCount,
            int nodeCount,
            LayerInfo[] layerArr,
            T[] weightArr) 
        : base(connArr, inputCount, outputCount, nodeCount, layerArr)
        {
            this.WeightArray = weightArr;
        }

        #endregion
    }
}
