
namespace SharpNeat.Network2
{
    public class WeightedDirectedGraph<T> : DirectedGraph 
        where T : struct
    {
        readonly T[] _weightArr;

        #region Constructor

        internal WeightedDirectedGraph(
            DirectedConnection[] connArr,
            int inputCount,
            int outputCount,
            int nodeCount,
            T[] weightArr) 
        : base(connArr, inputCount, outputCount, nodeCount)
        {
            _weightArr = weightArr;
        }

        #endregion

        #region Properties

        public T[] WeightArray => _weightArr;

        #endregion
    }
}
