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
using SharpNeat.Network.Analysis;

namespace SharpNeat.Network2
{
    /// <summary>
    /// An algorithm for analysing acyclic networks and calculating the depth of each node in the network.
    /// Input nodes are defined as being at depth 0, the depth of all other nodes is defined as 
    /// the maximum number of hops from the depth 0 nodes, so where multiple paths exist to a node (potentially
    /// with different numbers of hops) we take the maximum number of hops as that node's depth. 
    /// </summary>
    public class AcyclicGraphDepthAnalysis
    {
        #region Instance Fields

        /// <summary>
        /// The directed graph being analysed.
        /// </summary>
        DirectedGraph _directedGraph;

        /// <summary>
        /// Dictionary of node depths keyed by node ID. Working data.
        /// </summary>
        Dictionary<int,int> _nodeDepthById = new Dictionary<int,int>();

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor. Prevents construction from outside of this class.
        /// </summary>
        private AcyclicGraphDepthAnalysis(DirectedGraph directedGraph)
        {
            _directedGraph = directedGraph;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Calculate node depths in an acyclic network.
        /// </summary>
        public static NetworkDepthInfo CalculateNodeDepths(DirectedGraph directedGraph, IEnumerable<int> inputNodeIds)
        {
            return new AcyclicGraphDepthAnalysis(directedGraph).CalculateNodeDepthsInner(inputNodeIds);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculate node depths in an acyclic network.
        /// </summary>
        public NetworkDepthInfo CalculateNodeDepthsInner(IEnumerable<int> inputNodeIds)
        {
            // Clear any existing state (allow reuse of this class).
            _nodeDepthById.Clear();

            // Loop over all input nodes; Perform a depth first traversal of each in turn.
            int inputCount = 0;
            foreach(int nodeId in inputNodeIds) 
            {
                TraverseNode(nodeId, 0);
                inputCount++;
            }

            // Extract node depths from _nodeDepthById into an array of depths (node depth by node index).
            // Note. Any node not in the dictionary is in an isolated sub-network and will be assigned to 
            // layer 0 by default.
            int nodeCount = _directedGraph.NodeCount;
            int[] nodeDepthArr = new int[nodeCount];
            int maxDepth = 0;

            // Loop over nodes and set the node depth. Skip over input nodes, they are defined as 
            // being in layer zero.
            for(int nodeId=inputCount; nodeId<nodeCount; nodeId++)
            {
                // Lookup the node's depth. If not found depth remains set to zero.
                int depth;
                if(_nodeDepthById.TryGetValue(nodeId, out depth)) 
                {
                    nodeDepthArr[nodeId] = depth;
                    // Also determine maximum depth, that is, total depth of the network.
                    if(depth > maxDepth) {
                        maxDepth = depth;
                    }
                }
            }

            // Return depth analysis info.
            return new NetworkDepthInfo(maxDepth+1, nodeDepthArr);
        }

        #endregion

        #region Private Methods

        private void TraverseNode(int nodeId, int depth)
        {
            // Check if the node has been visited before.
            int assignedDepth;
            if(_nodeDepthById.TryGetValue(nodeId, out assignedDepth) && assignedDepth >= depth)
            {   // The node already has already been visited via a path that assigned it a greater depth than the 
                // current path. Stop traversing this path.
                return;
            }

            // Either this is the first visit to the node *or* the node has been visited, but via a shorter path.
            // Either way we assign it the current depth value and traverse into its targets to update/set their depth.
            _nodeDepthById[nodeId] = depth;

            IList<DirectedConnection> connArr = _directedGraph.GetConnections(nodeId);
            for(int i=0; i<connArr.Count; i++) {
                TraverseNode(connArr[i].TargetId, depth + 1);
            }
        }

        #endregion
    }
}
