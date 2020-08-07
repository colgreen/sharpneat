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
namespace SharpNeat.Drawing.Graph
{
    /// <summary>
    /// Represents a directed connection (vertex) in a graph.
    /// </summary>
    public class GraphConnection
    {
        #region Auto Properties

        /// <summary>
        /// Gets or sets the connection's source node.
        /// </summary>
        public GraphNode SourceNode { get; set; }

        /// <summary>
        /// Gets or sets the connection's target node.
        /// </summary>
        public GraphNode TargetNode { get; set; }

        /// <summary>
        /// Gets or sets the connection's weight.
        /// </summary>
        public float Weight { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a connection between the specified source and target nodes and of the specified weight.
        /// </summary>
        public GraphConnection(GraphNode sourceNode, GraphNode targetNode, float weight) 
        {
            SourceNode = sourceNode;
            TargetNode = targetNode;
            Weight = weight;
        }

        #endregion
    }
}
