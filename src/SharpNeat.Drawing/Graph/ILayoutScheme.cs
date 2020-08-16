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
    public interface ILayoutScheme
    {
        /// <summary>
        /// Position/layout the nodes of an IOGraph within a specified 2D layout area.
        /// </summary>
        /// <param name="graph">The network/graph structure to be laid out.</param>
        /// <param name="layoutArea">The area the structure is to be laid out on.</param>
        void Layout(IOGraph graph, Size layoutArea);
    }
}
