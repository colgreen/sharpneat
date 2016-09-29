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
using System.Collections.Generic;
using System.Drawing;

namespace SharpNeat.View.Graph
{
    /// <summary>
    /// Represents a node in a graph.
    /// </summary>
    public class GraphNode
    {
        string _tag;
        Point _position;
        object[] _auxData;
        int _depth;

        readonly List<GraphConnection> _inConnectionList;
        readonly List<GraphConnection> _outConnectionList;

        #region Constructor

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
        public GraphNode(string tag, Point position, object[] auxData)
            : this(tag, position, auxData, 0)
        {}

        /// <summary>
        /// Constructs with the provided string tag, position, auxiliary data and node depth.
        /// </summary>
        public GraphNode(string tag, Point position, object[] auxData, int depth)
        {
            _tag = tag;
            _position = position;
            _auxData = auxData;
            _depth = depth;
            _inConnectionList = new List<GraphConnection>();
            _outConnectionList = new List<GraphConnection>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the node's tag.
        /// </summary>
        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        /// <summary>
        /// Gets or sets the node's position.
        /// </summary>
        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Gets or sets an array of auxiliary data.
        /// </summary>
        public object[] AuxData
        {
            get { return _auxData; }
            set { _auxData = value; }
        }

        /// <summary>
        /// Depth of the node within the network. Input nodes are defined as being at depth zero,
        /// all other nodes are defined by the number of connection hops to reach them from an input node.
        /// </summary>
        public int Depth
        {
            get { return _depth; }
            set { _depth = value; }
        }

        /// <summary>
        /// Gets the node's list of input connections.
        /// </summary>
        public List<GraphConnection> InConnectionList
        {
            get { return _inConnectionList; }
        }

        /// <summary>
        /// Gets the node's list of output connections.
        /// </summary>
        public List<GraphConnection> OutConnectionList
        {
            get { return _outConnectionList; }
        }

        #endregion
    }
}
