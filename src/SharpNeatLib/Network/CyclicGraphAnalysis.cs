using System;
using System.Threading;
using Redzen;
using Redzen.Structures;

namespace SharpNeat.Network
{
    /// <summary>
    /// An algorithm for testing whether a given graph is cyclic or acyclic, i.e. does a given graph have 
    /// a connectivity cycle.
    /// 
    /// Method.
    /// =======
    /// 1) We loop over all nodes in the network and perform a depth-first traversal from each node. 
    /// (Note. the order that the nodes are traversed does not affect the correctness of the method)
    /// 
    /// 2) Each traversal keeps track of its ancestor nodes (the path to the current node) at each step
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

        /// <summary>
        /// A bitmap in which each bit represents a node in the graph. 
        /// The set bits represent the set of nodes that are ancestors of the current traversal node.
        /// </summary>
        BoolArray _ancestorNodeBitmap = new BoolArray(1024);

        /// <summary>
        /// A bitmap in which each bit represents a node in the graph. 
        /// The set bits represent the set of visited nodes on the current traversal path.
        /// This is used to quickly determine if a given path should be traversed or not. 
        /// </summary>
        BoolArray _visitedNodeBitmap = new BoolArray(1024);

        #if DEBUG
        /// <summary>
        /// Indicates if a call to IsCyclic() is currently in progress. 
        /// For checking for attempts to re-enter that method while a call is in progress.
        /// </summary>
        int _reentranceFlag = 0;
        #endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns true if there is at least one connectivity cycle within the provided DirectedGraph.
        /// </summary>
        public bool IsCyclic(DirectedGraph digraph)
        {
            #if DEBUG
            // Check for attempts to re-enter this method.
            if(1 == Interlocked.CompareExchange(ref _reentranceFlag, 1, 0)) {
                throw new InvalidOperationException("Attempt to re-enter non reentrant method.");
            }
            #endif

            _digraph = digraph;
            EnsureNodeCapacity(digraph.TotalNodeCount);

            try
            {
                // Loop over all nodes. Take each one in turn as a traversal root node.
                int nodeCount = _digraph.TotalNodeCount;
                for(int nodeIdx=0; nodeIdx < nodeCount; nodeIdx++)
                {
                    // Determine if the node has already been visited.
                    if(_visitedNodeBitmap[nodeIdx])
                    {   // Already traversed; Skip.
                        continue;
                    }

                    // Traverse into the node. 
                    if(TraverseNode(nodeIdx))
                    {   // Cycle detected.
                        return true;    
                    }
                }

                // No cycles detected.
                return false;
            }
            finally
            {
                Cleanup();
            }
        }

        #endregion

        #region Private Methods

        private void EnsureNodeCapacity(int requiredCapacity)
        {
            if(requiredCapacity > _ancestorNodeBitmap.Length)
            {
                // For the new capacity, select the lowest power of two that is above the required capacity;
                // this limits the number of capacity increases as the required capacity increases.
                requiredCapacity = MathUtils.CeilingToPowerOfTwo(requiredCapacity);

                // Allocate new bitmaps with the new capacity.
                _ancestorNodeBitmap = new BoolArray(requiredCapacity);
                _visitedNodeBitmap = new BoolArray(requiredCapacity);
            }
        }

        private bool TraverseNode(int nodeIdx)
        {
            // Is the node on the current stack of traversal ancestor nodes?
            if(_ancestorNodeBitmap[nodeIdx])
            {   // Connectivity cycle detected.
                return true;
            }

            // Have we already traversed this node?
            if(_visitedNodeBitmap[nodeIdx])
            {   // Already visited; Skip.
                return false;
            }

            // Traverse into the node's targets / children (if it has any)
            int connIdx = _digraph.GetFirstConnectionIndex(nodeIdx);
            if(-1 == connIdx) 
            {   // No target nodes to traverse, therefore no cycles on this traversal path.
                return false;
            }

            // Add node to the set of traversal path nodes.
            _ancestorNodeBitmap[nodeIdx] = true;

            // Register the node as having been visited.
            _visitedNodeBitmap[nodeIdx] = true;

            // Traverse into targets.
            int[] srcIdxArr = _digraph.ConnectionIdArrays._sourceIdArr;

            for(; connIdx < srcIdxArr.Length && srcIdxArr[connIdx] == nodeIdx; connIdx++)
            {
                if(TraverseNode(_digraph.GetTargetNodeIdx(connIdx))) 
                {   // Cycle detected.
                    return true;
                }
            }
            
            // Remove node from set of traversal path nodes.
            _ancestorNodeBitmap[nodeIdx] = false;

            // No cycles were detected in the traversal paths from this node.
            return false;
        }

        private void Cleanup()
        {
            _digraph = null;
            _ancestorNodeBitmap.Reset(false);
            _visitedNodeBitmap.Reset(false);

            #if DEBUG
            // Reset reentrancy test flag.
            Interlocked.Exchange(ref _reentranceFlag, 0);
            #endif
        }

        #endregion
    }
}
