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

namespace SharpNeat.Network
{
    /// <summary>
    /// An algorithm for analysing cyclic networks and calculating the depth of each node in the network.
    /// Bias and input nodes are defined as being at depth 0, the depth of all other nodes is defined as 
    /// the maximum number of hops from the depth 0 nodes, so where multiple paths exist to a node (potentially
    /// with different numbers of hops) we take the maximum number of hops as that node's depth. 
    /// 
    /// The number of hops to a node ignores any cycles in the connectivity, that is, teh numebr of hops is the 
    /// number of hops in the direct routes to a node (no node in the pathway more than once, hence not including 
    /// any cycles)
    /// </summary>
    public class CyclicNetworkDepthAnalysis
    {
        /// <summary>
        /// Connectivity data for the INetworkDefinition that is currently being tested.
        /// </summary>
        NetworkConnectivityData _networkConnectivityData;

        /// <summary>
        /// Dictionary of node depths keyed by node ID. Working data.
        /// </summary>
        Dictionary<uint,int> _nodeDepthById = new Dictionary<uint,int>();

        #region Public Methods

        /// <summary>
        /// Calculate node depths in an acyclic network.
        /// </summary>
        public NetworkDepthInfo CalculateNodeDepths(INetworkDefinition networkDef)
        {
            // Clear any existing state (allow reuse of this class).
            _nodeDepthById.Clear();

            // Get and store connectivity data for the network.
            _networkConnectivityData = networkDef.GetConnectivityData();

            // Loop over all input (and bias) nodes; Perform a depth first traversal of each in turn.
            // Set of nodes visited in the current traversal (reset before each individual depth first traversal).
            HashSet<uint> visitedNodeSet = new HashSet<uint>();
            int inputAndBiasCount = networkDef.InputNodeCount + 1;
            for(int i=0; i<inputAndBiasCount; i++)
            {
                visitedNodeSet.Clear();
                TraverseNode(_networkConnectivityData.GetNodeDataByIndex(i), visitedNodeSet, 0);
            }

            // Extract node depths from _nodeDepthById into an array of depths (node depth by node index).
            // Note. Any node not in the dictionary is in an isolated sub-network and will be assigned to 
            // layer 0 by default.
            INodeList nodeList = networkDef.NodeList;
            int nodeCount = nodeList.Count;
            int[] nodeDepthArr = new int[nodeCount];
            int maxDepth = 0;

            // Loop over nodes and set the node depth. Skip over input and bias nodes, they are defined as 
            // being in layer zero.
            for(int i=inputAndBiasCount; i<nodeCount; i++)
            {
                // Lookup the node's depth. If not found depth remains set to zero.
                int depth;
                if(_nodeDepthById.TryGetValue(nodeList[i].Id, out depth)) 
                {
                    nodeDepthArr[i] = depth;
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

        private void TraverseNode(NodeConnectivityData nodeData, HashSet<uint> visitedNodeSet, int depth)
        {
            // Check if the node has already been encountered during the current traversal (have we followed a cycle in the connectivity).
            if(visitedNodeSet.Contains(nodeData._id)) 
            {   // Dont follow cycles.
                return;
            }

            // Register the visit.
            visitedNodeSet.Add(nodeData._id);

            // Check if the node has been traversed by a previous traversal.
            int assignedDepth;
            if(_nodeDepthById.TryGetValue(nodeData._id, out assignedDepth) && assignedDepth >= depth)
            {   // The node already has already been visited via a path that assigned it a greater depth than the 
                // current path. Stop traversing this path.
                return;
            }

            // Either this is the first visit to the node *or* the node has been visited, but via a shorter path.
            // Either way we assign it the current depth value and traverse into its targets to update/set their depth.
            _nodeDepthById[nodeData._id] = depth;
            foreach(uint targetId in nodeData._tgtNodes)
            {
                TraverseNode(_networkConnectivityData.GetNodeDataById(targetId), visitedNodeSet, depth + 1);
            }
        }

        #endregion
    }
}
