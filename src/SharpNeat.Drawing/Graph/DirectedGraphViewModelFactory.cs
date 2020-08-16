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
using SharpNeat.Graphs;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.Drawing.Graph
{
    public static class DirectedGraphViewModelFactory
    {
        #region Public Static Methods

        public static DirectedGraphViewModel Create(
            DirectedGraph digraph,
            float[] weightArr,
            int[] nodeIdByIdx,
            float connectionWeightRange)
        {
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

            // DirectedGraphAcyclic contains depth info, and therfore no further depth analysis of the
            // digraph nodes is required here.

            // Create a node layer by node index, array.
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

            // Assign output nodes to their own layer, so that they are all shown in one layer
            // visually, and separated from hidden nodes.
            int outputLayerIdx = digraphAcyclic.LayerArray.Length;
            foreach(int outputNodeIdx in digraphAcyclic.OutputNodeIdxArr) {
                nodeLayerByIdx[outputNodeIdx] = outputLayerIdx;
            }

            // Calc how many nodes three are in each layer.
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

                    // Set the empty layer's layer index to -1, primarily to mark it as not a vald ID (although we don't actually use this
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


        private static DirectedGraphViewModel CreateCyclic(
            DirectedGraph digraph,
            float[] weightArr,
            int [] nodeIdByIdx,
            float connectionWeightRange)
        {
            // TODO: Implement. This requires an equivalent to CyclicNetworkDepthAnalysis in the SharpNEAT 2.x to calc nodes depths for an cyclic graph.
            return null;
        }

        #endregion
    }
}
