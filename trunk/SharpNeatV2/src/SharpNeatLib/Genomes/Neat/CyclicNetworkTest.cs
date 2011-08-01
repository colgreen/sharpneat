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

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// Logic to determine if the network described by a NeatGenome is cyclic (has at least one cycle in its connectivity).
    /// 
    /// Method. We perform a depth first traversal of the network, starting from each neuron in turn as it
    /// appears in _neuronGeneList. Hence we traverse from the bias and input node's first, followed by the
    /// output and then hidden nodes (this order is not important for the correctness of the method).
    /// During traversal we maintain a stack of all ancestor nodes of the current traversal node (a single 
    /// pathway from the start node to the current node, but not necessarily the only pathway between those two nodes).
    /// When traversing to a new child/target node we check:
    /// (A) Has the node already been visted (via another traversal path). If so we do no traverse into that node.
    /// (B) Is the node in ancestorNodeStack. This would indicate that a cycle exists and we return true.
    /// </summary>
    public class CyclicNetworkTest
    {
        #region Instance Fields

        /// <summary>
        /// The NeatGenome that is currently being tested.
        /// </summary>
        NeatGenome _neatGenome;
        /// <summary>
        /// Set of traversal ancestors of current node. 
        /// </summary>
        HashSet<uint> _ancestorNodeSet = new HashSet<uint>();
        /// <summary>
        /// Set of all visted nodes. This allows us to quickly determine if a path should be traversed or not. 
        /// </summary>
        HashSet<uint> _visitedNodeSet = new HashSet<uint>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns true if the network described by the specified genome is cyclic (has at least one cycle in the connectivity).
        /// </summary>
        public bool IsNetworkCyclic(NeatGenome neatGenome)
        {
            // Clear any existing state (allow reuse).
            _ancestorNodeSet.Clear();
            _visitedNodeSet.Clear();
            
            // Store ref to genome.
            _neatGenome = neatGenome;

            // Loop neurons. Take each one in turn as a traversal root node.
            foreach(NeuronGene neuronGene in neatGenome.NeuronGeneList)
            {
                // Determine if the node has already been visited.
                if(_visitedNodeSet.Contains(neuronGene.Id)) 
                {   // Already traversed. Skip.
                    continue;
                }

                // Traverse into the node. 
                if(TraverseNode(neuronGene.Id))
                {   // Cycle detected.
                    return true;
                }
            }

            // No cycles detected.
            return false;
        }

        #endregion

        #region Private Methods

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
            NeuronGene node = _neatGenome.NeuronGeneList.GetNeuronById(nodeId);
            if(0 == node.TargetNeurons.Count) 
            {   // No cycles on this traversal path.
                return false;
            }

            // Register node with set of traversal path ancestor nodes.
            _ancestorNodeSet.Add(nodeId);

            // Register the node as having been visited.
            _visitedNodeSet.Add(nodeId);

            // Traverse into targets.
            foreach(uint targetId in node.TargetNeurons)
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
