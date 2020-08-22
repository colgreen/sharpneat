using System;
using System.Linq;
using System.Threading;
using Redzen;
using Redzen.Structures;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.Graphs
{

    // TODO: Unit tests.

    /// <summary>
    /// An algorithm for calculating the depth of each node in an cyclic graph.
    /// 
    /// Input nodes are defined as being at depth 0, the depth of all other nodes is defined as 
    /// the maximum number of hops to each node from an input node. I.e. where multiple paths exist to a
    /// node (potentially each with a different numbers of hops), the node's depth is defined by the path 
    /// with the most number of hops.
    /// 
    /// The graph traversal algorithm uses function recursion. A number of other classes in SharpNEAT perform
    /// graph traversal by using a separate traversal stack (stored on the heap); that approach is faster but
    /// more complex, thus this class has not been converted to the faster approach because it is not directly 
    /// used in the evolutionary algorithm. At time of writing this class is used only for graph visualization.
    /// </summary>
    public class CyclicGraphDepthAnalysis
    {
        #region Instance Fields

        /// <summary>
        /// The directed graph being tested.
        /// </summary>
        DirectedGraph? _digraph;

        /// <summary>
        /// A bitmap in which each bit represents a node in the graph. 
        /// The set bits represent the set of nodes that are ancestors of the current traversal node.
        /// </summary>
        BoolArray _ancestorNodeBitmap = new BoolArray(1024);

        /// <summary>
        /// An integer array in which each element represents a node in the graph.
        ///
        /// The non-zero elements represent the set of nodes that have been visited by either the current traversal,
        /// or previous traversals (i.e. starting from a different input node).
        /// 
        /// This is used to quickly determine if a given path needs to be traversed or not, i.e. if a path has 
        /// previously been traversed, *and* its assigned depth is greater than or equal to the current traversal depth,
        /// then we do not need to traverse this pathway again.
        /// 
        /// The integer values represent the depth of each node. Zeros indicate a node that has no incoming connections,
        /// therefore input nodes always remain set to zero.
        /// </summary>
        int[]? _nodeDepthByIdx;

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
        /// Calculate node depths in a cyclic network.
        /// </summary>
        public GraphDepthInfo CalculateNodeDepths(DirectedGraph digraph)
        {
            #if DEBUG
            // Check for attempts to re-enter this method.
            if(Interlocked.CompareExchange(ref _reentranceFlag, 1, 0) == 1) {
                throw new InvalidOperationException("Attempt to re-enter non reentrant method.");
            }
            #endif

            _digraph = digraph;
            EnsureNodeCapacity(digraph.TotalNodeCount);

            try
            {
                // Loop over input nodes. Take each one in turn as a traversal root node.
                int inputCount = _digraph.InputCount;
                for(int nodeIdx=0; nodeIdx < inputCount; nodeIdx++)
                {
                    // Determine if the node has already been visited.
                    if(_nodeDepthByIdx![nodeIdx] != 0)
                    {   // Already traversed; Skip.
                        continue;
                    }

                    // Traverse into the current inputs node's target nodes, with the depth value
                    // to assign to those nodes.
                    TraverseNode(nodeIdx, 1);
                }

                // Return depth analysis info.
                return new GraphDepthInfo(_nodeDepthByIdx.Max()+1, _nodeDepthByIdx!);
            }
            finally
            {
                Cleanup();
            }
        }

        #endregion

        #region Private Methods

        private void TraverseNode(int nodeIdx, int depth)
        {
            // Is the node on the current stack of traversal ancestor nodes?
            if(_ancestorNodeBitmap[nodeIdx])
            {   // Connectivity cycle detected; don't traverse into this pathway.
                return;
            }

            // Have we already traversed this node? And if so, was the depth assigned to it greater than the current traversal depth?
            // If so we can skip traversal into this node, as we could not assign it, or any of its descendants, a greater depth than
            // it already has.
            if(_nodeDepthByIdx![nodeIdx] >= depth) {
                return;
            }

            // Traverse into the node's targets / children (if it has any)
            int connIdx = _digraph!.GetFirstConnectionIndex(nodeIdx);
            if(connIdx == -1) 
            {   // No target nodes to traverse.
                return;
            }

            // Add node to the set of traversal path nodes.
            _ancestorNodeBitmap[nodeIdx] = true;

            // Register the node as having been visited.
            _nodeDepthByIdx![nodeIdx] = depth;

            // Traverse into target nodes.
            int[] srcIdxArr = _digraph.ConnectionIdArrays._sourceIdArr;

            for(; connIdx < srcIdxArr.Length && srcIdxArr[connIdx] == nodeIdx; connIdx++)
            {
                TraverseNode(_digraph.GetTargetNodeIdx(connIdx), depth + 1);
            }
            
            // Remove node from set of traversal path nodes.
            _ancestorNodeBitmap[nodeIdx] = false;
        }

        private void EnsureNodeCapacity(int requiredCapacity)
        {
            if(requiredCapacity > _ancestorNodeBitmap.Length)
            {
                // For the new capacity, select the lowest power of two that is above the required capacity;
                // this limits the number of capacity increases as the required capacity increases.
                requiredCapacity = MathUtils.CeilingToPowerOfTwo(requiredCapacity);

                // Allocate new bitmaps with the new capacity.
                _ancestorNodeBitmap = new BoolArray(requiredCapacity);
                _nodeDepthByIdx = new int[requiredCapacity];
            }
        }

        private void Cleanup()
        {
            _digraph = null;
            _ancestorNodeBitmap.Reset(false);
            _nodeDepthByIdx = null;

            #if DEBUG
            // Reset reentrancy test flag.
            Interlocked.Exchange(ref _reentranceFlag, 0);
            #endif
        }

        #endregion
    }
}
