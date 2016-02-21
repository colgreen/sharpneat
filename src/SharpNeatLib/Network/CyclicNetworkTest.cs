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
    /// An algorithm for testing for the presence of at least one connectivity cycle within a network.
    /// 
    /// Method.
    /// =======
    /// 1) We loop over all nodes in the network and perform a depth-first traversal from each node. 
    /// (Note. the order that the nodes are traversed does not affext the correctness of the method)
    /// 
    /// 2) Each traversal keeps track of its ancestor nodes (the path to the current node) for each step
    /// in the traversal. Thus if the traversal encounters an ancestor node then a cycle has been detected.
    /// 
    /// 3) A set of visited nodes is maintained. This persists between traversals and allows each traversal 
    /// to avoid traversing into nodes that have already been traversed.
    /// 
    /// Note. We must traverse from each node rather then just e.g. the input nodes, because the network may 
    /// have connectivity dead ends or even isolated connectivity that therefore would not be traversed into 
    /// by following connectivity from the input nodes only, hence we perform a traversal from each node and
    /// attempt to maintain algorithmic efficiency by avoiding traversal into ndoes that haev already been 
    /// traversed into.
    /// </summary>
    public class CyclicNetworkTest
    {
        #region Instance Fields

        /// <summary>
        /// Connectivity data for the INetworkDefinition that is currently being tested.
        /// </summary>
        NetworkConnectivityData _networkConnectivityData;
        /// <summary>
        /// Set of traversal ancestors of current node. 
        /// </summary>
        HashSet<uint> _ancestorNodeSet = new HashSet<uint>();
        /// <summary>
        /// Set of all visted nodes. This allows us to quickly determine if a path should be traversed or not. 
        /// </summary>
        HashSet<uint> _visitedNodeSet = new HashSet<uint>();

        #endregion

        #region Constructor

        /// <summary>
        /// Private consrtcutor. Prevents construction from outside of this class.
        /// </summary>
        private CyclicNetworkTest()
        {
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Returns true if there is at least one connectivity cycle within the provided INetworkDefinition.
        /// </summary>
        public static bool IsNetworkCyclic(INetworkDefinition networkDef)
        {
            return new CyclicNetworkTest().IsNetworkCyclicInternal(networkDef);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns true if there is at least one connectivity cycle within the provided INetworkDefinition.
        /// </summary>
        public bool IsNetworkCyclicInternal(INetworkDefinition networkDef)
        {
            // Clear any existing state (allow reuse of this class).
            _ancestorNodeSet.Clear();
            _visitedNodeSet.Clear();

            // Get and store connectivity data for the network.
            _networkConnectivityData = networkDef.GetConnectivityData();

            // Loop over all nodes. Take each one in turn as a traversal root node.
            foreach(INetworkNode node in networkDef.NodeList)
            {
                // Determine if the node has already been visited.
                if(_visitedNodeSet.Contains(node.Id)) 
                {   // Already traversed; Skip.
                    continue;
                }

                // Traverse into the node. 
                if(TraverseNode(node.Id))
                {   // Cycle detected.
                    return true;
                }
            }

            // No cycles detected.
            return false;
        }

        private bool TraverseNode(uint nodeId)
        {
            // Is the node on the current stack of traversal ancestor nodes?
            if(_ancestorNodeSet.Contains(nodeId))
            {   // Connectivity cycle detected.
                return true;
            }

            // Have we already traversed this node?
            if(_visitedNodeSet.Contains(nodeId))
            {   // Already visited; Skip.
                return false;
            }

            // Traverse into the node's targets / children (if it has any)
            NodeConnectivityData node = _networkConnectivityData.GetNodeDataById(nodeId);
            if(0 == node._tgtNodes.Count) 
            {   // No cycles on this traversal path.
                return false;
            }

            // Register node with set of traversal path ancestor nodes.
            _ancestorNodeSet.Add(nodeId);

            // Register the node as having been visited.
            _visitedNodeSet.Add(nodeId);

            // Traverse into targets.
            foreach(uint targetId in node._tgtNodes)
            {
                if(TraverseNode(targetId)) 
                {   // Cycle detected.
                    return true;
                }
            }
            
            // Remove node from set of traversal path ancestor nodes.
            _ancestorNodeSet.Remove(nodeId);

            // No cycles were detected in the traversal paths from this node.
            return false;
        }

        #endregion
    }
}
