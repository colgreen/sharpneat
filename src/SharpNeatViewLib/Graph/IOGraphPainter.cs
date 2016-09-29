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
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SharpNeat.View.Graph
{
    /// <summary>
    /// Paints IOGraphs to a GDI+ Graphics object.
    /// </summary>
    public class IOGraphPainter
    {
        /// <summary>Diameter of a node in the model coordinate space.</summary>
        const float NodeDiameterModel = 10;
        /// <summary>Font for drawing text on the viewport.</summary>
        protected static readonly Font __fontNodeTag = new Font("Microsoft Sans Serif", 7.0F);
        /// <summary>Black brush</summary>
        protected static readonly Brush __brushBlack = new SolidBrush(Color.Black);
        /// <summary>Brush for node fill color.</summary>
        protected static readonly Brush __brushNodeFill = new SolidBrush(Color.GhostWhite);
        /// <summary>Black pen for node borders.</summary>
        protected static readonly Pen __penBlack = new Pen(Color.Black, 2F);
        /// <summary>Pen for drawing connections with positive connection weight.</summary>
        protected static readonly Color _connectionPositive = Color.Red;
        /// <summary>Pen for drawing connections with negative connection weight.</summary>
        protected static readonly Color _connectionNegative = Color.Blue;

        #region Painting Methods / High Level

        /// <summary>
        /// Paints the provided IOGraph onto the provided GDI+ Graphics drawing surface.
        /// </summary>
        public void PaintNetwork(IOGraph graph, Graphics g, Rectangle viewportArea, float zoomFactor)
        {
            // Create a PaintState object. This holds all temporary state info for the painting routines.
            // Pass the call on to the virtual PaintNetwork. This allows us to override a version of PaintNetwork 
            // that has access to a PaintState object.
            PaintState state = new PaintState(g, viewportArea, zoomFactor, graph.ConnectionWeightRange);
            PaintNetwork(graph, state);
        }

        /// <summary>
        /// Paints the provided IOGraph onto the current GDI+ Graphics drawing surface.
        /// </summary>
        protected virtual void PaintNetwork(IOGraph graph, PaintState state)
        {
            // Create per-node state info.
            int hiddenNodeCount = graph.HiddenNodeList.Count;
            int inputNodeCount = graph.InputNodeList.Count;
            int outputNodeCount = graph.OutputNodeList.Count;
            state._nodeStateDict = new Dictionary<GraphNode,ConnectionPointInfo>(hiddenNodeCount + inputNodeCount + outputNodeCount);

            // Paint all connections. We do this first and paint nodes on top of the connections. This allows the 
            // slightly messy ends of the connections to be painted over by the nodes.
            PaintConnections(graph.InputNodeList, state);
            PaintConnections(graph.HiddenNodeList, state);
            PaintConnections(graph.OutputNodeList, state);

            // Paint all nodes. Painted over the top of connection endpoints.
            PaintNodes(graph.InputNodeList, state);
            PaintNodes(graph.HiddenNodeList, state);
            PaintNodes(graph.OutputNodeList, state);
        }

        #endregion

        #region Painting Methods / Model Element Painting

        private void PaintNodes(IList<GraphNode> nodeList, PaintState state)
        {
            int nodeCount = nodeList.Count;
            for(int i=0; i<nodeCount; i++) {
                PaintNode(nodeList[i], state);
            }
        }

        private void PaintConnections(IList<GraphNode> nodeList, PaintState state)
        {
            int nodeCount = nodeList.Count;
            for(int i=0; i<nodeCount; i++)
            {
                List<GraphConnection> conList = nodeList[i].OutConnectionList;
                int conCount = conList.Count;
                for(int j=0; j<conCount; j++) {
                    PaintConnection(conList[j], state);
                }
            }
        }

        /// <summary>
        /// Paints a single graph node.
        /// </summary>
        protected virtual void PaintNode(GraphNode node, PaintState state)
        {
            Point nodePos = ModelToViewport(node.Position, state);
            if(!IsPointWithinViewport(nodePos, state))
            {   // Skip node. It's outside the viewport area.
                return;
            }

            // Paint the node as a square. Create a Rectangle that represents the square's position and size.
            Point p = new Point(nodePos.X - state._nodeDiameterHalf, nodePos.Y - state._nodeDiameterHalf);
            Size s = new Size(state._nodeDiameter, state._nodeDiameter);
            Rectangle r = new Rectangle(p, s);

            // Paint the node. Fill first and then border, this gives a clean border.
            Graphics g = state._g;
            g.FillRectangle(__brushNodeFill, r);
            g.DrawRectangle(__penBlack, r);

            // Draw the node tag.
            nodePos.X += state._nodeDiameterHalf+1;
            nodePos.Y -= state._nodeDiameterHalf/2;
            g.DrawString(node.Tag, __fontNodeTag, __brushBlack, nodePos);
        }

        private void PaintConnection(GraphConnection con, PaintState state)
        {
            Point srcPos = ModelToViewport(con.SourceNode.Position, state);
            Point tgtPos = ModelToViewport(con.TargetNode.Position, state);

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
            float width = (float)(con.Weight < 0.0 ? -Math.Log10(1.0 - con.Weight) : Math.Log10(1.0 + con.Weight));
            width = width * state._connectionWeightToWidth * state._zoomFactor;

            width = Math.Max(1f, Math.Abs(width));
            Pen pen = new Pen(con.Weight < 0f ? _connectionNegative : _connectionPositive, width);
      
            // Draw the connection line.
            if(tgtPos.Y > srcPos.Y)
            {   
                // Target is below the source. Draw a straight line.
                state._g.DrawLine(pen, srcPos, tgtPos);                
            }
            else
            {   
                // Target is above source. Draw a back-connection.
                PaintBackConnection(pen, srcPos, tgtPos,
                                    state.GetNodeStateInfo(con.SourceNode),
                                    state.GetNodeStateInfo(con.TargetNode),
                                    state);
            }
        }

        private void PaintBackConnection(Pen pen,
                                         Point srcPos, 
                                         Point tgtPos,
                                         ConnectionPointInfo srcInfo,
                                         ConnectionPointInfo tgtInfo,
                                         PaintState state)
        {
            const float SlopeInit = 0.25f;
            const float SlopeIncr = 0.23f;

            // This is the maximum slope value we get before exceeding the slope threshold of 1.
            float slopeMax = SlopeInit + (SlopeIncr * (float)Math.Floor((1f-SlopeInit) / SlopeIncr));

            // Back connection is described by the line ABCDEF. A = srcPos and F = tgtPos.
            int srcConIdx, tgtConIdx;
            int srcSide, tgtSide;

            // If the source and target nodes are close on the X-axis then connect to the same side on both
            // nodes. Otherwise connect nodes on their facing sides.
            if(Math.Abs(tgtPos.X - srcPos.X) <= NodeDiameterModel) {
                srcConIdx = srcInfo._lowerLeft++;
                tgtConIdx = tgtInfo._upperLeft++;
                srcSide = -1;
                tgtSide = -1;
            }
            else if(tgtPos.X > srcPos.X) {
                srcConIdx = srcInfo._lowerRight++;
                tgtConIdx = tgtInfo._upperLeft++;
                srcSide = 1;
                tgtSide = -1;
            } else {
                srcConIdx = srcInfo._lowerLeft++;
                tgtConIdx = tgtInfo._upperRight++;
                srcSide = -1;
                tgtSide = 1;
            }

        //--- Point B.
            // The line AB is a connection leg emerging from the base of a node. To visually separate multiple legs
            // the first leg has a gentle gradient (almost horizontal) and each successive leg has a steeper gradient.
            // Once a vertical gradient has been reached each successive leg is made longer.
            // Calculate leg slope: 0=horizontal, 1=vertical. Hence this is value is not a gradient.
            // Slope pre-trimming back to maximum of 1.0.
            float slopePre = SlopeInit + (SlopeIncr * srcConIdx);

            // Leg length.
            float lenAB = state._backConnectionLegLength;
            float slope = slopePre;
            if(slope > slopeMax)  
            {   // Increase length in fractions of _backConnectionLegLength.
                lenAB += (slopePre-slopeMax) * state._backConnectionLegLength;
                slope = 1f;
            }

            // Calculate position of B as relative to A. 
            // Note. Length is taken to be L1 length (Manhattan distance). This means that the successive B positions 
            // describe a straight line (rather than the circle you get with L2/Euclidean distance) which in turn 
            // ensures that the BC segments of successive connections are evenly spaced out.
            int xDelta = (int)(lenAB * (1f - slope)) * srcSide;
            int yDelta = (int)(lenAB * slope);
            Point b = new Point(srcPos.X + xDelta, srcPos.Y + yDelta);

        //--- Point C.
            // Line BC is a horizontal line from the end of the leg AB.
            int lenBC = (int)(2f * slopePre * state._backConnectionLegLength);
            xDelta = lenBC * srcSide;
            Point c = new Point(b.X + xDelta, b.Y);

        //--- Point E. Equivalent to point B but emerging from the target node.
            slopePre = SlopeInit + (SlopeIncr * tgtConIdx);

            // Leg length.
            float lenEF = state._backConnectionLegLength;
            slope = slopePre;
            if(slope > slopeMax)  
            {   // Increase length in fractions of _backConnectionLegLength.
                lenEF += (slopePre-slopeMax) * state._backConnectionLegLength;
                slope = 1f;
            }

            xDelta = (int)(lenEF * (1f - slope)) * tgtSide;
            yDelta = -(int)(lenEF * slope);
            Point e = new Point(tgtPos.X + xDelta, tgtPos.Y + yDelta);

        //--- Point D. Equivalent to point C but on the target end of the connection.
            int lenDE = (int)(2f * slopePre * state._backConnectionLegLength);
            xDelta = lenDE * tgtSide;
            Point d = new Point(e.X + xDelta, e.Y);

            state._g.DrawLines(pen, new Point[]{srcPos,b,c,d,e,tgtPos});
        }

        #endregion

        #region Low Level Helper Methods

        /// <summary>
        /// Converts from a model coordinate to a viewport coordinate.
        /// </summary>
        protected Point ModelToViewport(Point p, PaintState state)
        {
            p.X = (int)((float)p.X * state._zoomFactor) - state._viewportArea.X;
            p.Y = (int)((float)p.Y * state._zoomFactor) - state._viewportArea.Y;
            return p;
        }

        /// <summary>
        /// Indicates if a point is within the graphics area represented by the viewport.
        /// That is, does an element at this position need to be painted.
        /// </summary>
        protected bool IsPointWithinViewport(Point p, PaintState state)
        {
            return (p.X >= 0) 
                && (p.Y >= 0) 
                && (p.X < state._viewportArea.Width) 
                && (p.Y < state._viewportArea.Height);
        }

        #endregion

        #region Inner Classes

        /// <summary>
        /// Represents data required for by painting routines.
        /// </summary>
        public class PaintState
        {
            // State variables.
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

            /// <summary>
            /// Dictionary containing temporary painting related state for each graph node.
            /// </summary>
            public Dictionary<GraphNode,ConnectionPointInfo> _nodeStateDict;

            /// <summary>
            /// Construct with the provided Graphics painting surface and state data.
            /// </summary>
            public PaintState(Graphics g, Rectangle viewportArea, float zoomFactor, float connectionWeightRange)
            {
                // Store state variables.
                _g = g;
                _viewportArea = viewportArea;
                _zoomFactor = zoomFactor;
                _connectionWeightRange = connectionWeightRange;
                _connectionWeightRangeHalf = connectionWeightRange * 0.5f;
                _connectionWeightToWidth = (float)(2.0 / Math.Log10(connectionWeightRange + 1.0));

                // Precalculate some useful derived values.
                _nodeDiameter = (int)(NodeDiameterModel * zoomFactor);
                _nodeDiameterHalf = (int)((NodeDiameterModel * zoomFactor) * 0.5f);
                _backConnectionLegLength = _nodeDiameter * 1.6f;
            }

            /// <summary>
            /// Gets the state object for a given graph node. Creates the object if it does not yet exist.
            /// </summary>
            public ConnectionPointInfo GetNodeStateInfo(GraphNode node)
            {
                ConnectionPointInfo info;
                if(!_nodeStateDict.TryGetValue(node, out info))
                {
                    info = new ConnectionPointInfo();
                    _nodeStateDict.Add(node, info);
                }
                return info;
            }
        }

        /// <summary>
        /// Class used for tracking connection point on nodes when drawing backwards directed 
        /// connections (target node higher than the source node).
        /// </summary>
        public class ConnectionPointInfo
        {
            /// <summary>Running connection count for top left of node.</summary>
            public int _upperLeft  = 0;
            /// <summary>Running connection count for top right of node.</summary>
            public int _upperRight = 0;
            /// <summary>Running connection count for bottom left of node.</summary>
            public int _lowerLeft  = 0;
            /// <summary>Running connection count for bottom right of node.</summary>
            public int _lowerRight = 0;
        }

        #endregion
    }
}
