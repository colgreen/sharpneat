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
using SharpNeat.Network;

namespace SharpNeat.View.Graph
{
    /// <summary>
    /// Paints CPPNGraphs to a GDI+ Graphics object.
    /// </summary>
    public class CppnGraphPainter : IOGraphPainter
    {
        static readonly Brush[] _brushNodeFillArr;
        readonly IActivationFunctionLibrary _activationFnLibrary;

        #region Static Initializer

        static CppnGraphPainter()
        {
            // Create some distinct colors for coloring nodes. Node colors denotes activation function.
            _brushNodeFillArr = new Brush[8];
            _brushNodeFillArr[0] = new SolidBrush(Color.GhostWhite);            // Denotes the linear fn. Also used for input and output nodes.
            _brushNodeFillArr[1] = new SolidBrush(Color.FromArgb(84,100,248));  // Pastel Blue.
            _brushNodeFillArr[2] = new SolidBrush(Color.FromArgb(88,214,77));   // Green.
            _brushNodeFillArr[3] = new SolidBrush(Color.FromArgb(237,97,2));    // Orange.
            _brushNodeFillArr[4] = new SolidBrush(Color.FromArgb(255,244,0));   // Yellow.
            _brushNodeFillArr[5] = new SolidBrush(Color.FromArgb(0,255,255));   // Cyan.
            _brushNodeFillArr[6] = new SolidBrush(Color.FromArgb(214,77,186));  // Purple.
            _brushNodeFillArr[7] = new SolidBrush(Color.FromArgb(255,0,0));     // Red.
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided activation function library.
        /// This must be the same library used by the genomes/graphs being painted. 
        /// A legend of the activation functions is shown and the nodes are color coded to indicate the
        /// activation function at each node.
        /// </summary>
        public CppnGraphPainter(IActivationFunctionLibrary activationFnLibrary)
        {
            _activationFnLibrary = activationFnLibrary;
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Paints the provided IOGraph onto the provided GDI+ Graphics drawing surface.
        /// </summary>
        protected override void PaintNetwork(IOGraph graph, PaintState state)
        {
            // Invoke the base painting routine.
            base.PaintNetwork(graph, state);

            // Paint a legend for the activation functions.
            PaintLegend(state);
        }

        /// <summary>
        /// Override that paints nodes with a fill color that represents each node's activation function.
        /// </summary>
        protected override void PaintNode(GraphNode node, PaintState state)
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
            int activationFnId = (int)node.AuxData[0];
            Brush fillBrush = _brushNodeFillArr[activationFnId % _brushNodeFillArr.Length];
            g.FillRectangle(fillBrush, r);
            g.DrawRectangle(__penBlack, r);

            // Draw the node tag.
            nodePos.X += state._nodeDiameterHalf+1;
            nodePos.Y -= state._nodeDiameterHalf/2;
            g.DrawString(node.Tag, __fontNodeTag, __brushBlack, nodePos);
        }

        #endregion

        #region Private Methods

        private void PaintLegend(PaintState state)
        {
            const int LineHeight = 16;
            const int Margin = 10;

            IList<ActivationFunctionInfo> fnList = _activationFnLibrary.GetFunctionList();
            int count = fnList.Count;

            // Determine y position of first line.
            int yCurr = Math.Max(Margin, state._viewportArea.Height - ((count * LineHeight) + Margin));
            
            for(int i=0; i<count; i++, yCurr += LineHeight)
            {
                ActivationFunctionInfo fnInfo = fnList[i];
                const int X = Margin;

                // Paint an example node as part of the legend item.
                Point p = new Point(X, yCurr);
                Size s = new Size(state._nodeDiameter, state._nodeDiameter);
                Rectangle r = new Rectangle(p, s);

                // Paint the node. Fill first and then border, this gives a clean border.
                Graphics g = state._g;
                Brush fillBrush = _brushNodeFillArr[fnInfo.Id % _brushNodeFillArr.Length];
                g.FillRectangle(fillBrush, r);
                g.DrawRectangle(__penBlack, r);

                // Write the activation function string ID.
                g.DrawString(fnList[i].ActivationFunction.FunctionId, __fontNodeTag, __brushBlack, X+12, yCurr-1);
            }
        }

        #endregion
    }
}
