using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Network2.Acyclic
{
    public class AcyclicDirectedGraph : DirectedGraph
    {
        readonly LayerInfo[] _layerArr;

        #region Constructor

        internal AcyclicDirectedGraph(
                DirectedConnection[] connArr,
                int nodeCount,
                LayerInfo[] layerArr)
            : base(connArr, nodeCount)
        {
            _layerArr = layerArr;
        }

        #endregion

        #region Properties

        public LayerInfo[] LayerArray => _layerArr;

        #endregion
    }
}
