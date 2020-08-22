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
using SharpNeat.Graphs;

namespace SharpNeat.Drawing
{
    /// <summary>
    /// Represents a directed graph, with supplementary data suitable for producing a 2D visual representation of the graph.
    /// </summary>
    public class DirectedGraphViewModel
    {
        /// <summary>
        /// Represents the directed graph topology.
        /// </summary>
        public DirectedGraph DirectedGraph { get; }

        /// <summary>
        /// Graph connection/vertex weights.
        /// </summary>
        public float[] WeightArr { get; }

        /// <summary>
        /// Provides nodes IDs for each node. These can be though of as labels that are assigned to each node, 
        /// suitable for showing against each node in a visual representation of the graph.
        /// </summary>
        public int[] NodeIdByIdx { get; }

        /// <summary>
        /// Provides a depth/layer for each node. This is used to visually layout the nodes of the graph in a 2D visual space.
        /// </summary>
        public int[] NodeLayerByIdx { get; }

        /// <summary>
        /// Gets the number of node layers for visualisation purposes.
        /// </summary>
        /// <remarks>
        /// This may be different to the graph depth because input and output nodes are arranged into their own visual layers,
        /// even though output nodes may be at any depth/layer in the logical graph.
        /// </remarks>
        public int LayerCount { get; }

        /// <summary>
        /// Provides a node 2D position for each node.
        /// </summary>
        public Point[]? NodePosByIdx { get; }

        /// <summary>
        /// Construct with the provided digraph, and supplementary data suitable for producing a 2D visual representation of the graph.
        /// </summary>
        /// <param name="digraph">Directed graph.</param>
        /// <param name="weightArr">Graph connection/vertex weights.</param>
        /// <param name="nodeIdByIdx">Node IDs/labels.</param>
        /// <param name="nodeLayerByIdx">Node layer indexes.</param>
        /// <param name="layerCount">The number of layout layers.</param>
        public DirectedGraphViewModel(
            DirectedGraph digraph,
            float[] weightArr,
            int[] nodeIdByIdx,
            int[] nodeLayerByIdx,
            int layerCount)
        {
            this.DirectedGraph = digraph;
            this.WeightArr = weightArr;
            this.NodeIdByIdx = nodeIdByIdx;
            this.NodeLayerByIdx = nodeLayerByIdx;
            this.LayerCount = layerCount;

            // TODO: Validation.
        }
    }
}
