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
namespace SharpNeat.View.Graph
{
    /// <summary>
    /// Represents a connection in a graph.
    /// </summary>
    public class GraphConnection
    {
        GraphNode _sourceNode;
        GraphNode _targetNode;
        float _weight;

        #region Constructor

        /// <summary>
        /// Constructs a connection between the specified source and target nodes and of the specified weight.
        /// </summary>
        public GraphConnection(GraphNode sourceNode, GraphNode targetNode, float weight) 
        {
            _sourceNode = sourceNode;
            _targetNode = targetNode;
            _weight = weight;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the connection's source node.
        /// </summary>
        public GraphNode SourceNode
        {
            get { return _sourceNode; }
            set { _sourceNode = value; }
        }

        /// <summary>
        /// Gets or sets the connection's target node.
        /// </summary>
        public GraphNode TargetNode
        {
            get { return _targetNode; }
            set { _targetNode = value; }
        }

        /// <summary>
        /// Gets or sets the connection's weight.
        /// </summary>
        public float Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        #endregion
    }
}
