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
using System.Drawing;
using Redzen;
using Redzen.Collections;
using SharpNeat.Graphs;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.Drawing.Graph;

/// <summary>
/// An <see cref="IGraphLayoutScheme"/> that arranges/positions nodes into layers, based on their depth in the network.
/// </summary>
/// <remarks>
/// === Summary/Overview ===
/// Lay out nodes based on their depth within the graph/network.
///
/// The nodes of the graph are assigned a node depth. All nodes at the same depth are defined as being in the same layer,
/// and the nodes of each layer are positioned in a horizontal row, with the depth zero row being placed at the top of
/// the layout area, and the final layer being positioned at the bottom.
///
///
/// === Input and Output Layers ===
/// The actual layers used are slightly different to the scheme described in the summary. The input nodes and output nodes
/// of a graph are always laid out in their own rows at the top and bottom of the layout area, respectively. Hence, any
/// hidden nodes defined as being at depth zero (i.e. no incoming connections) are actually positioned in the second layer.
/// Likewise, hidden nodes with depths greater than or equal to any node in the output layer are positioned in he layer
/// before the output layer.
///
/// Essentially there are two 'virtual rows' for input and outputs, and the hidden nodes are arranged in between based on
/// their node depth.
///
///
/// === Node Depths ===
/// For acyclic graphs the node depths are already determined and stored in the DirectedGraph data structure (specifically in
/// the subclass DirectedGraphAcyclic). This depth info is necessary for using the acyclic graphs (i.e. propagating a
/// signal through the graph, from the input nodes through to he output nodes) and is based on the maximum number of hops
/// to a given node, starting from the input layer.
///
/// For cyclic graphs this layout scheme calculates node depths using a scheme similar to that used for the acyclic graphs,
/// but with modifications to handle cyclic connections. In this scheme the depth of nodes with multiple incoming connections,
/// some or all of which may be part of a cycle, is based on the average number of hops to that node. This is essentially a
/// heuristic that aims to place nodes 'naturally', i.e. with connections mostly in nearby layers (on average).
/// </remarks>
public sealed class DepthLayoutScheme : IGraphLayoutScheme
{
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
        if(nodePosByIdx.Length != digraph.TotalNodeCount) throw new ArgumentException("Node positions array length doesn't match the number of nodes in the graph.", nameof(nodePosByIdx));

        // Determine depth of each node.
        LightweightList<int>[] nodesByLayer = BuildNodesByLayer(digraph);

