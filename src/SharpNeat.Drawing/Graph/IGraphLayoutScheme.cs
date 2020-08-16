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
    /// Represents a scheme for assigning a 2D position to each node node in a graph.
    /// </summary>
    public interface IGraphLayoutScheme
    {
        /// <summary>
        /// Position/layout the nodes of a digraph within a specified 2D layout area.
        /// </summary>
        /// <param name="viewModel">The graph view model to be laid out.</param>
        /// <param name="layoutArea">The area to layout nodes within.</param>
        void Layout(DirectedGraphViewModel viewModel, Size layoutArea);
    }
}
