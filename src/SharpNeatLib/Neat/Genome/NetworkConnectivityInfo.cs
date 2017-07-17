using SharpNeat.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpNeat.Neat.Genome
{
    public class NetworkConnectivityInfo
    {
        #region Static Fields
        readonly static Comparison<ConnectionEndpoints> __comparisonFn;
        readonly static IComparer<ConnectionEndpoints> __comparer;
        #endregion

        #region Instance Fields

        readonly int _inputNodeCount;
        readonly int _outputNodeCount;
        readonly int _inputOutputNodeCount;
        readonly ConnectionEndpoints[] _connectionList;
        readonly uint[] _hiddenNodeIdArr;

        #endregion

        #region Static Initialiser

        static NetworkConnectivityInfo()
        {
            __comparisonFn = delegate(ConnectionEndpoints x, ConnectionEndpoints y) {
                // Primary compare on source ID.
                int cmp = x.SourceId.CompareTo(y.SourceId);
                if(0 != cmp) {
                    return cmp;
                }
                // Secondary compare on target ID.
                return x.TargetId.CompareTo(y.TargetId);
            };

            __comparer = Comparer<ConnectionEndpoints>.Create(__comparisonFn);
        }

        #endregion

        #region Constructor

        public NetworkConnectivityInfo(int inputNodeCount, int outputNodeCount,
                                       ConnectionGeneList connGeneList) 
        {
            _inputNodeCount = inputNodeCount;
            _outputNodeCount = outputNodeCount;
            _inputOutputNodeCount = inputNodeCount + outputNodeCount;

            // Create a list of Connection(s).
            // Note. the Connection struct is essentially ConnectionGene without a weight property. The weight is not necessary to describe the 
            // network connetivity graph, thus it is dropped to reduce storage.
            //
            // Also create a hashset of all the node IDs.
            var hiddenNodeIdSet = new HashSet<uint>();

            int count = connGeneList.Count;
            _connectionList = new ConnectionEndpoints[count];
            for(int i=0; i < count; i++) 
            {
                // Add connection gene to the list.
                var cGene = connGeneList[i];
                _connectionList[i] = new ConnectionEndpoints(cGene.SourceId, cGene.TargetId);

                // Register the source and target nodes IDs of hidden nodes only.
                // Note. input and ouput nodes are defined as always existing, and are allocated predefined IDs.
                if(cGene.SourceId >= _inputOutputNodeCount) {
                    hiddenNodeIdSet.Add(cGene.SourceId);
                }

                if(cGene.TargetId >= _inputOutputNodeCount) {
                    hiddenNodeIdSet.Add(cGene.TargetId);
                }
            }

            // Sort the connections (by source ID, secondary sort by target id).
            Array.Sort(_connectionList, __comparisonFn);

            // Copy the set of hidden nodes IDs into an an array and sort them.
            _hiddenNodeIdArr = hiddenNodeIdSet.ToArray();
            Array.Sort(_hiddenNodeIdArr);
        }

        #endregion

        #region Properties

        public int NodeCount
        {
            get { return _inputOutputNodeCount + _hiddenNodeIdArr.Length; }
        }

        public int HiddenNodeCount
        {
            get { return _hiddenNodeIdArr.Length; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests if the given hidden node ID exists in the genome.
        /// </summary>
        /// <param name="id">node ID.</param>
        /// <returns>True if the node ID exists.</returns>
        public bool ContainsHiddenNodeId(uint id) 
        {
            int idx = Array.BinarySearch(_hiddenNodeIdArr, id);
            return idx >= 0;
        }

        /// <summary>
        /// Tests if the given connection exists in the connectivity graph.
        /// </summary>
        /// <param name="srcId">Source node ID.</param>
        /// <param name="tgtId">Target node ID.</param>
        /// <returns></returns>
        public bool ContainsConnection(uint srcId, uint tgtId)
        {
            int idx = Array.BinarySearch(_connectionList, new ConnectionEndpoints(srcId, tgtId), __comparer);
            return idx >= 0;
        }

        /// <summary>
        /// Get the ID of the node with the given index.
        /// </summary>
        /// <param name="idx">Index.</param>
        /// <returns>Node ID.</returns>
        public uint GetNodeId(int idx) 
        {
            if(idx < _inputOutputNodeCount)
            {   // There are a fixed number of input nodes, and these are defined as having contiguous IDs starting at zero.
                return (uint)idx;
            }

            // The index is in the range occupied by hidden nodes.
            return _hiddenNodeIdArr[idx - _inputOutputNodeCount];
        }

        #endregion
    }
}
