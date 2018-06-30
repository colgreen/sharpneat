using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Redzen;
using Redzen.Collections;
using Redzen.Structures;
using SharpNeat.Network;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    /// <summary>
    /// For testing if a proposed new connection on a NEAT genome would form a connectivity cycle.
    /// </summary>
    /// <remarks>
    /// The algorithm utilises a depth first traversal of the graph but using its own traversal stack
    /// data structure instead of relying on function recursion and the call stack. This is an optimisation,
    /// for more details see the comments on: 
    /// <see cref="SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover.CyclicConnectionTest"/>.
    /// 
    /// This is a minor variant on that class. The key difference in this class is that IsConnectionCyclic()
    /// is passed a function that maps from node IDs to node indexes. These indexes represent all of the nodes
    /// in the graph in a contiguous span starting at zero, and therefore the _visitedNodes structure can be 
    /// implemented as a compact BoolArray instead of a HashSet of nodeIDs.
    /// 
    /// This is a more efficient approach *if* the mapping function already exists, i.e. is readily 
    /// available because it is required for other purposes. Otherwise the cost of constructing the 
    /// mapping may outweigh the benefits of using this class.
    /// 
    /// Also see:
    /// <see cref="SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover.CyclicConnectionTest"/>
    /// <see cref="SharpNeat.Network.Acyclic.CyclicConnectionTest"/>
    /// <see cref="SharpNeat.Network.Acyclic.AcyclicGraphDepthAnalysis"/>
    /// <see cref="SharpNeat.Network.CyclicGraphAnalysis"/>
    /// </remarks>
    public class CyclicConnectionTest
    {
        #region Instance Fields

        /// <summary>
        /// The graph traversal stack, as required by a depth first graph traversal algorithm.
        /// Each stack entry is an index into a connection list, representing both the current node being traversed 
        /// (the connections's source ID), and the current position in that node's outgoing connections.
        /// for one source node.
        /// </summary>
        IntStack _traversalStack = new IntStack(16);    

        /// <summary>
        /// Maintain a set of nodes that have been visited, this allows us to avoid unnecessary
        /// re-traversal of nodes.
        /// </summary>        
        BoolArray _visitedNodes = new BoolArray(1024);

        #if DEBUG
        /// <summary>
        /// Indicates if a call to IsConnectionCyclic() is currently in progress.
        /// For checking for attempts to re-enter that method while a call is in progress.
        /// </summary>
        int _reentranceFlag = 0;
        #endif

        /// <summary>
        /// A function that implements a mapping from node IDs to node indexes.
        /// </summary>
        INodeIdMap _nodeIdxByIdMap;

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests if the proposed new connection newConn would form a cycle if added to the existing directed
        /// acyclic graph described by connArr.
        /// </summary>
        /// <param name="connArr">A set of connections that describe a directed acyclic graph.</param>
        /// <param name="newConn">A proposed new connection to add to the graph.</param>
        public bool IsConnectionCyclic(
            DirectedConnection[] connArr,
            INodeIdMap nodeIdxByIdMap,
            int totalNodeCount,
            DirectedConnection newConn)
        {
            #if DEBUG
            // Check for attempts to re-enter this method.
            if(1 == Interlocked.CompareExchange(ref _reentranceFlag, 1, 0)) {
                throw new InvalidOperationException("Attempt to re-enter non reentrant method.");
            }
            #endif

            EnsureNodeCapacity(totalNodeCount);
            _nodeIdxByIdMap = nodeIdxByIdMap;

            try 
            {
                return IsConnectionCyclicInner(connArr, newConn);
            }
            finally 
            {   // Ensure cleanup occurs before we return so that we can guarantee the class instance is ready for 
                // re-use on the next call.
                Cleanup();
            }
        }

        #endregion

        #region Private Methods

        private bool IsConnectionCyclicInner(DirectedConnection[] connArr, DirectedConnection newConn)
        {
            // Test if the new connection is pointing to itself.
            if(newConn.SourceId == newConn.TargetId) {
                return true;
            }

            // Note. We traverse forwards starting at the new connection's target node. If the new connection's source node is
            // encountered during traversal then the connection would form a cycle in the graph as a whole, and we return true.
            int startNodeId = newConn.TargetId;

            // Search for outgoing connections from the starting node.
            // ENHANCEMENT: Eliminate binary search for childNodeId.
            int connIdx = DirectedConnectionUtils.GetConnectionIndexBySourceNodeId(connArr, startNodeId);
            if(connIdx < 0)
            {   // The current node has no outgoing connections, therefore newConn does not form a cycle.
                return false;
            }

            // Initialise and run the graph traversal algorithm.
            InitGraphTraversal(startNodeId, connIdx);

            // Note. we pass newConn.SourceId as the terminalNodeId; if traversal reaches this node then a cycle has been detected.
            return TraverseGraph(connArr, newConn.SourceId);
        }

        private void EnsureNodeCapacity(int capacity)
        {
            if (capacity > _visitedNodes.Length)
            {
                // For the new capacity, select the lowest power of two that is above the required capacity.
                capacity = MathUtils.CeilingToPowerOfTwo(capacity);

                // Allocate new bitmap with the new capacity.
                _visitedNodes = new BoolArray(capacity);
            }
        }

        private void Cleanup()
        {
            _traversalStack.Clear();
            _visitedNodes.Reset(false);
            _nodeIdxByIdMap = null;

            #if DEBUG
            // Reset reentrancy test flag.
            Interlocked.Exchange(ref _reentranceFlag, 0);
            #endif
        }

        #endregion

        #region Private Methods [Graph Traversal]

        private void InitGraphTraversal(int startNodeId, int connIdx)
        {
            // Push connIdx onto the stack.
            _traversalStack.Push(connIdx);

            // Add the current node to the set of visited nodes; this prevents the traversal algorithm from re-entering this node 
            // (it's on the stack thus it is in the process of being traversed).
            _visitedNodes[_nodeIdxByIdMap.Map(startNodeId)] = true;
        }

        /// <summary>
        /// The graph traversal algorithm.
        /// </summary>
        /// <param name="connArr">A set of connections that represents the graph to traverse.</param>
        /// <param name="terminalNodeId">// The 'terminal' node ID, i.e. if traversal reaches this node then newConn would form a cycle and we stop/terminate traversal.</param>
        /// <returns></returns>
        private bool TraverseGraph(DirectedConnection[] connArr, int terminalNodeId)
        {
            // While there are entries on the stack.
            while(0 != _traversalStack.Count)
            {
                // Get the connection index from the top of stack; this is the next connection to be traversed.
                int currConnIdx = _traversalStack.Peek();

                // Notes.
                // Before we traverse the current connection, update the stack state to point to the next connection to be
                // traversed, either from the current node or a parent node. I.e. we modify the stack state  ready for when
                // the traversal down into the current connection completes and returns back to the current node.
                //
                // This approach results in tail call optimisation and thus will result in a shallower stack on average. It 
                // also has the side effect that we can no longer examine the stack to observe the traversal path at a given
                // point in time, since some of the path may no longer be on the stack.
                MoveForward(connArr, currConnIdx);

                // Test if the next traversal child node has already been visited.
                int childNodeId = connArr[currConnIdx].TargetId;
                int childNodeIdx = _nodeIdxByIdMap.Map(childNodeId);
                if(_visitedNodes[childNodeIdx]) {
                    continue;
                }

                // Test if the connection target is the terminal node.
                if(childNodeId == terminalNodeId) {
                    return true;
                }

                // We're about to traverse into childNodeId, so mark it as visited to prevent re-traversal.
                _visitedNodes[childNodeIdx] = true;

                // Search for outgoing connections from childNodeId.
                // ENHANCEMENT: Eliminate binary search for childNodeId.
                int connIdx = DirectedConnectionUtils.GetConnectionIndexBySourceNodeId(connArr, childNodeId);
                if(connIdx >= 0)
                {   // childNodeId has outgoing connections; push the first connection onto the stack to mark it for traversal.
                    _traversalStack.Push(connIdx);    
                }
            }

            // Traversal has completed without visiting the terminal node, therefore the new connection
            // does not form a cycle in the graph.
            return false;
        }

        /// <summary>
        /// Update the stack state to point to the next connection to traverse down.
        /// </summary>
        /// <returns>The current connection to traverse down.</returns>
        private void MoveForward(DirectedConnection[] connArr, int currConnIdx)
        {
            // If the current node has at least one more outgoing connection leading to an unvisited node,
            // then update the node's entry on the top of the stack to point to said connection.
            for(int i=currConnIdx + 1; i < connArr.Length && (connArr[currConnIdx].SourceId == connArr[i].SourceId); i++)
            {
                if(!_visitedNodes[connArr[i].TargetId])
                {
                    _traversalStack.Poke(currConnIdx + 1);
                    return;
                }
            }

            // No more connections for the current node; pop/remove the current node from the top of the stack.
            // Traversal will thus continue from its traversal parent node's current position, or will terminate 
            // if the stack is now empty.
            _traversalStack.Pop();
        }

        #endregion
    }
}
