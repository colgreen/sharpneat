using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Network2
{
    public class WeightedDirectedGraph<T> : DirectedGraph 
        where T : struct
    {
        readonly T[] _weightArr;

        #region Constructor

        internal WeightedDirectedGraph(
            DirectedConnection[] connArr,
            int nodeCount,
            T[] weightArr) 
            : base(connArr, nodeCount)
        {
            _weightArr = weightArr;
        }

        #endregion

        #region Properties

        public T[] WeightArray => _weightArr;

        #endregion
    }
}
