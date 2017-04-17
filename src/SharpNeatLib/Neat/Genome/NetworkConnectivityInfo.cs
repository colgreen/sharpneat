using Redzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat.Genome
{
    public class NetworkConnectivityInfo
    {
        readonly static Comparison<Connection> __comparisonFn;
        readonly Connection[] _connectionList;
        readonly HashSet<uint> _nodeIdSet;

        #region Static Initialiser

        static NetworkConnectivityInfo()
        {
            __comparisonFn = delegate(Connection x, Connection y) {
                // Primary compare on source ID.
                int cmp = x.SrcId.CompareTo(y.SrcId);
                if(0 != cmp) {
                    return cmp;
                }
                // Secondary compare on target ID.
                return x.TgtId.CompareTo(y.TgtId);
            };
        }

        #endregion

        #region Constructor

        public NetworkConnectivityInfo(ConnectionGeneList connGeneList) 
        {
            // Create a list of Connection(s).
            // Note. the Connection struct is essentially ConnectionGene without a weight property. The weight is not necessary to describe the 
            // network connetivity graph, thus it is dropped to reduce storage.
            //
            // Also create a hashset of all the node IDs.
            _nodeIdSet = new HashSet<uint>();

            int count = connGeneList.Count;
            _connectionList = new Connection[count];
            for(int i=0; i < count; i++) 
            {
                var cGene = connGeneList[i];
                _connectionList[i] = new Connection(cGene);
                _nodeIdSet.Add(cGene.SourceNodeId);
                _nodeIdSet.Add(cGene.TargetNodeId);
            }

            // Sort the connections (by source ID, secondary sort by target id).
            Array.Sort(_connectionList, __comparisonFn);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests if the given node ID has any connections connected to it.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the node with the given ID has connections.</returns>
        public bool ContainsNodeId(uint id) {
            return _nodeIdSet.Contains(id);
        }

        #endregion

        #region Inner Classes

        struct Connection
        {
            public readonly uint SrcId;
            public readonly uint TgtId;

            public Connection(ConnectionGene cGene)
            {
                this.SrcId = cGene.SourceNodeId;
                this.TgtId = cGene.TargetNodeId;
            }
        }

        #endregion
    }
}
