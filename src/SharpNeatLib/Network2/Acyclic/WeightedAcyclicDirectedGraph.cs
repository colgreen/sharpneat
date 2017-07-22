using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Network2.Acyclic
{
    public class WeightedAcyclicDirectedGraph<T> : AcyclicDirectedGraph
        where T : struct
    {
        readonly T[] _weightArr;

        #region Constructor

        internal WeightedAcyclicDirectedGraph(
                DirectedConnection[] connArr,
                int nodeCount,
                LayerInfo[] layerArr,
                T[] weightArr) 
            : base(connArr, nodeCount, layerArr)
        {
            _weightArr = weightArr;
        }

        #endregion

        #region Properties

        public T[] WeightArray => _weightArr;

        #endregion
    }
}
