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

namespace SharpNeat.Drawing.Graph
{
    /// <summary>
    /// Represents a scheme for assigning a 2D position to each node in a graph.
    /// </summary>
    public interface IGraphLayoutScheme
    {
        /// <summary>
        /// Layout the nodes of the provided directed in a 2D area specified by <paramref name="layoutArea"/>.
        /// </summary>
        /// <param name="digraph">The directed graph to be laid out.</param>
        /// <param name="layoutArea">The area to layout nodes within.</param>
        /// <param name="nodePosByIdx">A span that will be populated with a 2D position for each node, within the provided layout area.</param>
        void Layout(
            DirectedGraph digraph,
            Size layoutArea,
            Span<Point> nodePosByIdx);
    }
}
