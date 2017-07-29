
namespace SharpNeat.Network
{
    public class WeightedDirectedGraph<T> : DirectedGraph 
        where T : struct
    {
        /// <summary>
        /// Connection weight array.
        /// </summary>
        public T[] WeightArray { get; }

        #region Constructor

        internal WeightedDirectedGraph(
            DirectedConnection[] connArr,
            int inputCount,
            int outputCount,
            int nodeCount,
            T[] weightArr) 
        : base(connArr, inputCount, outputCount, nodeCount)
        {
            this.WeightArray = weightArr;
        }

        #endregion
    }
}
