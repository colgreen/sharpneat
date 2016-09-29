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
using System.Drawing;

namespace SharpNeat.View.Graph
{
    /// <summary>
    /// An IViewportPainter that paints IOGraph objects.
    /// </summary>
    public class IOGraphViewportPainter : IViewportPainter
    {
        IOGraph _graph;
        readonly ILayoutManager _layoutManager;
        readonly IOGraphPainter _graphPainter;

        #region Constructor

        /// <summary>
        /// Constructs with the provided graph painter and a default layout manager.
        /// </summary>
        public IOGraphViewportPainter(IOGraphPainter graphPainter)
        {
            _graph = null;
            _layoutManager = new DepthLayoutManager();
            _graphPainter = graphPainter;
        }

        /// <summary>
        /// Constructs with the provided graph painter and layout manager.
        /// </summary>
        public IOGraphViewportPainter(IOGraphPainter graphPainter, ILayoutManager layoutManager)
        {
            _graph = null;
            _layoutManager = layoutManager;
            _graphPainter = graphPainter;
        }

        #endregion

        #region IViewportPainter

        /// <summary>
        /// Paints the wrapped IOGraph onto the specified viewport. Does nothing if no 
        /// IOGraph has been provided.
        /// </summary>
        public void Paint(Graphics g, Rectangle viewportArea, float zoomFactor)
        {
            if(null != _graph) {
                _layoutManager.Layout(_graph, viewportArea.Size);
                _graphPainter.PaintNetwork(_graph, g, viewportArea, zoomFactor);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the IOGraph to paint.
        /// </summary>
        public IOGraph IOGraph
        {
            get { return _graph; }
            set { _graph = value; }
        }

        #endregion
    }
}
