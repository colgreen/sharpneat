using System.Collections.Generic;
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
    ///    1) No method call overhead from recursive method calls.
    ///    2) Each stack frame is a single int32, which keeps the max size of the stack for any given traversal at a minimum.
    ///    3) The stack and a visitedNodes hashset are allocated for each class instance and are cleared and re-used for each 
    ///       call to IsConnectionCyclic(), therefore avoiding memory allocation and garbage collection overhead.
    /// 
    /// Using our own stack also avoids any potential for a stack overflow on very deep graphs, which could occur if using 
    /// method call recursion.
    /// </remarks>
    /// <typeparam name="T">Connection weight type.</typeparam>
    public class CyclicConnectionTest<T> where T : struct
    {
        #region Instance Fields

        /// <summary>
        /// The graph traversal stack, as required by a depth first graph traversal algorithm.
        /// Each stack entry is an index into a connection array, representing iteration over the connections 
        /// for one source node.
        /// </summary>
        IntStack _traversalStack = new IntStack(16);    
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
        public bool IsConnectionCyclic(DirectedConnection[] connArr, DirectedConnection newConn)
        {
            try 
            {
                return IsConnectionCyclicInner(connArr, newConn);
            }
            finally 
            {   // Ensure cleanup occurs before we return so that we can guarantee the class instance is ready for 
                // re-use on the next call.
                _traversalStack.Clear();
                _visitedNodes.Clear();
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

            // Initialise traversal.
            // Notes. 
            // We traverse forwards starting at the new connection's target node. If the new connection's source node is encountered
            // during traversal then the new connection would form a cycle in the graph as a whole.

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
                // Get the connection at the top of the stack.
                // This determines the current traversal node and the iteration position through its outgoing connections.
                int connIdx = _traversalStack.Peek();
                currNodeId = connArr[connIdx].SourceId;

                // Find the next connection from the current node that we can traverse down, if any.
                int nextConnIdx = -1;
                for(int i = connIdx+1; i < connArr.Length && connArr[i].SourceId == currNodeId; i++)
                {
                    if(!_visitedNodes.Contains(connArr[i].TargetId))
                    {   // We have found the next connection to traverse.
                        nextConnIdx = i;
                        break;
                    }
                }

                // Move/iterate to the next connection for the current node, if any.
                if(-1 != nextConnIdx) 
                {   // We have the next connection to traverse down for the current node; update the current node's
                    // entry on the top of the stack to point to it.
                    _traversalStack.Poke(nextConnIdx);
                }
                else
                {   // No more connections for the current node; remove its entry from the top of the stack.
                    _traversalStack.Pop();
                }

                // Test if the next traversal child node has already been visited.
                int childNodeId = connArr[connIdx].TargetId;
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

        #endregion
    }
}
