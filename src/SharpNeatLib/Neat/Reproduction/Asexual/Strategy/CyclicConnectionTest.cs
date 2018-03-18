using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Redzen.Collections;
using SharpNeat.Network;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    /// <summary>
    /// For testing if a proposed new connection on a NEAT genome would form a connectivity cycle.
    /// </summary>
    /// <remarks>
    /// Each instance of this class allocates a stack and a hashset for use by the traversal algorithm, and these
    /// are cleared and re-used for each call to IsConnectionCyclic(). This avoids memory re-allocation and garbage
    /// collection overhead, but the side effect is that IsConnectionCyclic() is not thread safe. However, A thread 
    /// safe static method IsCyclicStatic() is provided for convenience, but this will have the additional memory
    /// alloc and GC overhead associated with each call to it.
    /// 
    /// This class is optimized for speed and efficiency and as such is tightly coupled with the connection gene 
    /// array data structure, and is perhaps not as easy to read/understand as a traditional depth first graph traversal 
    /// algorithm using function recursion. However this is essentially a depth first algorithm but with its own stack 
    /// instead of using the call stack, and each stack frame is just an index into the connection array.
    /// 
    /// The idea is that an entry on the stack represents both a node that is being traversed (given by the current 
    /// connection's source node) and an iterator over that node's target nodes (given by the connection index, which 
    /// works because connections are sorted by sourceId).
    /// 
    /// The main optimizations then are:
    /// 
    ///    * No method call overhead from recursive method calls.
    ///    
    ///    * Each stack frame is a single int32 and thus the stack as a whole is highly compact; this improves CPU cache
    ///      locality and hit rate, and also which keeps the max size of the stack for any given traversal at a minimum.
    ///      
    ///    * The stack and a visitedNodes hashset are allocated for each class instance and are cleared and re-used for each 
    ///       call to IsConnectionCyclic(), therefore avoiding memory allocation and garbage collection overhead.
    /// 
    /// Using our own stack also avoids any potential for a stack overflow on very deep graphs, which could occur if using 
    /// method call recursion.
    /// 
    /// Problems with the approach of this class are:
    /// 
    ///    * The code is more complex than the same algorithm written as a recursive function; this makes the code harder 
    ///      to read and understand and thus increases the probability of subtle defects, and makes the code harder to maintain.
    ///      
    ///    * Currently the custom stack is allocated on the heap, and this could improved by using stackalloc of a Span{int}
    ///      at time of writing that option is not available in the current target framework (.NET Standard 2.0).
    ///
    /// </remarks>
    public class CyclicConnectionTest
    {
        #region Instance Fields

        // ENHANCEMENT: Use stackalloc of a Span<int> instead of a re-usable heap based stack.
        /// <summary>
        /// The graph traversal stack, as required by a depth first graph traversal algorithm.
        /// Each stack entry is an index into a connection array, representing iteration over the connections 
        /// for one source node.
        /// </summary>
        IntStack _traversalStack = new IntStack(16);    

        // ENHANCEMENT: Assign an initial capacity when that becomes possible (i.e. possibly in .NET Standard 2.1)
        /// <summary>
        /// Maintain a set of nodes that have been visited, this allows us to avoid unnecessary
        /// re-traversal of nodes.
        /// </summary>
        HashSet<int> _visitedNodes = new HashSet<int>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests if the proposed new connection newConn would form a cycle if added to the existing directed
        /// acyclic graph described by connArr.
        /// </summary>
        /// <param name="connArr">A set of connections that describe a directed acyclic graph.</param>
        /// <param name="newConn">A proposed new connection to add to the graph.</param>
        public bool IsConnectionCyclic(IList<DirectedConnection> connArr, DirectedConnection newConn)
        {
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

        private bool IsConnectionCyclicInner(IList<DirectedConnection> connArr, DirectedConnection newConn)
        {
            // Test if the new connection is pointing to itself.
            if(newConn.SourceId == newConn.TargetId) {
                return true;
            }

            // Initialise traversal.
            // Notes. 
            // We traverse forwards starting at the new connection's target node. If the new connection's source node is encountered
            // during traversal then the connection would form a cycle in the graph as a whole.

            // The 'terminal' node ID, i.e. if traversal reaches this node then newConn would form a cycle and we stop/terminate traversal.
            int terminalNodeId = newConn.SourceId;

            // Init the current traversal node, i.e. we start at newConn.TargetId.
            int currNodeId = newConn.TargetId;
            
            // Search for outgoing connections from the current node.
            int connStartIdx = DirectedConnectionUtils.GetConnectionIndexBySourceNodeId(connArr, currNodeId);
            if(connStartIdx < 0)
            {   // The current node has no outgoing connections, therefore newConn does not form a cycle.
                return false;
            }

            // Push connStartIdx onto the stack.
            _traversalStack.Push(connStartIdx);

            // Add the current node to the set of visited nodes.
            _visitedNodes.Add(currNodeId);

            // While there are entries on the stack.
            while(0 != _traversalStack.Count)
            {
                // Get the connection index from the top of stack; this is the next connection to be traversed.
                int currConnIdx = _traversalStack.Peek();

                // Before we traverse the current connection, update the stack state to point to the next connection
                // to be traversed. I.e. set up the stack state ready for when the traversal down the current 
                // connection completes.
                MoveForward(connArr, currConnIdx);

                // Test if the next traversal child node has already been visited.
                int childNodeId = connArr[currConnIdx].TargetId;
                if(_visitedNodes.Contains(childNodeId)) {
                    continue;
                }

                // Test if the connection target is the terminal node.
                if(childNodeId == terminalNodeId) {
                    return true;
                }

                // We're about to traverse into childNodeId, so mark it as visited to prevent re-traversal.
                _visitedNodes.Add(childNodeId);

                // Search for outgoing connections from childNodeId.
                connStartIdx = DirectedConnectionUtils.GetConnectionIndexBySourceNodeId(connArr, childNodeId);
                if(connStartIdx >= 0)
                {   // childNodeId has outgoing connections from it. Push the first connection onto the stack.
                    _traversalStack.Push(connStartIdx);    
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MoveForward(IList<DirectedConnection> connArr, int currConnIdx)
        {
            // Get the node currently being traversed, as indicated by the current connection's source node ID.
            int currNodeId = connArr[currConnIdx].SourceId;

            // Find the next connection from the current node that we can traverse down, if any.
            for(int nextConnIdx = currConnIdx+1; nextConnIdx < connArr.Count && connArr[nextConnIdx].SourceId == currNodeId; nextConnIdx++)
            {
                if(!_visitedNodes.Contains(connArr[nextConnIdx].TargetId))
                {
                    // We have found the next connection to traverse for the current node;
                    // update the current node's entry on the top of the stack to point to it.
                    _traversalStack.Poke(nextConnIdx);
                    return;
                }
            }

            // No more connections for the current node; pop/remove its entry from the top of the stack;
            // traversal will thus continue from the parent node's current position, or will terminate 
            // if the stack is now empty.
            _traversalStack.Pop();
        }

        private void Cleanup()
        {
            _traversalStack.Clear();
            _visitedNodes.Clear();
        }

        #endregion
    }
}
