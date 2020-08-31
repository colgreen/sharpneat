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
using System.Drawing;
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

        readonly IGraphLayoutScheme _layoutScheme;

        #endregion

        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DirectedGraphViewModelFactory()
        {
            _layoutScheme = new DepthLayoutScheme();
        }

        /// <summary>
        /// Construct with the provided layout scheme.
        /// </summary>
        /// <param name="layoutScheme">The graph layout scheme to use.</param>
        public DirectedGraphViewModelFactory(IGraphLayoutScheme layoutScheme)
        {
            _layoutScheme = layoutScheme ?? throw new ArgumentNullException(nameof(layoutScheme));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a <see cref="DirectedGraphViewModel"/> that represent the provided directed graph.
        /// </summary>
        /// <param name="digraph">The directed graph to construct the model for.</param>
        /// <param name="weightArr">The graph's connection/edge weights.</param>
        /// <param name="nodeIdByIdx">An array that provides a node ID value for each node, i.e. keyed by node index..</param>
        /// <param name="layoutArea">The area to layout nodes within.</param>
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
            Size layoutArea)
        {
            // Invoke the layout scheme to determine a 2D position for each node in the graph.
            var nodePosByIdx = new Point[digraph.TotalNodeCount];
            _layoutScheme.Layout(digraph, layoutArea, nodePosByIdx);

            // Construct a new DirectedGraphViewModel and return.
            return new DirectedGraphViewModel(digraph, weightArr, nodeIdByIdx, nodePosByIdx);
        }

        #endregion
    }
}
