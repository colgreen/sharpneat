/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;

namespace SharpNeat.Graphs
{
    // TODO: Review summary and update / re-write.

    /// <summary>
    /// Represents a directed graph.
    /// </summary>
    /// <remarks>
    /// Overview
    /// --------
    /// Represents a directed graph. The graph is described by an array of connections, each with a 
    /// source and target node ID.
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
    /// located using a binary search. However to improve efficiency further, an array of lookup indexes is
    /// compiled, which gives the starting index of a connection span/segment for a each source node index.
    /// </remarks>
    public class DirectedGraph
    {
        #region Instance Fields

        // The number of input nodes; these are required to be assigned contiguous IDs starting at zero.
        readonly int _inputCount;

        // The number of output nodes; these are required to be assigned contiguous IDs starting immediately after the last input node ID.
        readonly int _outputCount;

        // The total number of nodes in the graph, i.e. input, output and hidden nodes.
        readonly int _totalNodeCount;

        // The connection source and target node IDs.
        readonly ConnectionIdArrays _connIdArrays;

        // An array of indexes into _connArr. 
        // For a given node index, gives the index of the first connection with that node as its source.
        int[]? _connIdxBySrcNodeIdx;

        #endregion

        #region Constructor

        internal DirectedGraph(
            int inputCount,
            int outputCount,
            int totalNodeCount,
            in ConnectionIdArrays connIdArrays)
        {
            _inputCount = inputCount;
            _outputCount = outputCount;
            _totalNodeCount = totalNodeCount;
            _connIdArrays = connIdArrays;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the input node count.
        /// </summary>
        public int InputCount => _inputCount;

        /// <summary>
        /// Get the output node count.
        /// </summary>
        public int OutputCount => _outputCount;

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
        /// Get the source node index of the given connection.
        /// </summary>
        /// <param name="connIdx">Connection index.</param>
        /// <returns>The connection's source node index.</returns>
        public ref int GetSouceNodeIdx(int connIdx)
        {
            return ref _connIdArrays._sourceIdArr[connIdx];
        }

        /// <summary>
        /// Get the target node index of the given connection.
        /// </summary>
        /// <param name="connIdx">Connection index.</param>
        /// <returns>The connection's target node index.</returns>
        public ref int GetTargetNodeIdx(int connIdx)
        {
            return ref _connIdArrays._targetIdArr[connIdx];
        }

        /// <summary>
        /// Get the index of the first connection with the given sourceNodeIdx.
        /// </summary>
        /// <param name="srcNodeIdx"></param>
        /// <returns>The index of the first connection with the given source node index, or -1 if no such connection exists.</returns>
        public int GetFirstConnectionIndex(int srcNodeIdx)
        {
            if(_connIdxBySrcNodeIdx is null) {
                _connIdxBySrcNodeIdx = CompileSourceNodeConnectionIndexes();
            }
            return _connIdxBySrcNodeIdx[srcNodeIdx];
        }

        /// <summary>
        /// Get an array of all connection target node indexes for the specified source node index.
        /// </summary>
        public Memory<int> GetTargetNodeIndexes(int srcNodeIdx)
        {
            if(_connIdxBySrcNodeIdx is null) {
                _connIdxBySrcNodeIdx = CompileSourceNodeConnectionIndexes();
            }

            int startIdx = _connIdxBySrcNodeIdx[srcNodeIdx];
            if(startIdx == -1)
            {   // There are no connections that have the specified node as their source.
                // Return an empty array segment.
                return new Memory<int>();
            }

            // Scan for the last connection with the specified source node.
            int[] connSrcIdArr = _connIdArrays._sourceIdArr;
            int[] connTgtIdArr = _connIdArrays._targetIdArr;

            int endIdx = startIdx + 1;
            for(; endIdx < connSrcIdArr.Length && connSrcIdArr[endIdx] == srcNodeIdx; endIdx++);

            // Return an array segment over the sub-range of the connection array.
            return connTgtIdArr.AsMemory(startIdx, endIdx - startIdx);            
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determine the connection index that each source node's connections start at.
        /// </summary>
        private int[] CompileSourceNodeConnectionIndexes()
        {
            // Alloc an array of indexes; one index per node, and init with -1 (indicates that a node has no connections exiting from it).
            // Note. _totalNodeCount may be higher than the number of unique nodes described by the connections, this is to handle
            // input and output nodes in NEAT which are allocated fixed node indexes, but may not have any connections.
            // As such this loop is needed, i.e. don't skip this loop just because _connArr.Length is zero; there may still be a
            // non-zero number of nodes defined.
            int[] connIdxBySrcNodeIdx = new int[_totalNodeCount];
            for(int i=0; i < _totalNodeCount; i++) {
                connIdxBySrcNodeIdx[i] = -1;
            }

            // If no connections then nothing to do.
            int[] srcIdArr = _connIdArrays._sourceIdArr;
            if(srcIdArr.Length == 0) {
                return connIdxBySrcNodeIdx;
            }

            // Initialise.
            int currentSrcNodeId = srcIdArr[0];
            connIdxBySrcNodeIdx[currentSrcNodeId] = 0;

            // Loop connections.
            for(int i=1; i < srcIdArr.Length; i++)
            {
                if(srcIdArr[i] != currentSrcNodeId)
                {
                    // We have arrived at the next source node's connections.
                    currentSrcNodeId = srcIdArr[i];
                    connIdxBySrcNodeIdx[srcIdArr[i]] = i;
                }
            }

            return connIdxBySrcNodeIdx;
        }

        #endregion
    }
}
