
namespace SharpNeat.Network.Acyclic
{
    public class WeightedAcyclicDirectedGraph<T> : AcyclicDirectedGraph
        where T : struct
    {
        /// <summary>
        /// Connection weight array.
        /// </summary>
        public T[] WeightArray { get; }

        #region Constructor

        internal WeightedAcyclicDirectedGraph(
            ConnectionIdArrays connIdArrays,
            int inputCount,
            int outputCount,
            int nodeCount,
            LayerInfo[] layerArr,
            int[] outputNodeIdxArr,
            T[] weightArr) 
        : base(connIdArrays, inputCount, outputCount, nodeCount, layerArr, outputNodeIdxArr)
        {
            this.WeightArray = weightArr;
        }

        #endregion
    }
}
