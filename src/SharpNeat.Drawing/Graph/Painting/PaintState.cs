/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2022 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Drawing;

namespace SharpNeat.Drawing.Graph.Painting;

/// <summary>
/// A collection of working variables for painting a graph to a GDI+ surface.
/// </summary>
public sealed class PaintState
{
    #region Instance Fields

    /// <summary>
    /// The GDI+ painting surface.
    /// </summary>
    public readonly Graphics _g;

    /// <summary>
    /// The area being painted to. Any elements outside of this area are not painted.
    /// </summary>
    public readonly Rectangle _viewportArea;

    /// <summary>
    /// Scales the elements being drawn.
    /// </summary>
    public readonly float _zoomFactor;

    /// <summary>
    /// Diameter of drawn nodes.
    /// </summary>
    public readonly int _nodeDiameter;

    /// <summary>
    /// Used in conjunction with _nodeDiameter to draw nodes.
    /// </summary>
    public readonly int _nodeDiameterHalf;

    /// <summary>
    /// Length of connection legs emanating from the base of nodes when drawing 'back' connections,
    /// i.e., to target nodes above the source node.
    /// </summary>
    public readonly float _backConnectionLegLength;

    /// <summary>
    /// Connection point info per node. Used to track how many back-connections have been attached to nodes,
    /// so that new back-connections can be drawn without overlapping already drawn back-connections.
    /// </summary>
    public readonly ConnectionPointInfo[] _nodeStateByIdx;

    #endregion

    #region Construction

    /// <summary>
    /// Construct with the provided Graphics painting surface and painting metadata.
    /// </summary>
    /// <param name="g">The GDI+ painting surface.</param>
    /// <param name="viewportArea">The area being painted to. Any elements outside of this area are not painted.</param>
    /// <param name="nodeDiameter">Diameter of drawn nodes.</param>
    /// <param name="zoomFactor">Painting zoom/scaling factor.</param>
    /// <param name="nodeCount">The number of nodes being painted.</param>
    public PaintState(
        Graphics g, Rectangle viewportArea,
        float nodeDiameter,
        float zoomFactor, int nodeCount)
    {
        // Store state variables.
        _g = g;
        _viewportArea = viewportArea;
        _zoomFactor = zoomFactor;

        // Pre-calculate some useful derived values.
        _nodeDiameter = (int)(nodeDiameter * zoomFactor);
        _nodeDiameterHalf = (int)((nodeDiameter * zoomFactor) * 0.5f);
        _backConnectionLegLength = _nodeDiameter * 1.6f;

        // Create per-node state info map.
        _nodeStateByIdx = new ConnectionPointInfo[nodeCount];
    }

    #endregion
}
