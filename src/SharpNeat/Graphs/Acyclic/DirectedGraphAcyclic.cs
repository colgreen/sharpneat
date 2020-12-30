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

namespace SharpNeat.Graphs.Acyclic
{
    /// <summary>
    /// Represents an acyclic directed graph.
    /// </summary>
    public class DirectedGraphAcyclic : DirectedGraph
    {
        /// <summary>
        /// Layer information for the acyclic graph.
        /// </summary>
        /// <remarks>
        /// The nodes of the graph are arranged into layers, where the layer index (or depth) for a node is defined as the longest path
        /// (number of node to node hops) to arrive at that node from an input node, thus input nodes are defined as being in layer zero
        /// (depth 0).
        /// </remarks>
        public LayerInfo[] LayerArray { get; }

        /// <summary>
        /// An array containing the node index of each output node.
        /// </summary>
        /// <remarks>
        /// In acyclic networks the output and hidden nodes are re-ordered by network depth. This array describes
        /// the index of each output node in the full set of nodes. Note however that the input nodes *are* in
        /// their original positions, as they are defined as being at depth zero and therefore are not moved by
        /// the depth based sort.
        /// </remarks>
        public int[] OutputNodeIdxArr { get; }

        #region Constructor

        /// <summary>
        /// Construct with the given node counts, connection data, layer information, and indexes of the output nodes.
        /// </summary>
        /// <param name="inputCount">Input node count.</param>
        /// <param name="outputCount">Output node count.</param>
        /// <param name="totalNodeCount">Total node count.</param>
        /// <param name="connIdArrays">The connection source and target node IDs.</param>
        /// <param name="layerArr">Layer information for the acyclic graph.</param>
        /// <param name="outputNodeIdxArr">An array containing the node index of each output node.</param>
        internal DirectedGraphAcyclic(
            int inputCount,
            int outputCount,
            int totalNodeCount,
            in ConnectionIdArrays connIdArrays,
            LayerInfo[] layerArr,
            int[] outputNodeIdxArr)
        : base(inputCount, outputCount, totalNodeCount, in connIdArrays)
        {
            this.LayerArray = layerArr;
            this.OutputNodeIdxArr = outputNodeIdxArr;
        }

        #endregion
    }
}
