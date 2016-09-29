/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
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

namespace SharpNeat.View.Graph
{
    /// <summary>
    /// An ILayoutManager that positions nodes in layers based on their depth in the network.
    /// </summary>
    public class DepthLayoutManager : ILayoutManager
    {
        /// <summary>
        /// Layout nodes based on their depth within the network.
        /// 
        /// Note 1.
        /// Input nodes are defined as being at layer zero and we position them in their own layer at the 
        /// top of the layout area. Any other type of node (hidden or output) node not connected to is also
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
        /// </summary>
        /// <param name="graph">The network/graph structure to be laid out.</param>
        /// <param name="layoutArea">The area the structure is to be laid out on.</param>
        public void Layout(IOGraph graph, Size layoutArea)
        {
            int inputCount = graph.InputNodeList.Count;
            int outputCount = graph.OutputNodeList.Count;
            int hiddenCount = graph.HiddenNodeList.Count;

        //=== Stratify all nodes into layout layers, each layer keyed by depth value.
            SortedDictionary<int,List<GraphNode>> layersByDepth = new SortedDictionary<int,List<GraphNode>>();

            // Special case. Key all input nodes by depth -1. This places them as the first layer and distinct 
            // from all other nodes.
            layersByDepth.Add(-1, graph.InputNodeList);

            // Stratify hidden nodes into layers.
            // Count hidden nodes in each layer.
            int[] hiddenNodeCountByLayerArr = new int[graph.Depth];
            for(int i=0; i<hiddenCount; i++) {
                hiddenNodeCountByLayerArr[graph.HiddenNodeList[i].Depth]++;
            }

            // Allocate storage per layer.
            for(int i=0; i<graph.Depth; i++) 
            {
                int nodeCount = hiddenNodeCountByLayerArr[i];
                if(0 != nodeCount) {
                    layersByDepth.Add(i, new List<GraphNode>(nodeCount));
                }
            }

            // Put hidden nodes into layer lists.
            for(int i=0; i<hiddenCount; i++) 
            {
                GraphNode node = graph.HiddenNodeList[i];
                layersByDepth[node.Depth].Add(node);
            }

            // Special case 2. Place output nodes in their own distinct layer at a depth one more than the max
            // depth of the network. This places them as the last layer and distinct from all other nodes.
            layersByDepth.Add(graph.Depth, graph.OutputNodeList);

        //=== Layout.
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
            const float p = 0.33f;
            const float p2 = 2f * p;
            // Rounding will produce 'off by one' errors, e.g. all heights may not total height of layout area,
            // but the effect is probably negligible on the quality of the layout.
            int layoutLayerCount = layersByDepth.Count;
            int l = (int)Math.Round((float)layoutArea.Height / ((float)layoutLayerCount + p2 - 1f));
            int m = (int)Math.Round(l * p);

            // Size struct to keep track of the area that bounds all nodes.
            Size bounds = new Size(0,0);

            // Assign a position to each node, one layer at a time.
            int yCurrent = m;
            foreach(List<GraphNode> layerNodeList in layersByDepth.Values)
            {
                // Calculate inter-node gap and margin width (we use the same principle as with the vertical layout of layers, see notes above).
                int nodeCount = layerNodeList.Count;
                float xIncr = (float)layoutArea.Width / ((float)nodeCount + p2 - 1f);
                int xMargin = (int)Math.Round(xIncr * p);

                // Loop nodes in layer; Assign position to each.
                float xCurrent = xMargin;
                for(int nodeIdx=0; nodeIdx < nodeCount; nodeIdx++, xCurrent += xIncr) 
                {
                    layerNodeList[nodeIdx].Position = new Point((int)xCurrent, yCurrent);
                    UpdateModelBounds(layerNodeList[nodeIdx], ref bounds);
                }

                // Increment y coord for next layer.
                yCurrent += l;
            }

            // Store the bounds of the graph elements. Useful when drawing the graph.
            graph.Bounds = bounds;
        }

        private void UpdateModelBounds(GraphNode node, ref Size bounds)
        {
            if(node.Position.X > bounds.Width) {
                bounds.Width = node.Position.X;
            }
            if(node.Position.Y > bounds.Height) {
                bounds.Height = node.Position.Y;
            }
        }
    }
}