        // Layout the nodes within the 2D layout area.
        LayoutNodes(layoutArea, nodesByLayer, nodePosByIdx);
    }

    /// <summary>
    /// Layout the nodes of the provided directed in a 2D area specified by <paramref name="layoutArea"/>.
    /// </summary>
    /// <param name="digraph">The directed graph to be laid out.</param>
    /// <param name="layoutArea">The area to layout nodes within.</param>
    /// <param name="nodePosByIdx">A span that will be populated with a 2D position for each node, within the provided layout area.</param>
    /// <param name="layoutSchemeData">An optional object that conveys layout data specific to the layout scheme in use.</param>
    public void Layout(
        DirectedGraph digraph,
        Size layoutArea,
        Span<Point> nodePosByIdx,
        ref object? layoutSchemeData)
    {
        if(nodePosByIdx.Length != digraph.TotalNodeCount) throw new ArgumentException("Node positions array length doesn't match the number of nodes in the graph.", nameof(nodePosByIdx));

        // Use previously determined depth info, if provided; otherwise calculate it and return via layoutSchemeData
        // parameter for future use.
        LightweightList<int>[] nodesByLayer;
        if(layoutSchemeData is DepthLayoutSchemeData schemeData)
        {
            nodesByLayer = schemeData.NodesByLayer;
        }
        else
        {
            nodesByLayer = BuildNodesByLayer(digraph);
            layoutSchemeData = new DepthLayoutSchemeData(nodesByLayer);
        }

        // Layout the nodes within the 2D layout area.
        LayoutNodes(layoutArea, nodesByLayer, nodePosByIdx);
    }

    #endregion

    #region Private Static Methods

    /// <summary>
    /// Layout nodes based on their depth in the graph, i.e. a layer index that has been pre-calculated.
    /// </summary>
    /// <param name="layoutArea">The area to layout nodes within.</param>
    /// <param name="nodesByLayer">An array of lists. One list per layer, and containing the indexes of the nodes in that layer.</param>
    /// <param name="nodePosByIdx">A span that will be populated with a 2D position for each node, within the provided layout area.</param>
    private static void LayoutNodes(
        Size layoutArea,
        LightweightList<int>[] nodesByLayer,
        Span<Point> nodePosByIdx)
    {
        // ============================
        //
        // Layout variables and method.
        // ============================
        //
        // my_top     Vertical (y-axis) margin (top)
        // my_bottom  Vertical (y-axis) margin (bottom)
        // mx         Horizontal (x-axis) margin (left/right)
        //
        // g    Vertical distance between adjacent horizontal layers.
        // u    Layout width, minus margins, i.e. the horizontal range that nodes in each layer can occupy.
        // v    Horizontal distance between adjacent nodes in a horizontal layer.

        // Calculate top/bottom margins.
        // Each margin consists of a fixed amount, plus a proportional component based on the height of the layout area.
        // The fixed amounts are different because the bottom layer of nodes have connections drawn below them, hence
        // additional margin is required there.
        int my_prop = (int)(layoutArea.Height * 0.05f);
        int my_top = 5 + my_prop;
        int my_bottom = 28 + my_prop;
        int my_total = my_top + my_bottom;

        // Calc left/right margins.
        int mx_prop = (int)(layoutArea.Width * 0.02f);
        int mx = 20 + mx_prop;
        int mx2 = 2 * mx;

        // Calculate g, i.e. the vertical distance between adjacent horizontal layers.
        int layerCount = nodesByLayer.Length;
        int g = (int)MathF.Round((layoutArea.Height - my_total) / (float)(layerCount - 1));

        // Define a running y coordinate for positioning of nodes in a horizontal layer. This starts at the top margin
        int ycurr = my_top;

        // Calculate u, i.e. the horizontal range that nodes in each layer can occupy.
        float u = layoutArea.Width - mx2;

        // Loop over the graph layers, positioning the nodes in each layer in turn.
        foreach(LightweightList<int> nodeList in nodesByLayer)
        {
            var nodeSpan = nodeList.AsSpan();

            // Calculate v, i.e. the horizontal distance between adjacent nodes in the current layer.
            int n = nodeSpan.Length;
            float v = u / n;

            // Define a running x coordinate for positioning of nodes horizontally within the current layer.
            float xcurr = mx + (v * 0.5f);

            // Loop nodes in layer, assigning an (x,y) position to each.
            for(int i=0; i < n; i++, xcurr += v)
            {
                int nodeIdx = nodeSpan[i];
                nodePosByIdx[nodeIdx] = new Point((int)xcurr, ycurr);
            }

            // Increment layer y coord, ready for the next layer.
            ycurr += g;
        }
    }

    #endregion

    #region Private Static Methods [Node Depth Analysis]

    private static LightweightList<int>[] BuildNodesByLayer(DirectedGraph digraph)
    {
        // Build an array that gives the node layer for each node, keyed by node index.
        int[] nodeLayerByIdx = BuildNodeLayerByIdx(digraph);

        // Group nodes into layers.
        int layerCount = MathSpan.Max(nodeLayerByIdx) + 1;
        var nodesByLayer = new LightweightList<int>[layerCount];
        for(int i=0; i < layerCount; i++)
            nodesByLayer[i] = new LightweightList<int>();

        for(int nodeIdx=0; nodeIdx < nodeLayerByIdx.Length; nodeIdx++)
        {
            int depth = nodeLayerByIdx[nodeIdx];
            nodesByLayer[depth].Add(nodeIdx);
        }
        return nodesByLayer;
    }

    private static int[] BuildNodeLayerByIdx(DirectedGraph digraph)
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
    /// <returns>A new integer array.</returns>
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
                for(; nodeIdx < layerInfo.EndNodeIdx; nodeIdx++)
                    nodeLayerByIdx[nodeIdx] = layerIdx;

                layerIdx++;
            }
        }

        // Assign input nodes to their own layer (layer zero).
        for(int i=0; i < digraphAcyclic.InputCount; i++)
            nodeLayerByIdx[i] = 0;

        // Assign output nodes to their own layer.
        int outputLayerIdx = digraphAcyclic.LayerArray.Length + 1;
        foreach(int outputNodeIdx in digraphAcyclic.OutputNodeIdxArr)
        {
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
    /// <returns>A new integer array.</returns>
    private static int[] BuildNodeLayerByIdx_Cyclic(DirectedGraph digraph)
    {
        // Perform an analysis on the cyclic graph to assign a depth to each node.
        // TODO: Re-use these instances, by maintaining a pool of them that can be 'rented from' and 'returned to' the pool.
        CyclicGraphDepthAnalysis cyclicDepthAnalysis = new();

        GraphDepthInfo depthInfo = cyclicDepthAnalysis.CalculateNodeDepths(digraph);
        int[] nodeLayerByIdx = depthInfo._nodeDepthArr;

        // Move all nodes up one layer, such that layer 1 is the first/top layer; layer zero will be
        // used later to represent input nodes only.
        for(int i=digraph.InputCount; i < nodeLayerByIdx.Length; i++)
            nodeLayerByIdx[i]++;

        // Assign input nodes to their own layer (layer zero).
        for(int i=0; i < digraph.InputCount; i++)
            nodeLayerByIdx[i] = 0;

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

        foreach(int layerIdx in nodeLayerByIdx)
            nodeCountByLayer[layerIdx]++;

        // Create a mapping from old to new layer indexes, and init with the identity mapping.
        Span<int> layerIdxMap = stackalloc int[nodeCountByLayer.Length];
        for(int i=0; i < layerIdxMap.Length; i++)
            layerIdxMap[i] = i;

        // Loop through nodeCountByLayer backwards, testing for empty layers, and removing them as we go.
        // Removal here means adjusting layerIdxMap.
        for(int layerIdx = nodeCountByLayer.Length-1; layerIdx > -1; layerIdx--)
        {
            if(nodeCountByLayer[layerIdx] == 0)
            {
                // Empty layer detected. Decrement all higher layer indexes to fill the gap.
                for(int i = layerIdx+1; i < layerIdxMap.Length; i++)
                    layerIdxMap[i]--;

                // Set the empty layer's layer index to -1, primarily to mark it as not a valid ID (although we don't actually use this
                // anywhere, except maybe for debugging purposes).
                layerIdxMap[layerIdx] = -1;

                // Update/track the number of layers with nodes.
                layerCount--;
            }
        }

        // Apply the node layer index mappings we have just constructed; this 'moves' the nodes to their new layers, i.e. to remove/collapse
        // the empty layers (if any).
        for(int i=0; i < nodeLayerByIdx.Length; i++)
            nodeLayerByIdx[i] = layerIdxMap[nodeLayerByIdx[i]];
    }

    #endregion

    #region Inner Class

    private class DepthLayoutSchemeData
    {
        public LightweightList<int>[] NodesByLayer { get; }

        public DepthLayoutSchemeData(LightweightList<int>[] nodesByLayer)
        {
            this.NodesByLayer = nodesByLayer;
        }
    }

    #endregion
}
