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
using System.Drawing;

namespace SharpNeat.View.Graph
{
    /// <summary>
    /// An ILayoutManager that positions nodes evenly spaced out on a grid.
    /// </summary>
    public class GridLayoutManager : ILayoutManager
    {
        const int MarginX = 0;
        const float MarginProportionYTop = 0.04f;
        const float MarginProportionYBottom = 0.1f;

        /// <summary>
        /// Layout nodes evenly spaced out on a grid.
        /// </summary>
        /// <param name="graph">The network/graph structure to be laid out.</param>
        /// <param name="layoutArea">The area the structure is to be laid out on.</param>
        public void Layout(IOGraph graph, Size layoutArea)
        {
            // Size struct to keep track of the area that bounds all nodes.
            Size bounds = new Size(0,0);

            // Some other useful variables.
            int inputCount = graph.InputNodeList.Count;
            int outputCount = graph.OutputNodeList.Count;
            int hiddenCount = graph.HiddenNodeList.Count;
            double heightWidthRatio = (double)layoutArea.Height/(double)layoutArea.Width;

            // Determine how many layers/rows to arrange the nodes into. 
            // Note. we always show the inputs and outputs in their own rows, therefore this just
            // handles how many additional rows are required for the hidden nodes.
            int numLayersHidden=0;

            if(hiddenCount>0)
            {   // Arrange nodes in a square and adjust for height/width ratio of layout area.
                double sqrtHidden = Math.Sqrt(hiddenCount);
                numLayersHidden = (int)Math.Floor(sqrtHidden * heightWidthRatio);

                // Must be at least 1 layer if we have nodes.
                numLayersHidden = Math.Max(1, numLayersHidden);
            }

            // Arrange the nodes.
            int yMarginTop = (int)(layoutArea.Height * MarginProportionYTop);
            int yMarginBottom = (int)(layoutArea.Height * MarginProportionYBottom);
            int layoutHeight = layoutArea.Height - (yMarginTop + yMarginBottom);
            int yCurrent = yMarginTop;

            int yIncrement; 
            if(0 == numLayersHidden) 
            {   // Just two layers (input and output).
                yIncrement = layoutHeight;
            } else {
                yIncrement = layoutHeight / (numLayersHidden+1);
            }

            // Input layer. Place all input nodes in one layer.
            int layoutWidth = layoutArea.Width - (2 * MarginX);
            int xIncrement = layoutWidth / (inputCount+1);
            int xCurrent = MarginX + xIncrement;

            // Loop input nodes.
            for(int i=0; i<inputCount; i++)
            {
                GraphNode node = graph.InputNodeList[i];
                node.Position = new Point(xCurrent, yCurrent);
                UpdateModelBounds(node, ref bounds);
                xCurrent += xIncrement;
            }

            // Increment yCurrent, ready for the next layer.
            yCurrent += yIncrement;            
            
            // Layout hidden layers.
            if(0 != numLayersHidden)
            {
                // Calculate the max number of nodes in any hidden layer.
                int layerNodesMax = (int)Math.Ceiling((double)hiddenCount / (double)numLayersHidden);

                // Keep track of how many nodes remain to be positioned. The last layer will have fewer 
                // than layerNodesMax if the number of nodes isn't a square number.
                int nodesRemaining = hiddenCount;
                int nodeIdx=0;

                // Loop layers.
                for(int i=0; i<numLayersHidden; i++)
                {
                    // Calculate the number of nodes in this layer.
                    int layerNodeCount = Math.Min(nodesRemaining, layerNodesMax);

                    // Position nodes in this layer.
                    xIncrement = layoutWidth / (layerNodeCount+1);
                    xCurrent = MarginX + xIncrement;

                    // Loop nodes in this layer.
                    for(int j=0; j<layerNodeCount; j++, nodeIdx++)
                    {
                        GraphNode node = graph.HiddenNodeList[nodeIdx];
                        node.Position = new Point(xCurrent, yCurrent);
                        UpdateModelBounds(node, ref bounds);
                        xCurrent += xIncrement;
                    }

                    // Increment yCurrent, ready for the next layer.
                    yCurrent += yIncrement;
                    nodesRemaining -= layerNodeCount;
                }
            }

            // Output layer. Place all output nodes in one layer.
            xIncrement = layoutWidth / (outputCount+1);
            xCurrent = MarginX + xIncrement;

            // Loop output nodes.
            for(int i=0; i<outputCount; i++)
            {
                GraphNode node = graph.OutputNodeList[i];
                node.Position = new Point(xCurrent, yCurrent);
                UpdateModelBounds(node, ref bounds);
                xCurrent += xIncrement;
            }

            // Store the bounds of the graph elements. Useful when drawing the graph.
            graph.Bounds = bounds;
        }

        private void UpdateModelBounds(GraphNode node, ref Size bounds)
        {
            if(node.Position.X > bounds.Width) {
                bounds.Width = node.Position.X;
            }
            if(node.Position.Y > bounds.Height) {
                bounds.Height = node.Position.Y;
            }
        }
    }
}
