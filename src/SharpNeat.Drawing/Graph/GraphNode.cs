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
using System.Collections.Generic;
using System.Drawing;

namespace SharpNeat.Drawing.Graph
{
    /// <summary>
    /// Represents a node in a graph.
    /// </summary>
    public class GraphNode
    {
        #region Auto Properties

        /// <summary>
        /// Gets or sets the node's tag.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the node's position.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets an array of auxiliary data.
        /// </summary>
        public object[]? AuxData { get; set; }

        /// <summary>
        /// Depth of the node within the network. Input nodes are defined as being at depth zero,
        /// all other nodes are defined by the number of connection hops to reach them from an input node.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Gets the node's list of output connections.
        /// </summary>
        public List<GraphConnection> OutConnectionList { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs with the provided string tag.
        /// </summary>
        public GraphNode(string tag) 
            : this(tag, Point.Empty, null, 0)
        {}

        /// <summary>
        /// Constructs with the provided string tag and position.
        /// </summary>
        public GraphNode(string tag, Point position)
            : this(tag, position, null, 0)
        {}

        /// <summary>
        /// Constructs with the provided string tag, position and auxiliary data.
        /// </summary>
        public GraphNode(string tag, Point position, object[]? auxData)
            : this(tag, position, auxData, 0)
        {}

        /// <summary>
        /// Constructs with the provided string tag, position, auxiliary data and node depth.
        /// </summary>
        public GraphNode(string tag, Point position, object[]? auxData, int depth)
        {
            Tag = tag;
            Position = position;
            AuxData = auxData;
            Depth = depth;
            OutConnectionList = new List<GraphConnection>();
        }

        #endregion
    }
}
