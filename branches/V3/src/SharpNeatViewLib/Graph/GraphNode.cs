/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
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
        /// Constructs with the provided string tag, position and auxilliary data.
        /// </summary>
        public GraphNode(string tag, Point position, object[] auxData)
            : this(tag, position, auxData, 0)
        {}

        /// <summary>
        /// Constructs with the provided string tag, position, auxilliary data and node depth.
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
        /// Gets or sets an array of auxilliary data.
        /// </summary>
        public object[] AuxData
        {
            get { return _auxData; }
            set { _auxData = value; }
        }

        /// <summary>
        /// Depth of the node within the network. Input nodes are defined as beign at depth zero,
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
