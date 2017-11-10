using System.Collections.Generic;

namespace SharpNeat.Network
{
    /// <summary>
    /// An algorithm for testing for the presence of at least one connectivity cycle within a network.
    /// 
    /// Method.
    /// =======
    /// 1) We loop over all nodes in the network and perform a depth-first traversal from each node. 
    /// (Note. the order that the nodes are traversed does not affect the correctness of the method)
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
    /// attempt to maintain algorithmic efficiency by avoiding traversal into nodes that have already been 
    /// traversed into.
    /// </summary>
    public class CyclicGraphAnalysis
    {
        #region Instance Fields

        /// <summary>
        /// The directed graph being tested.
        /// </summary>
        DirectedGraph _digraph;

        // TODO: Re-use these hashsets to avoid the overhead of re-allocation and garbage collection. See CyclicConnectionTest class

        /// <summary>
        /// Set of traversal ancestors of current node. 
        /// </summary>
        HashSet<int> _ancestorNodeSet = new HashSet<int>();
        /// <summary>
        /// Set of all visited nodes. This allows us to quickly determine if a path should be traversed or not. 
        /// </summary>
        HashSet<int> _visitedNodeSet = new HashSet<int>();

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor. Prevents construction from outside of this class.
        /// </summary>
        private CyclicGraphAnalysis(DirectedGraph digraph)
        {
            _digraph = digraph;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Returns true if there is at least one connectivity cycle within the provided DirectedGraph.
        /// </summary>
        public static bool IsCyclic(DirectedGraph digraph)
        {
            return new CyclicGraphAnalysis(digraph).IsCyclicInternal();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns true if there is at least one connectivity cycle within the provided INetworkDefinition.
        /// </summary>
        private bool IsCyclicInternal()
        {
            // Loop over all nodes. Take each one in turn as a traversal root node.
            int nodeCount = _digraph.TotalNodeCount;
            for(int nodeId=0; nodeId < nodeCount; nodeId++)
            {
                // Determine if the node has already been visited.
                if(_visitedNodeSet.Contains(nodeId)) 
                {   // Already traversed; Skip.
                    continue;
                }

                // Traverse into the node. 
                if(TraverseNode(nodeId))
                {   // Cycle detected.
                    return true;
                }
            }

            // No cycles detected.
            return false;
        }

        private bool TraverseNode(int nodeId)
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
            IList<int> tgtIdArr = _digraph.GetConnections(nodeId);

            if(0 == tgtIdArr.Count) 
            {   // No cycles on this traversal path.
                return false;
            }

            // Register node with set of traversal path ancestor nodes.
            _ancestorNodeSet.Add(nodeId);

            // Register the node as having been visited.
            _visitedNodeSet.Add(nodeId);

            // Traverse into targets.
            for(int i=0; i<tgtIdArr.Count; i++)
            {
                if(TraverseNode(tgtIdArr[i])) 
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
