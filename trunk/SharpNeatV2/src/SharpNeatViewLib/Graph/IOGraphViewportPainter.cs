/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
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
            _layoutManager = new GridLayoutManager();
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
