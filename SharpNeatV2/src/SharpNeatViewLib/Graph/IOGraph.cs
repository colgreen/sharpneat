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
    /// Represents a [weighted and directed] graph of connected nodes with nodes divided into 
    /// three types/groups; Input nodes, output nodes and hidden nodes. 
    /// </summary>
    public class IOGraph
    {
        readonly List<GraphNode> _inputNodeList;
        readonly List<GraphNode> _outputNodeList;
        readonly List<GraphNode> _hiddenNodeList;
        readonly float _connectionWeightRange;
        Size _bounds;

        #region Constructors

        /// <summary>
        /// Construct with the specified connection weight range. Weight range is used to determine
        /// each connection's strength relative to the overall range.
        /// </summary>
        public IOGraph(float connectionWeightRange)
        {
            _inputNodeList = new List<GraphNode>();
            _outputNodeList = new List<GraphNode>();
            _hiddenNodeList = new List<GraphNode>();
            _connectionWeightRange = connectionWeightRange;
        }

        /// <summary>
        /// Construct with the specified input, output and hidden node count. Counts are used to 
        /// pre-allocate storage.
        /// Weight range is used to determine each connection's strength relative to the overall range.
        /// </summary>
        public IOGraph(int inputCount, int outputCount, int hiddenCount, float connectionWeightRange)
        {
            _inputNodeList = new List<GraphNode>(inputCount);
            _outputNodeList = new List<GraphNode>(outputCount);
            _hiddenNodeList = new List<GraphNode>(hiddenCount);
            _connectionWeightRange = connectionWeightRange;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of input nodes.
        /// </summary>
        public List<GraphNode> InputNodeList
        {
            get { return _inputNodeList; }
        }

        /// <summary>
        /// Gets the list of output nodes.
        /// </summary>
        public List<GraphNode> OutputNodeList
        {
            get { return _outputNodeList; }
        }

        /// <summary>
        /// Gets the list of hidden nodes.
        /// </summary>
        public List<GraphNode> HiddenNodeList
        {
            get { return _hiddenNodeList; }
        }

        /// <summary>
        /// Gets the connection weight range.
        /// </summary>
        public float ConnectionWeightRange
        {
            get { return _connectionWeightRange; }
        }

        /// <summary>
        /// Gets or sets the bounds of the model elements.
        /// </summary>
        public Size Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        #endregion
    }
}
