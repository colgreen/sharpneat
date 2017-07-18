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

        // The number of nodes in the graph.
        int _nodeCount;

        // An array of indexes into _connArr. 
        // For a given node index, gives the index of the first connection with that node as its source.
        int[] _connIdxBySrcNodeIdx;

        #endregion

        #region Constructor

        internal DirectedGraph(
            DirectedConnection[] connArr,
            int nodeCount)
        {
            _connArr = connArr;
            _nodeCount = nodeCount;

            // Determine the connection index that each source node's connections start at.
            CompileSourceNodeConnectionIndexes();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the graph node count.
        /// Note. if additional node IDs were defined at construction time then this may give a higher number of nodes than described by the connections.
        /// </summary>
        public int NodeCount => _nodeCount;

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
            // Note. _nodeCount may be higher than the number of unique nodes described by the connections, this is to handle
            // input and output nodes in NEAT which are allocated fixed node indexes, but may not have any connections.
            // As such this loop is needed, i.e. don't skip this loop just because _connArr.Length is zero; there may still be a
            // non-zero number of nodes defined.
            _connIdxBySrcNodeIdx = new int[_nodeCount];
            for(int i=0; i<_nodeCount; i++) {
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
