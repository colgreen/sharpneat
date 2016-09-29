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
    /// Represents a [weighted and directed] graph of connected nodes with nodes divided into 
    /// three types/groups; Input nodes, output nodes and hidden nodes. 
    /// </summary>
    public class IOGraph
    {
        readonly List<GraphNode> _inputNodeList;
        readonly List<GraphNode> _outputNodeList;
        readonly List<GraphNode> _hiddenNodeList;
        float _connectionWeightRange;
        Size _bounds;
        int _depth;

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
        public IOGraph(int inputCount, int outputCount, int hiddenCount, float connectionWeightRange, int depth)
        {
            _inputNodeList = new List<GraphNode>(inputCount);
            _outputNodeList = new List<GraphNode>(outputCount);
            _hiddenNodeList = new List<GraphNode>(hiddenCount);
            _connectionWeightRange = connectionWeightRange;
            _depth = depth;
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
            set { _connectionWeightRange = value; }
        }

        /// <summary>
        /// Gets or sets the bounds of the model elements.
        /// </summary>
        public Size Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        /// <summary>
        /// Indicates the total depth of the network. 
        /// E.g. a network with a single hidden node connected up to an input and output will have three layers
        /// (input, hidden layer 1, output) and will thus have a depth of 3. The layers are assigned depth values
        /// of 0, 1 and 2 respectively.
        /// </summary>
        public int Depth
        {
            get { return _depth; }
        }

        #endregion
    }
}
