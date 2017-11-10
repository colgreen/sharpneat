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
using System.Diagnostics;

namespace SharpNeat.Network.Acyclic
{
    /// <summary>
    /// An algorithm for analysing acyclic networks and calculating the depth of each node in the network.
    /// 
    /// Input nodes are defined as being at depth 0, the depth of all other nodes is defined as 
    /// the maximum number of hops from the depth 0 nodes.
    /// 
    /// Where multiple paths exist to a node (potentially each with a different numbers of hops) the node's 
    /// depth is defined by the path with the most number of hops.
    /// </summary>
    public class AcyclicGraphDepthAnalysis
    {
        #region Instance Fields

        /// <summary>
        /// The directed graph being analysed.
        /// </summary>
        DirectedGraph _digraph;

        /// <summary>
        /// Working array of node depths.
        /// </summary>
        int[] _nodeDepthById;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor. Prevents construction from outside of this class.
        /// </summary>
        private AcyclicGraphDepthAnalysis(DirectedGraph digraph)
        {
            _digraph = digraph;
            _nodeDepthById = new int[digraph.TotalNodeCount];
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Calculate node depths in an acyclic network.
        /// </summary>
        public static GraphDepthInfo CalculateNodeDepths(DirectedGraph digraph)
        {
            // Debug assert the graph is acyclic. 
            // Note. In a release build this test is not performed; in that case the depth analysis will throw a stack overflow exception for cyclic graphs.
            Debug.Assert(!CyclicGraphAnalysis.IsCyclicStatic(digraph));

            return new AcyclicGraphDepthAnalysis(digraph).CalculateNodeDepthsInner();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculate node depths in an acyclic network.
        /// </summary>
        private GraphDepthInfo CalculateNodeDepthsInner()
        {
            // Loop over all input nodes; Perform a depth first traversal of each in turn.
            int inputCount = _digraph.InputNodeCount;
            for(int id=0; id < inputCount; id++) 
            {
                // Traverse into the input node's target nodes.
                IList<int> tgtIdArr = _digraph.GetConnections(id);
                for(int i=0; i<tgtIdArr.Count; i++) {
                    TraverseNode(tgtIdArr[i], 1);
                }
            }

            // Determine the maximum depth of the graph.
            int maxDepth = 0;
            int nodeCount = _digraph.TotalNodeCount;
            for(int i=0; i < nodeCount; i++) 
            {
                if(_nodeDepthById[i] > maxDepth) {
                    maxDepth = _nodeDepthById[i];
                }
            }

            // Return depth analysis info.
            return new GraphDepthInfo(maxDepth+1, _nodeDepthById);
        }

        #endregion

        #region Private Methods

        private void TraverseNode(int nodeId, int depth)
        {
            // Check if the node has been visited before.
            if(_nodeDepthById[nodeId] >= depth)
            {   // The node already has already been visited via a path that assigned it a greater depth than the 
                // current path. Stop traversing this path.
                return;
            }

            // Either this is the first visit to the node *or* the node has been visited, but via a shorter path.
            // Either way we assign it the current depth value and traverse into its targets to update/set their depth.
            _nodeDepthById[nodeId] = depth;

            // Traverse into the current node's target nodes.
            IList<int> tgtIdArr = _digraph.GetConnections(nodeId);
            for(int i=0; i<tgtIdArr.Count; i++) {
                TraverseNode(tgtIdArr[i], depth + 1);
            }
        }

        #endregion
    }
}
