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

namespace SharpNeat.Drawing.Graph
{
    /// <summary>
    /// An <see cref="IViewportPainter"/> that paints <see cref="IOGraph"/> objects.
    /// </summary>
    public class IOGraphViewportPainter : IViewportPainter
    {
        readonly ILayoutScheme _layoutManager;
        readonly IOGraphPainter _graphPainter;

        /// <summary>
        /// Gets or sets the IOGraph to paint.
        /// </summary>
        public IOGraph IOGraph { get; set; }

        #region Constructor

        /// <summary>
        /// Constructs with the provided graph painter and a default layout manager.
        /// </summary>
        public IOGraphViewportPainter(IOGraphPainter graphPainter)
        {
            IOGraph = null;
            _layoutManager = new DepthLayoutScheme();
            _graphPainter = graphPainter;
        }

        /// <summary>
        /// Constructs with the provided graph painter and layout manager.
        /// </summary>
        public IOGraphViewportPainter(IOGraphPainter graphPainter, ILayoutScheme layoutManager)
        {
            IOGraph = null;
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
            if(IOGraph is object) 
            {
                _layoutManager.Layout(IOGraph, viewportArea.Size);
                _graphPainter.PaintNetwork(IOGraph, g, viewportArea, zoomFactor);
            }
        }

        #endregion
    }
}
