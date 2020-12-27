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
using SharpNeat.Drawing.Graph.Painting;

namespace SharpNeat.Drawing.Graph
{
    /// <summary>
    /// Paints graphs to a viewport.
    /// </summary>
    /// <remarks>
    /// Graph painting involves two main high level steps
    /// 1) laying out the nodes of the graph within the specified paint area.
    /// 2) Painting the nodes and connections based on the node positions assigned in step 1.
    /// </remarks>
    public sealed class GraphViewportPainter : IViewportPainter
    {
        static readonly Brush _brushBackground = new SolidBrush(Color.White);

        readonly IGraphLayoutScheme _layoutScheme;
        readonly GraphPainter _graphPainter;

        // The current graph view model being painted.
        DirectedGraphViewModel? _graphViewModel;

        // Describes the viewport area the graph has been laid out within. The initial size of -1, -1 indicates no layout has yet occurred.
        Rectangle _viewportArea = new Rectangle(0, 0, -1, -1);
        object? _layoutSchemeData;

        #region Construction

        /// <summary>
        /// Construct with a default layout manager and graph painter.
        /// </summary>
        public GraphViewportPainter()
        {
            _layoutScheme = new DepthLayoutScheme();
            _graphPainter = new GraphPainter();
        }

        /// <summary>
        /// Construct with the provided layout manager and graph painter.
        /// </summary>
        public GraphViewportPainter(
            IGraphLayoutScheme layoutScheme,
            GraphPainter graphPainter)
        {
            _layoutScheme = layoutScheme;
            _graphPainter = graphPainter;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the graph view model to paint.
        /// </summary>
        public DirectedGraphViewModel? GraphViewModel
        {
            get => _graphViewModel;
            set
            {
                _graphViewModel = value;

                // Reset viewportArea to indicate no layout of the new graph has yet occurred.
                _viewportArea = new Rectangle(0, 0, -1, -1);

                // Reset any layout data created by the scheme, as this would have been specific to the previous model.
                _layoutSchemeData = null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Paint the current graph to the provided GDI+ drawing surface.
        /// </summary>
        /// <param name="g">The GDI+ drawing surface to paint on to.</param>
        /// <param name="viewportArea">The area to paint within.</param>
        /// <param name="zoomFactor">Zoom factor.</param>
        public void Paint(Graphics g, Rectangle viewportArea, float zoomFactor)
        {
            // Paint background as a solid colour.
            g.FillRectangle(_brushBackground, viewportArea);

            // Get a local stack reference to the class variable; this avoid the chance of another thread changing the model object from this point onwards.
            var model = this.GraphViewModel;

            // Skip painting if nothing to paint.
            if(model is null) {
                return;
            }

            // Determine if the graph requires laying out within the specified viewportArea, i.e. if the previous call to Paint()
            // supplied the same viewportArea, then we can just use the layout positions determined previously.
            if(viewportArea != _viewportArea)
            {
                // Layout the graph within the required viewport area, and record the area we used.
                LayoutGraph(model, viewportArea);
                _viewportArea = viewportArea;
            }

            // Paint the graph.
            _graphPainter.PaintGraph(model, g, viewportArea, zoomFactor);
        }

        #endregion

        #region Private Methods

        private void LayoutGraph(DirectedGraphViewModel model, Rectangle viewportArea)
        {
            // Determine 2D position for each node in the graph, within the viewport area/rectangle.
            _layoutScheme.Layout(model.DirectedGraph, viewportArea.Size, model.NodePosByIdx, ref _layoutSchemeData);

            // Translate the points if the viewport origin is not (0,0); the layout schemes assume an origin of (0,0).
            if(viewportArea.Location != Point.Empty)
            {
                Point origin = viewportArea.Location;
                Point[] nodePosByIdx = model.NodePosByIdx;

                for(int i=0; i < model.NodePosByIdx.Length; i++) {
                    nodePosByIdx[i].Offset(origin);
                }
            }
        }

        #endregion
    }
}
