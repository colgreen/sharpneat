

namespace SharpNeat.Network.Acyclic
{
    public class AcyclicDirectedGraph : DirectedGraph
    {
        public LayerInfo[] LayerArray { get; }

        /// <summary>
        /// Gives the node index of each output. In acyclic networks the output and hidden nodes are re-ordered by network depth.
        /// This array describes the location of each output signal in the node activation signal array.
        /// Note however that the input nodes *are* in their original positions as they are defined as being at depth zero and therefore
        /// are not moved by the depth based sort.
        /// </summary>
        public int[] OutputNodeIdxArr { get; }

        #region Constructor

        internal AcyclicDirectedGraph(
            ConnectionIdArrays connIdArrays,
            int inputCount,
            int outputCount,
            int nodeCount,
            LayerInfo[] layerArr,
            int[] outputNodeIdxArr)
        : base(connIdArrays, inputCount, outputCount, nodeCount)
        {
            this.LayerArray = layerArr;
            this.OutputNodeIdxArr = outputNodeIdxArr;
        }

        #endregion
    }
}
