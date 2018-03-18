using System;
using System.Collections.Generic;

namespace SharpNeat.Network
{
    /// <summary>
    /// Overview
    /// --------
    /// Represents a directed graph. The graph is described by an array of connections,
    /// each with a source and target node ID.
    /// 
    /// The node IDs are actually node indexes, i.e. if there are N unique IDs referred to in the
    /// connection array then the indexes run from 0 to N-1.
    /// 
    /// Elsewhere in sharpneat (e.g. in a NeatGenome) graph node IDs are not necessarily contiguous,
    /// and thus any such graph representation must have its non-contiguous node IDs mapped to zero
    /// based node indexes to be represented by this class. Such node ID mapping is outside the scope
    /// of this class.
    /// 
    /// This class can represent both cyclic or acyclic graphs, however, SharpNEAT uses it in the 
    /// conversion of cyclic NeatGenomes only; a specialized class is used for acyclic graphs that 
    /// gives improved runtime performance for acyclic networks.
    /// 
    /// Specifics
    /// ---------
    /// The connection array is sorted by sourceIdx and secondary sorted by targetIdx. This means that all
    /// connections from a given node are located in a contiguous segment, the start of which can be efficiently
    /// located using a binary search. However to improve efficiency further an array of lookup indexes is compiled
    /// which gives the starting index of a connection span/segment for a given source node index.
    /// </summary>
    public class DirectedGraph
    {
        #region Instance Fields

        ConnectionIdArrays _connIdArrays;

        // The number of input nodes; these are required to be assigned contiguous IDs starting at zero.
        readonly int _inputCount;

        // The number of output nodes; these are required to be assigned contiguous IDs starting immediately after the last input node ID.
        readonly int _outputCount;

        // The total number of nodes in the graph, i.e. input, output and hidden nodes.
        readonly int _totalNodeCount;

        // An array of indexes into _connArr. 
        // For a given node index, gives the index of the first connection with that node as its source.
        int[] _connIdxBySrcNodeIdx;

        #endregion

        #region Constructor

        internal DirectedGraph(
            ConnectionIdArrays connIdArrays,
            int inputCount,
            int outputCount,
            int totalNodeCount)
        {
            _connIdArrays = connIdArrays;
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
        /// The internal arrays of connection source and target node indexes. Exposed publicly for high performance scenarios.
        /// </summary>
        public ConnectionIdArrays ConnectionIdArrays => _connIdArrays;

        #endregion

        #region Public Methods

        /// <summary>
        /// Get an array of all connection target node indexes for the specified source node index.
        /// </summary>
        public IList<int> GetConnections(int srcNodeIdx)
        {
            int startIdx = _connIdxBySrcNodeIdx[srcNodeIdx];
            if(-1 == startIdx)
            {   // There are no connections that have the specified node as their source.
                // Return an empty array segment.
                return new ArraySegment<int>();
            }

            // Scan for the last connection with the specified source node.
            int[] connSrcIdArr = _connIdArrays._sourceIdArr;
            int[] connTgtIdArr = _connIdArrays._targetIdArr;

            int endIdx = startIdx+1;
            for(; endIdx < connSrcIdArr.Length && connSrcIdArr[endIdx] == srcNodeIdx; endIdx++);

            // Return an array segment over the sub-range of the connection array.
            return new ArraySegment<int>(connTgtIdArr, startIdx, endIdx - startIdx);
        }

        /// <summary>
        /// Get the index of the first connection with the given sourceNodeIdx.
        /// </summary>
        /// <param name="srcNodeIdx"></param>
        /// <returns>The index of the first connection with the given source node index, or -1 if no such connection exists.</returns>
        public int GetFirstConnectionIndex(int srcNodeIdx)
        {
            return _connIdxBySrcNodeIdx[srcNodeIdx];
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// For each node that is a source node, determine the first connection with that node as its source.
        /// </summary>
        private void CompileSourceNodeConnectionIndexes()
        {
            // Alloc an array of indexes; one index per node, and init with -1 (indicates that a node has no connections exiting from it).
            // Note. _totalNodeCount may be higher than the number of unique nodes described by the connections, this is to handle
            // input and output nodes in NEAT which are allocated fixed node indexes, but may not have any connections.
            // As such this loop is needed, i.e. don't skip this loop just because _connArr.Length is zero; there may still be a
            // non-zero number of nodes defined.
            _connIdxBySrcNodeIdx = new int[_totalNodeCount];
            for(int i=0; i<_totalNodeCount; i++) {
                _connIdxBySrcNodeIdx[i] = -1;
            }

            // If no connections then nothing to do.
            int[] srcIdArr = _connIdArrays._sourceIdArr;
            if(0 == srcIdArr.Length) {
                return;
            }

            // Initialise.
            int currentSrcNodeId = srcIdArr[0];
            _connIdxBySrcNodeIdx[currentSrcNodeId] = 0;

            // Loop connections.
            for(int i=1; i < srcIdArr.Length; i++)
            {
                if (srcIdArr[i] == currentSrcNodeId)
                {   // Skip.
                    continue;
                }

                // We have arrived at the next source node's connections.
                currentSrcNodeId = srcIdArr[i];
                _connIdxBySrcNodeIdx[srcIdArr[i]] = i;
            }
        }

        #endregion
    }
}
