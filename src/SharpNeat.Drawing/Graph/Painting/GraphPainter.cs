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

namespace SharpNeat.Drawing.Graph.Painting
{
    /// <summary>
    /// For painting a directed graph onto a GDI+ drawing surface.
    /// This requires the nodes of the graph to have already been arranged in a 2D
    /// space and thus assigned 2D positions, this can be achieved with an implementation of <see cref="IGraphLayoutScheme"/>.
    /// </summary>
    public class GraphPainter
    {
        #region Instance Fields

        readonly PainterSettings _settings;
        readonly Pen _nodeBorderPen;
        readonly Brush _nodeFillBrush;
        readonly Brush _nodeLabelBrush;
        readonly float _connectionWeightToWidth;

        #endregion

        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GraphPainter()
            : this(new PainterSettings())
        {
        }

        /// <summary>
        /// Construct with the provided painter settings.
        /// </summary>
        /// <param name="settings">Painter settings.</param>
        public GraphPainter(PainterSettings settings)
        {
            _settings = settings;
            _nodeBorderPen = new Pen(settings.NodeBorderColor, 2f);
            _nodeFillBrush = new SolidBrush(settings.NodeFillColor);
            _nodeLabelBrush = new SolidBrush(settings.NodeLabelColor);
            _connectionWeightToWidth = 2f / MathF.Log10(settings.ConnectionWeightRange + 1f);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Paint a directed graph onto the a provided GDI+ surface.
        /// </summary>
        /// <param name="model">The graph view model to paint.</param>
        /// <param name="g">The GDI+ surface to paint on to.</param>
        /// <param name="viewportArea">An area within the GDI+ surface to paint the graph within.</param>
        /// <param name="zoomFactor">Zoom factor.</param>
        public void PaintGraph(
            DirectedGraphViewModel model,
            Graphics g,
            Rectangle viewportArea,
            float zoomFactor)
        {
            PaintState state = new(g, viewportArea, _settings.NodeDiameter, zoomFactor, model.DirectedGraph.TotalNodeCount);
            PaintGraph(model, state);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Paint a directed graph.
        /// </summary>
        /// <param name="model">The graph view model to paint.</param>
        /// <param name="state">A collection of working variables for painting a graph to a GDI+ surface.</param>
        protected virtual void PaintGraph(DirectedGraphViewModel model, PaintState state)
        {
            // Paint all connections, followed by all nodes.
            // This way the slightly 'rough' positioning of the connection endpoints is overpainted by the nodes
            // to produce an overall good visual result.
            PaintConnections(model, state);
            PaintNodes(model, state);
        }

        /// <summary>
        /// Paint the nodes of a directed graph.
        /// </summary>
        /// <param name="model">The graph view model being painted.</param>
        /// <param name="state">A collection of working variables for painting a graph to a GDI+ surface.</param>
        protected virtual void PaintNodes(DirectedGraphViewModel model, PaintState state)
        {
            // Loop the nodes, painting each in turn.
            for(int i=0; i < model.NodeIdByIdx.Count; i++)
            {
                int id = model.NodeIdByIdx.Map(i);
                Point pos = model.NodePosByIdx![i];

                PaintNode(pos, id, state);
            }
        }

        /// <summary>
        /// Paint a single node.
        /// </summary>
        /// <param name="pos">Node position.</param>
        /// <param name="id">Node id/label.</param>
        /// <param name="state">A collection of working variables for painting a graph to a GDI+ surface.</param>
        protected virtual void PaintNode(Point pos, int id, PaintState state)
        {
            pos = ModelToViewport(pos, state);
            if(!IsPointWithinViewport(pos, state))
            {   // Skip node. It's outside the viewport area.
                return;
            }

            // Paint the node as a square. Create a Rectangle that represents the square's position and size.
            Point p = new(pos.X - state._nodeDiameterHalf, pos.Y - state._nodeDiameterHalf);
            Size s = new(state._nodeDiameter, state._nodeDiameter);
            Rectangle r = new(p, s);

            // Paint the node. Fill first and then border, this gives a clean border.
            Graphics g = state._g;
            g.FillRectangle(_nodeFillBrush, r);
            g.DrawRectangle(_nodeBorderPen, r);

            // Draw the node label.
            pos.X += state._nodeDiameterHalf + 1;
            pos.Y -= state._nodeDiameterHalf / 2;
            g.DrawString(id.ToString(), _settings.NodeLabelFont, _nodeLabelBrush, pos);
        }

        /// <summary>
        /// Paint the connections of a directed graph.
        /// </summary>
        /// <param name="model">The graph view model being painted.</param>
        /// <param name="state">A collection of working variables for painting a graph to a GDI+ surface.</param>
        protected virtual void PaintConnections(DirectedGraphViewModel model, PaintState state)
        {
            // Loop the connections, painting each in turn.
            ConnectionIds connIds = model.DirectedGraph.ConnectionIds;
            for(int i=0; i < connIds.Length; i++)
            {
                int srcIdx = connIds.GetSourceId(i);
                int tgtIdx = connIds.GetTargetId(i);
                float weight = model.WeightArr[i];

                Point srcPos = model.NodePosByIdx![srcIdx];
                Point tgtPos = model.NodePosByIdx![tgtIdx];

                PaintConnection(
                    srcIdx, tgtIdx,
                    srcPos, tgtPos, weight,
                    state);
            }
        }

        /// <summary>
        /// Paint a single connection.
        /// </summary>
        /// <param name="srcIdx">Source node index.</param>
        /// <param name="tgtIdx">Target node index.</param>
        /// <param name="srcPos">Source node position.</param>
        /// <param name="tgtPos">Target node position.</param>
        /// <param name="weight">Connection weight.</param>
        /// <param name="state">A collection of working variables for painting a graph to a GDI+ surface.</param>
        protected virtual void PaintConnection(
            int srcIdx, int tgtIdx,
            Point srcPos, Point tgtPos, float weight,
            PaintState state)
        {
            srcPos = ModelToViewport(srcPos, state);
            tgtPos = ModelToViewport(tgtPos, state);

            // Connections leave from the base of the source node and enter the top of the target node.
            // Adjust end points to make them neatly terminate just underneath the edge of the endpoint nodes.
            srcPos.Y += (int)(state._nodeDiameterHalf * 0.9f);
            tgtPos.Y -= (int)(state._nodeDiameterHalf * 0.9f);

            // Is any part of the connection within the viewport area?
            if(!IsPointWithinViewport(srcPos, state) && !IsPointWithinViewport(tgtPos, state))
            {   // Skip connection. It's outside the viewport area.
                return;
            }

            // Create a pen for painting the connection.
            // Width is related to connection strength/magnitude.
            float width = weight < 0f ? -MathF.Log10(1f - weight) : MathF.Log10(1f + weight);
            width = width * _connectionWeightToWidth * state._zoomFactor;

            width = Math.Max(1f, Math.Abs(width));
            Pen pen = new(weight < 0f ? _settings.NegativeWeightColor : _settings.PositiveWeightColor, width);

            // Draw the connection line.
            if(tgtPos.Y > srcPos.Y)
            {
                // Target is below the source. Draw a straight line.
                state._g.DrawLine(pen, srcPos, tgtPos);
            }
            else
            {
                // Target is above source. Draw a back-connection.
                PaintBackConnection(
                    srcIdx, tgtIdx,
                    srcPos, tgtPos,
                    state, pen);
            }
        }

        /// <summary>
        /// Paint a single back-connection, i.e. a connection whereby the target node is vertically above the source node.
        /// </summary>
        /// <param name="srcIdx">Source node index.</param>
        /// <param name="tgtIdx">Target node index.</param>
        /// <param name="srcPos">Source node position.</param>
        /// <param name="tgtPos">Target node position.</param>
        /// <param name="state">A collection of working variables for painting a graph to a GDI+ surface.</param>
        /// <param name="pen">A Pen for painting the connection.</param>
        protected virtual void PaintBackConnection(
            int srcIdx, int tgtIdx,
            Point srcPos, Point tgtPos,
            PaintState state,
            Pen pen)
        {
            const float slopeInit = 0.25f;
            const float slopeIncr = 0.23f;

            // Note. 'ref' here gives us a pointer to the actual struct data within each array element, as opposed to a copy an element on the local stack.
            // As such, modifications to srcInfo and tgtInfo will modify the array elements.
            ref ConnectionPointInfo srcInfo = ref state._nodeStateByIdx[srcIdx];
            ref ConnectionPointInfo tgtInfo = ref state._nodeStateByIdx[tgtIdx];

            // This is the maximum slope value we get before exceeding the slope threshold of 1.
            float slopeMax = slopeInit + (slopeIncr * MathF.Floor((1f - slopeInit) / slopeIncr));

            // Back connection is described by the line ABCDEF. A = srcPos and F = tgtPos.
            int srcConIdx, tgtConIdx;
            int srcSide, tgtSide;

            // If the source and target nodes are close on the X-axis then connect to the same side on both
            // nodes. Otherwise connect nodes on their facing sides.
            if(Math.Abs(tgtPos.X - srcPos.X) <= _settings.NodeDiameter)
            {
                srcConIdx = srcInfo.LowerLeft++;
                tgtConIdx = tgtInfo.UpperLeft++;
                srcSide = -1;
                tgtSide = -1;
            }
            else if(tgtPos.X > srcPos.X)
            {
                srcConIdx = srcInfo.LowerRight++;
                tgtConIdx = tgtInfo.UpperLeft++;
                srcSide = 1;
                tgtSide = -1;
            }
            else
            {
                srcConIdx = srcInfo.LowerLeft++;
                tgtConIdx = tgtInfo.UpperRight++;
                srcSide = -1;
                tgtSide = 1;
            }

            //--- Point B.
            // The line AB is a connection leg emerging from the base of a node. To visually separate multiple legs
            // the first leg has a gentle gradient (almost horizontal) and each successive leg has a steeper gradient.
            // Once a vertical gradient has been reached each successive leg is made longer.
            // Calculate leg slope: 0=horizontal, 1=vertical. Hence this is value is not a gradient.
            // Slope pre-trimming back to maximum of 1.0.
            float slopePre = slopeInit + (slopeIncr * srcConIdx);

            // Leg length.
            float lenAB = state._backConnectionLegLength;
            float slope = slopePre;
            if(slope > slopeMax)
            {
                // Increase length in fractions of _backConnectionLegLength.
                lenAB += (slopePre-slopeMax) * state._backConnectionLegLength;
                slope = 1f;
            }

            // Calculate position of B as relative to A.
            // Note. Length is taken to be L1 length (Manhattan distance). This means that the successive B positions
            // describe a straight line (rather than the circle you get with L2/Euclidean distance) which in turn
            // ensures that the BC segments of successive connections are evenly spaced out.
            int xDelta = (int)(lenAB * (1f - slope)) * srcSide;
            int yDelta = (int)(lenAB * slope);
            Point b = new(srcPos.X + xDelta, srcPos.Y + yDelta);

            //--- Point C.
            // Line BC is a horizontal line from the end of the leg AB.
            int lenBC = (int)(2f * slopePre * state._backConnectionLegLength);
            xDelta = lenBC * srcSide;
            Point c = new(b.X + xDelta, b.Y);

            //--- Point E. Equivalent to point B but emerging from the target node.
            slopePre = slopeInit + (slopeIncr * tgtConIdx);

            // Leg length.
            float lenEF = state._backConnectionLegLength;
            slope = slopePre;
            if(slope > slopeMax)
            {
                // Increase length in fractions of _backConnectionLegLength.
                lenEF += (slopePre-slopeMax) * state._backConnectionLegLength;
                slope = 1f;
            }

            xDelta = (int)(lenEF * (1f - slope)) * tgtSide;
            yDelta = -(int)(lenEF * slope);
            Point e = new(tgtPos.X + xDelta, tgtPos.Y + yDelta);

            //--- Point D. Equivalent to point C but on the target end of the connection.
            int lenDE = (int)(2f * slopePre * state._backConnectionLegLength);
            xDelta = lenDE * tgtSide;
            Point d = new(e.X + xDelta, e.Y);

            state._g.DrawLines(pen, new Point[] { srcPos, b, c, d, e, tgtPos });
        }

        #endregion

        #region Low Level Helper Methods

        /// <summary>
        /// Converts from a model coordinate to a viewport coordinate.
        /// </summary>
        private static Point ModelToViewport(Point p, PaintState state)
        {
            p.X = (int)(p.X * state._zoomFactor) - state._viewportArea.X;
            p.Y = (int)(p.Y * state._zoomFactor) - state._viewportArea.Y;
            return p;
        }

        /// <summary>
        /// Indicates if a point is within the graphics area represented by the viewport.
        /// That is, does an element at this position need to be painted.
        /// </summary>
        private static bool IsPointWithinViewport(Point p, PaintState state)
        {
            return (p.X >= 0)
                && (p.Y >= 0)
                && (p.X < state._viewportArea.Width)
                && (p.Y < state._viewportArea.Height);
        }

        #endregion
    }
}
