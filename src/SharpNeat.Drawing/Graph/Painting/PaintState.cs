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

namespace SharpNeat.Drawing.Graph.Painting
{
    /// <summary>
    /// Represents data required for by painting routines.
    /// </summary>
    public sealed class PaintState
    {
        #region Instance Fields

        /// <summary>The current GDI+ painting surface.</summary>
        public readonly Graphics _g;

        /// <summary>The area being painted to. Any elements outside of this area are not visible.</summary>
        public readonly Rectangle _viewportArea;

        /// <summary>Scales the elements being drawn.</summary>
        public readonly float _zoomFactor;

        /// <summary>Range of connections weights. Used to determine width of drawn connections.</summary>
        public readonly float _connectionWeightRange;

        /// <summary>Use in conjunction with _connectionWeightRange to draw connections.</summary>
        public readonly float _connectionWeightRangeHalf;

        /// <summary>Uses in conjunction with _connectionWeightRange to draw connections.</summary>
        public readonly float _connectionWeightToWidth;

        // Useful derived values.
        /// <summary>Diameter of drawn nodes.</summary>
        public readonly int _nodeDiameter;

        /// <summary>Used in conjunction with _nodeDiameter to draw nodes.</summary>
        public readonly int _nodeDiameterHalf;

        /// <summary>Length of connection legs emanating from the base of nodes when drawing connections
        /// to nodes above the source node.</summary>
        public readonly float _backConnectionLegLength;

        // TODO: Key on node ID instead of 'object'.
        /// <summary>
        /// Dictionary containing temporary painting related state for each graph node.
        /// </summary>
        public Dictionary<GraphNode,ConnectionPointInfo> _nodeStateDict;

        #endregion

        #region Construction

        /// <summary>
        /// Construct with the provided Graphics painting surface and state data.
        /// </summary>
        public PaintState(
            Graphics g, Rectangle viewportArea,
            float zoomFactor, float connectionWeightRange,
            int nodeCount)
        {
            // Store state variables.
            _g = g;
            _viewportArea = viewportArea;
            _zoomFactor = zoomFactor;
            _connectionWeightRange = connectionWeightRange;
            _connectionWeightRangeHalf = connectionWeightRange * 0.5f;
            _connectionWeightToWidth = (float)(2.0 / Math.Log10(connectionWeightRange + 1.0));

            // Precalculate some useful derived values.
            _nodeDiameter = (int)(GraphPaintingConsts.NodeDiameterModel * zoomFactor);
            _nodeDiameterHalf = (int)((GraphPaintingConsts.NodeDiameterModel * zoomFactor) * 0.5f);
            _backConnectionLegLength = _nodeDiameter * 1.6f;

            // Create per-node state info map.
            _nodeStateDict = new Dictionary<GraphNode,ConnectionPointInfo>(nodeCount);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the state object for a given graph node. Creates the object if it does not yet exist.
        /// </summary>
        public ConnectionPointInfo GetNodeStateInfo(GraphNode node)
        {
            if(!_nodeStateDict.TryGetValue(node, out ConnectionPointInfo? info))
            {
                info = new ConnectionPointInfo();
                _nodeStateDict.Add(node, info);
            }
            return info;
        }

        #endregion
    }
}
