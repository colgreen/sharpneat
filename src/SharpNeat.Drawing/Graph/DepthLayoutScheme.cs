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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SharpNeat.Graphs;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.Drawing.Graph
{
    // TODO: Run tests. This code is completely new and untested.


    /// <summary>
    /// An <see cref="IGraphLayoutScheme"/> that arranges/positions nodes into layers, based on their depth in the network.
    /// </summary>
    /// <remarks>
    /// 
    /// TODO: Revise/edit these remarks.
    /// 
    /// Layout nodes based on their depth within the network.
    /// 
    /// Note 1.
    /// Input nodes are defined as being at layer zero, and we position them in their own layer at the 
    /// top of the layout area. Any other type of node (hidden or output) not connected to is also
    /// defined as being at layer zero, if such nodes exist then we place them into their own layout 
    /// layer to visually separate them from the input nodes.
    /// 
    /// Note 2.
    /// Output nodes can be at a range of depths, but for clarity we position them all in their own layer 
    /// at the bottom of the layout area. A hidden node can have a depth greater than or equal to one or 
    /// more of the output nodes, to handle this case neatly we ensure that the output nodes are always 
    /// in a layer by themselves by creating an additional layer in the layout if necessary.
    ///
    /// Note 3.
    /// Hidden nodes are positioned into layers between the inputs and outputs based on their depth.
    /// 
    /// Note 4. 
    /// Empty layers are not possible in the underlying network because for there to be a layer N node it 
    /// must have a connection from a layer N-1 node. However, in cyclic networks the output nodes can be
    /// source nodes, but we also always paint output nodes in their own layout layer at the bottom of the
    /// layout area. Therefore if the only node at a given depth is an output node then the layout layer 
    /// can be empty. To handle this neatly we check for empty layout layers before invoking the layout logic.
    /// 
    /// </remarks>
    public class DepthLayoutScheme : IGraphLayoutScheme
    {
        readonly CyclicGraphDepthAnalysis _cyclicDepthAnalysis = new CyclicGraphDepthAnalysis();

        #region Public Methods

        /// <summary>
        /// Layout the nodes of the provided directed in a 2D area specified by <paramref name="layoutArea"/>.
        /// </summary>
        /// <param name="digraph">The directed graph to be laid out.</param>
        /// <param name="layoutArea">The area to layout nodes within.</param>
        /// <param name="nodePosByIdx">A span that will be populated with a 2D position for each node, within the provided layout area.</param>
        public void Layout(
            DirectedGraph digraph,
            Size layoutArea,
            Span<Point> nodePosByIdx)
        {
            if(nodePosByIdx.Length != digraph.TotalNodeCount) throw new ArgumentException(nameof(nodePosByIdx));

            // Build an array that gives the node layer for each node, keyed by node index.
            int[] nodeLayerByIdx = BuildNodeLayerByIdx(digraph);

            // Group nodes into layers.
            int layerCount = nodeLayerByIdx.Max() + 1;
            var nodesByLayer = new List<int>[layerCount];
            for(int i=0; i < layerCount; i++) {
                nodesByLayer[i] = new List<int>();
            }

            for(int nodeIdx=0; nodeIdx < nodeLayerByIdx.Length; nodeIdx++)
            {
                int depth = nodeLayerByIdx[nodeIdx];
                nodesByLayer[depth].Add(nodeIdx);
            }

            // Layout the nodes within the 2D layout area.
            LayoutNodes(layoutArea, nodesByLayer, nodePosByIdx);
        }

        #endregion

        #region Private Methods (nix of instance and static methods)

        private static void LayoutNodes(
            Size layoutArea,
            List<int>[] nodesByLayer,
            Span<Point> nodePosByIdx)
        {
            // Layout the nodes.
            // Calculate height of each layer. We divide the layout area height by the number of layers, but also 
            // define margins at the top and bottom of the view. To make best use of available height the margins 
            // are defined as a proportion of the layer height. Hence we have:
            //
            // ============================
            // d - network depth.
            // l - layer height.
            // m - margin height (same height used for top and bottom margins).
            // p - proportion of layer height that gives margin height.
            // H - layout areas height
            // ============================
            //
            // Derivation:
            // ============================ 
            // 1) l = (H-2m) / (d-1)
            //
            // 2) m = lp, therefore
            //
            // 3) l = (H-2pl) / (d-1), solve for l
            //
            // 4) l = H/(d-1) - 2pl/(d-1)
            //
            // 5) 1 = H/(d-1)l - 2p/(d-1)
            //
            // 6) H / (d-1)l = 1 + 2p/(d-1), inverting both sides gives
            //
            // 7) (d-1)l / H = 1/[ 1 + 2p/(d-1) ]
            //
            // 8)            = (d-1) / ((d-1) + 2p)
            //
            // 9)            = (d-1) / (d + 2p - 1), rearranging for l gives
            //
            // 10) l = H / (d + 2p - 1)
            const float p = 2f;
            const float p2 = 2f * p;
            // Rounding will produce 'off by one' errors, e.g. all heights may not total height of layout area,
            // but the effect is probably negligible on the quality of the layout.
            int layerCount = nodesByLayer.Length;
            int l = (int)Math.Round((float)layoutArea.Height / ((float)layerCount + p2 - 1f));
            int m = (int)Math.Round(l * p);

            // Assign a position to each node, one layer at a time.
            int yCurrent = m;
            foreach(List<int> nodeList in nodesByLayer)
            {
                // Calculate inter-node gap and margin width (we use the same principle as with the vertical layout of layers, see notes above).
                int nodeCount = nodeList.Count;
                float xIncr = (float)layoutArea.Width / ((float)nodeCount + p2 - 1f);
                int xMargin = (int)Math.Round(xIncr * p);

                // Loop nodes in layer; Assign position to each.
                float xCurrent = xMargin;
                for(int nodeIdx=0; nodeIdx < nodeCount; nodeIdx++, xCurrent += xIncr) 
                {
                    nodePosByIdx[nodeIdx] = new Point((int)xCurrent, yCurrent);
                }

                // Increment y coord for next layer.
                yCurrent += l;
            }
        }

        private int[] BuildNodeLayerByIdx(DirectedGraph digraph)
        {
            // Invoke the appropriate subroutine for cyclic/acyclic graphs.
            if(digraph is DirectedGraphAcyclic digraphAcyclic)
            {
                return BuildNodeLayerByIdx_Acyclic(digraphAcyclic);
            }
            else
            {
                return BuildNodeLayerByIdx_Cyclic(digraph);
            }
        }

        /// <summary>
        /// Create an array that gives the node layer for each node, keyed by node index.
        /// </summary>
        /// <param name="digraphAcyclic">The directed acyclic graph.</param>
        /// <returns>A new integer array</returns>
        private static int[] BuildNodeLayerByIdx_Acyclic(DirectedGraphAcyclic digraphAcyclic)
        {
            // Alloc the array, and populate with the already determined node depth values as represented
            // by the layer info provided by DirectedGraphAcyclic.
            int nodeCount = digraphAcyclic.TotalNodeCount;
            int[] nodeLayerByIdx = new int[nodeCount];

            // Use a restricted scope for the loop variables.
            { 
                // The first layer is layer 1; layer zero will be used later to represent input nodes only.
                int layerIdx = 1;
                int nodeIdx = 0;
                foreach(LayerInfo layerInfo in digraphAcyclic.LayerArray)
                {
                    for(; nodeIdx < layerInfo.EndNodeIdx; nodeIdx++) {
                        nodeLayerByIdx[nodeIdx] = layerIdx;
                    }
                    layerIdx++;
                }
            }

            // Assign input nodes to their own layer (layer zero).
            for(int i=0; i < digraphAcyclic.InputCount; i++) {
                nodeLayerByIdx[i] = 0;
            }

            // Assign output nodes to their own layer.
            int outputLayerIdx = digraphAcyclic.LayerArray.Length + 1;
            foreach(int outputNodeIdx in digraphAcyclic.OutputNodeIdxArr) {
                nodeLayerByIdx[outputNodeIdx] = outputLayerIdx;
            }

            // Remove empty layers (if any), by adjusting the depth values in nodeLayerByIdx.
            RemoveEmptyLayers(nodeLayerByIdx, outputLayerIdx + 1);

            // Return the constructed node layer lookup table.
            return nodeLayerByIdx;
        }

        /// <summary>
        /// Create an array that gives the node layer for each node, keyed by node index.
        /// </summary>
        /// <param name="digraph">The directed cyclic graph.</param>
        /// <returns>A new integer array</returns>
        private int[] BuildNodeLayerByIdx_Cyclic(DirectedGraph digraph)
        {
            // Perform an analysis on the cyclic graph to assign a depth to each node.
            GraphDepthInfo depthInfo = _cyclicDepthAnalysis.CalculateNodeDepths(digraph);
            int[] nodeLayerByIdx = depthInfo._nodeDepthArr;

            // Move all nodes up one layer, such that layer 1 is the first/top layer; layer zero will be 
            // used later to represent input nodes only.
            Array.ForEach(nodeLayerByIdx, x => x++);

            // Assign input nodes to their own layer (layer zero).
            for(int i=0; i < digraph.InputCount; i++) {
                nodeLayerByIdx[i] = 0;
            }

            // Assign output nodes to their own layer.
            int outputLayerIdx = depthInfo._graphDepth + 1;
            for(int i=0; i < digraph.OutputCount; i++)
            {
                // Note. For cyclic networks the output node indexes occur in a contiguous segment following the input nodes.
                int outputNodeIdx = digraph.InputCount + i;
                nodeLayerByIdx[outputNodeIdx] = outputLayerIdx;
            }

            // Remove empty layers (if any), by adjusting the depth values in nodeLayerByIdx.
            RemoveEmptyLayers(nodeLayerByIdx, outputLayerIdx + 1);
            
            // Return the constructed node layer lookup table.
            return nodeLayerByIdx;
        }

        private static void RemoveEmptyLayers(Span<int> nodeLayerByIdx, int layerCount)
        {
            // Count how many nodes there are in each layer.
            Span<int> nodeCountByLayer = stackalloc int[layerCount];

            foreach(int layerIdx in nodeLayerByIdx) {
                nodeCountByLayer[layerIdx]++;
            }

            // Create a mapping from old to new layer indexes, and init with the identity mapping.
            Span<int> layerIdxMap = stackalloc int[nodeCountByLayer.Length];
            for(int i=0; i < layerIdxMap.Length; i++) {
                layerIdxMap[i] = i;
            }

            // Loop through nodeCountByLayer backwards, testing for empty layers, and removing them as we go.
            // Removal here means adjusting layerIdxMap.
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

            // Apply the node layer index mappings we have just constructed; this 'moves' the nodes to their new layers, i.e. to remove/collapse
            // the empty layers (if any).
            for(int i=0; i < nodeLayerByIdx.Length; i++) {
                nodeLayerByIdx[i] = layerIdxMap[nodeLayerByIdx[i]];
            }
        }

        #endregion
    }
}
