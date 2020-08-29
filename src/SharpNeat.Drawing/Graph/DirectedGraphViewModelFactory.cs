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
using System.Diagnostics;
using System.Threading;
using SharpNeat.Graphs;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.Drawing.Graph
{
    /// <summary>
    /// A factory class for creating instances of <see cref="DirectedGraphViewModel"/> from a <see cref="DirectedGraph"/> or a <see cref="DirectedGraphAcyclic"/>.
    /// </summary>
    public class DirectedGraphViewModelFactory
    {
        #region Instance Fields

        readonly CyclicGraphDepthAnalysis _cyclicDepthAnalysis = new CyclicGraphDepthAnalysis();

        /// <summary>
        /// Indicates if a call to Create() is currently in progress. 
        /// For checking for attempts to re-enter that method while a call is in progress.
        /// </summary>
        int _reentranceFlag = 0;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Create a <see cref="DirectedGraphViewModel"/> that represent the provided directed graph.
        /// </summary>
        /// <param name="digraph">The directed graph to construct the model for.</param>
        /// <param name="weightArr">The graph's connection/edge weights.</param>
        /// <param name="nodeIdByIdx">An array that provides a node ID value for each node, i.e. keyed by node index..</param>
        /// <param name="connectionWeightRange">An expected/nominal range for connection weight values.</param>
        /// <returns>A new instance of <see cref="DirectedGraphViewModel"/>.</returns>
        /// <remarks>
        /// This method is not reentrant, i.e. it must only be called by one thread at a time. An attempt to re-enter this method will result in an 
        /// <see cref="InvalidOperationException"/> exception.
        /// 
        /// The reasoning for this is that the factory logic re-uses internal state to reduce the need to re-allocate new data structure on each call 
        /// (and the associated garbage collection required to deallocate). 
        /// </remarks>
        public DirectedGraphViewModel Create(
            DirectedGraph digraph,
            float[] weightArr,
            int[] nodeIdByIdx,
            float connectionWeightRange)
        {
            // Check for attempts to re-enter this method.
            if(Interlocked.CompareExchange(ref _reentranceFlag, 1, 0) == 1) {
                throw new InvalidOperationException("Attempt to re-enter non-reentrant method.");
            }

            if(digraph is DirectedGraphAcyclic digraphAcyclic)
            {
                return CreateAcyclic(digraphAcyclic, weightArr, nodeIdByIdx, connectionWeightRange);
            }
            else
            {
                return CreateCyclic(digraph, weightArr, nodeIdByIdx, connectionWeightRange);
            }
        }

        #endregion

        #region Private Static Methods

        private static DirectedGraphViewModel CreateAcyclic(
            DirectedGraphAcyclic digraphAcyclic,
            float[] weightArr,
            int[] nodeIdByIdx,
            float connectionWeightRange)
        {
            Debug.Assert(nodeIdByIdx.Length == digraphAcyclic.TotalNodeCount);

            // Note. DirectedGraphAcyclic contains depth info, therefore no further depth analysis of the
            // digraph nodes is required here.

            // Create an array that gives the node layer for each node, keyed by node index.
            int nodeCount = digraphAcyclic.TotalNodeCount;
            int[] nodeLayerByIdx = new int[nodeCount];

            // Use a restricted scope for the loop variables.
            { 
                int layerIdx = 0;
                int nodeIdx = 0;
                foreach(LayerInfo layerInfo in digraphAcyclic.LayerArray)
                {
                    for(; nodeIdx < layerInfo.EndNodeIdx; nodeIdx++) {
                        nodeLayerByIdx[nodeIdx] = layerIdx;
                    }
                    layerIdx++;
                }
            }

            // Assign output nodes to their own layer, so that they are shown in one layer visually,
            // separated from hidden nodes.
            int outputLayerIdx = digraphAcyclic.LayerArray.Length;
            foreach(int outputNodeIdx in digraphAcyclic.OutputNodeIdxArr) {
                nodeLayerByIdx[outputNodeIdx] = outputLayerIdx;
            }

            // Calc how many nodes there are in each layer.
            Span<int> nodeCountByLayer = stackalloc int[digraphAcyclic.LayerArray.Length + 1];
            foreach(int layerIdx in nodeLayerByIdx) {
                nodeCountByLayer[layerIdx]++;
            }

            // Create a mapping from old to new layer indexes, and init with the identity mapping.
            Span<int> layerIdxMap = stackalloc int[nodeCountByLayer.Length];
            for(int i=0; i < layerIdxMap.Length; i++) {
                layerIdxMap[i] = i;
            }

            // Loop through nodeCountByLayer backwards, testing for empty layers.
            int layerCount = nodeCountByLayer.Length;

            for(int layerIdx = nodeCountByLayer.Length-1; layerIdx > -1; layerIdx--)
            {
                if(nodeCountByLayer[layerIdx] == 0)
                {
                    // Empty layer detected. Decrement all higher layer indexes to fill the gap.
                    for(int i=layerIdx+1; i < layerIdxMap.Length; i++) {
                        layerIdxMap[i]--;
                    }

                    // Set the empty layer's layer index to -1, primarily to mark it as not a valid ID (although we don't actually use this
                    // anywhere, except maybe for debugging purposes).
                    layerIdxMap[layerIdx] = -1;

                    // Update/track the number of layers with nodes.
                    layerCount--;
                }
            }

            // Apply the node layer index mappings we have just constructed.
            for(int i=0; i < nodeLayerByIdx.Length; i++) {
                nodeLayerByIdx[i] = layerIdxMap[nodeLayerByIdx[i]];
            }

            // Construct a new DirectedGraphViewModel and return.
            return new DirectedGraphViewModel(
                digraphAcyclic, weightArr,
                nodeIdByIdx, nodeLayerByIdx,
                layerCount);
        }

        private DirectedGraphViewModel CreateCyclic(
            DirectedGraph digraph,
            float[] weightArr,
            int [] nodeIdByIdx,
            float connectionWeightRange)
        {
            Debug.Assert(nodeIdByIdx.Length == digraph.TotalNodeCount);

            // Determine a depth/layer for each node in the graph.
            GraphDepthInfo depthInfo = _cyclicDepthAnalysis.CalculateNodeDepths(digraph);
            int[] nodeLayerByIdx = depthInfo._nodeDepthArr;

            // Assign output nodes to their own layer, so that they are shown in one layer visually,
            // separated from hidden nodes.
            int outputLayerIdx = depthInfo._graphDepth;
            for(int outputIdx=0; outputIdx < digraph.OutputCount; outputIdx++)
            {
                int outputNodeIdx = digraph.InputCount + outputIdx;
                nodeLayerByIdx[outputNodeIdx] = outputLayerIdx;
            }

            // Calc how many nodes there are in each layer.
            Span<int> nodeCountByLayer = stackalloc int[depthInfo._graphDepth + 1];
            foreach(int layerIdx in nodeLayerByIdx) {
                nodeCountByLayer[layerIdx]++;
            }

            // Create a mapping from old to new layer indexes, and init with the identity mapping.
            Span<int> layerIdxMap = stackalloc int[nodeCountByLayer.Length];
            for(int i=0; i < layerIdxMap.Length; i++) {
                layerIdxMap[i] = i;
            }

            // Loop through nodeCountByLayer backwards, testing for empty layers.
            int layerCount = nodeCountByLayer.Length;

            for(int layerIdx = nodeCountByLayer.Length-1; layerIdx > -1; layerIdx--)
            {
                if(nodeCountByLayer[layerIdx] == 0)
                {
                    // Empty layer detected. Decrement all higher layer indexes to fill the gap.
                    for(int i=layerIdx+1; i < layerIdxMap.Length; i++) {
                        layerIdxMap[i]--;
                    }

                    // Set the empty layer's layer index to -1, primarily to mark it as not a valid ID (although we don't actually use this
                    // anywhere, except maybe for debugging purposes).
                    layerIdxMap[layerIdx] = -1;

                    // Update/track the number of layers with nodes.
                    layerCount--;
                }
            }

            // Apply the node layer index mappings we have just constructed.
            for(int i=0; i < nodeLayerByIdx.Length; i++) {
                nodeLayerByIdx[i] = layerIdxMap[nodeLayerByIdx[i]];
            }

            // Construct a new DirectedGraphViewModel and return.
            return new DirectedGraphViewModel(
                digraph, weightArr,
                nodeIdByIdx, nodeLayerByIdx,
                layerCount);
        }

        #endregion
    }
}
