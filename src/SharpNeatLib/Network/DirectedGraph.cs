using System;

namespace SharpNeat.Network2
{
    /// <summary>
    /// Overview
    /// --------
    /// Represents a weighted directed graph. The graph is described by an array of connections,
    /// each with a source and target node ID and a weight.
    /// 
    /// The node IDs are actually node indexes, i.e. if there are N unique IDs referred to in the
    /// connection array then the indexes run from 0 to N-1. An exception to this is when representing
    /// graphs from a NeatGenome in which input and outputs nodes are given fixed IDs regardless of whether
    /// they are connected to or not, however the use of a contiguous range of node indexes starting at zero 
    /// still holds in that case.
    /// 
    /// Elsewhere in sharpneat (e.g. in a NeatGenome) graph node IDs are not necessarily contiguous,
    /// and thus any such graph representation must have its non-contiguous node IDs mapped to zero
    /// based node indexes to be represented by this class. Such node ID mapping is outside the scope
    /// of this class.
    /// 
    /// This class can represent both cyclic or acyclic graphs, however, SharpNEAT uses it in the 
    /// conversion of cyclic NeatGenomes only; a specialized class is used for acyclic graphs that 
    /// gives improves runtime performance of the acyclic networks.
    /// 
    /// Specifics
    /// ---------
    /// The connection array is sorted by sourceID and secondary sorted by targetID. This means that all
    /// connections from a given node are located in a contiguous segment, the start of which can be efficiently
    /// located using a binary search. However to improve efficiency further an array of lookup indexes is compiled
    /// which gives the starting index of a connection span/segment for a given source node index.
    /// 
    /// </summary>
    public class DirectedGraph
    {
        #region Instance Fields

        DirectedConnection[] _connArr;

        // The number of input nodes; these are required to be assigned contiguous IDs starting at zero.
        int _inputCount;

        // The number of output nodes; these are required to be assigned contiguous IDs starting immediately after the last input node ID.
        int _outputCount;

        // The total number of nodes in the graph, i.e. input, output and hidden nodes.
        int _totalNodeCount;

        // An array of indexes into _connArr. 
        // For a given node index, gives the index of the first connection with that node as its source.
        int[] _connIdxBySrcNodeIdx;

        #endregion

        #region Constructor

        internal DirectedGraph(
            DirectedConnection[] connArr,
            int inputCount,
            int outputCount,
            int totalNodeCount)
        {
            _connArr = connArr;
            _inputCount = inputCount;
            _outputCount = outputCount;
            _totalNodeCount = totalNodeCount;

            // Determine the connection index that each source node's connections start at.
            CompileSourceNodeConnectionIndexes();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the input node count.
        /// </summary>
        public int InputNodeCount => _inputCount;

        /// <summary>
        /// Get the output node count.
        /// </summary>
        public int OutputNodeCount => _outputCount;

        /// <summary>
        /// Gets the total node count.
        /// </summary>
        public int TotalNodeCount => _totalNodeCount;

        /// <summary>
        /// The internal array of connections. Exposed in this for high performance scenarios.
        /// </summary>
        public DirectedConnection[] ConnectionArray => _connArr;

        #endregion

        #region Public Methods

        public ArraySegment<DirectedConnection> GetConnections(int srcNodeIdx)
        {
            int startIdx = _connIdxBySrcNodeIdx[srcNodeIdx];
            if(-1 == startIdx)
            {   // The specified node has no connections with it as the source.
                // Return an empty array segment.
                return new ArraySegment<DirectedConnection>();
            }

            // Scan for the last connection with the specified source node.
            int endIdx = startIdx+1;
            for(; endIdx < _connArr.Length && _connArr[endIdx].SourceId == srcNodeIdx; endIdx++);

            // Return a array segment over the sub-range of the connection array.
            return new ArraySegment<DirectedConnection>(_connArr, startIdx, endIdx - startIdx);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// For each node that is a source node, determine the first connection with that node as its source.
        /// </summary>
        private void CompileSourceNodeConnectionIndexes()
        {
            // Alloc an array of indexes; one index per node, and init with -1 (indicates that a node has no exit connections).
            // Note. _totalNodeCount may be higher than the number of unique nodes described by the connections, this is to handle
            // input and output nodes in NEAT which are allocated fixed node indexes, but may not have any connections.
            // As such this loop is needed, i.e. don't skip this loop just because _connArr.Length is zero; there may still be a
            // non-zero number of nodes defined.
            _connIdxBySrcNodeIdx = new int[_totalNodeCount];
            for(int i=0; i<_totalNodeCount; i++) {
                _connIdxBySrcNodeIdx[i] = -1;
            }

            // If no connections then nothing to do.
            if(0 == _connArr.Length) {
                return;
            }

            // Initialise.
            int currentSrcNodeId = _connArr[0].SourceId;
            _connIdxBySrcNodeIdx[currentSrcNodeId] = 0;

            // Loop connections.
            for(int i=1; i<_connArr.Length; i++)
            {
                if (_connArr[i].SourceId == currentSrcNodeId)
                {   // Skip.
                    continue;
                }

                // We have arrived at the next source node's connections.
                currentSrcNodeId = _connArr[i].SourceId;
                _connIdxBySrcNodeIdx[_connArr[i].SourceId] = i;
            }
        }

        #endregion
    }
}
