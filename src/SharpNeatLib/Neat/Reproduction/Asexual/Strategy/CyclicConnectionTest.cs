using System.Collections.Generic;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public class CyclicConnectionTest<T> where T : struct
    {
        #region Instance Fields

        // Allocated at the class level so that the storage they allocate can be re-used, i.e. this is an attempt
        // to reduce memory allocation and garbage collection overhead on the cycle test code that is heavily used.
        // The capacity of these objects will grow to fit the maximum size required of them, and once that capacity 
        // is achieved no further memory allocation overhead is required.

        /// <summary>
        /// Maintain a set of nodes that have been visited, this allows us to avoid unnecessary
        /// re-traversal of nodes.
        /// </summary>
        HashSet<int> _visitedNodes = new HashSet<int>();

        /// <summary>
        /// A stack of nodes to be visited.
        /// </summary>
        Stack<int> _traversalStack = new Stack<int>(100);

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests if the graph described by connArr and newConn contains a cycle. 
        /// connArr describes an acyclic graph, and we are testing if newConn would create a cycle
        /// if it were added connArr.
        /// </summary>
        /// <param name="connArr">A set of connections that describe an acyclic graph.</param>
        /// <param name="newConn">A proposed new connection to add to the graph.</param>
        /// <remarks>
        /// This search uses an explicitly created stack instead of using function recursion, the main reason for doing this is to avoid
        /// any limit on the depth of stack we can use, i.e. in a graph with thousands of nodes to traverse we may run out of space on the 
        /// function call stack (stack overflow), so this is a more robust approach.
        /// </remarks>
        public bool IsConnectionCyclic(ConnectionGene<T>[] connArr, DirectedConnection newConn)
        {
            // Test if the new connection is pointing to itself.
            if(newConn.SourceId == newConn.TargetId) {
                return true;
            }

            // Traverse forwards starting at the new connection's target node.
            // If the new connection's source node is encountered then the new connection would
            // form a cycle in the graph as a whole.
            Cleanup();

            // Initialise traversal.
            // Notes. 
            // The starting point for traversal is newConn.TargetId, and a cycle is detected if traversal reaches newConn.SourceId.
            // However we already asserted that newConn.SourceId != newConn.TargetId (above), therefore we add newConn.TargetId to
            // visitedNodes to indicate that it has already been visited, and place its target nodes into the stack of nodes to traverse
            // (traversalStack). Essentially we're starting the traversal one level on to avoid a redundant test (i.e. for slightly 
            // improved performance).
            foreach(int targetId in GetTargetNodeIds(connArr, newConn.TargetId)) {
                _traversalStack.Push(targetId);
            }

            // While there are nodes to check/traverse.
            while(0 != _traversalStack.Count)
            {
                // Pop a node from the top of the stack.
                int currNodeId = _traversalStack.Pop();

                // Test if this node has already been traversed.
                if(_visitedNodes.Contains(currNodeId)) {
                    // Already visited (via a different route).
                    continue;
                }

                // Test if traversal has arrived at the new connection's source node, if so then a cycle has been detected.
                if(currNodeId == newConn.SourceId) 
                {
                    Cleanup();
                    return true;
                }

                // Register the visit of this node.
                _visitedNodes.Add(currNodeId);                

                // Push the current nodes's target nodes onto the traversal stack.
                foreach(int targetId in GetTargetNodeIds(connArr, currNodeId))
                {
                    // Avoid pushing already visited nodes.
                    if(!_visitedNodes.Contains(targetId)) {
                        _traversalStack.Push(targetId);
                    }

                    _traversalStack.Push(targetId);
                }
            }
            
            // Traversal has completed without visiting the new connection's source node, therefore the new connection
            // does not form a cycle in the graph.
            Cleanup();
            return false;
        }

        #endregion

        #region Private Methods

        private void Cleanup()
        {
            _visitedNodes.Clear();
            _traversalStack.Clear();
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Get an array of all target node IDs for the given source node ID.
        /// </summary>
        private static IEnumerable<int> GetTargetNodeIds(ConnectionGene<T>[] connArr, int srcNodeId)
        {
            // Search for a connection with the given source node ID.
            int connIdx = ConnectionGeneUtils.GetConnectionIndexBySourceNodeId(connArr, srcNodeId);

            // Test for no match, i.e. no connections with the given source node ID.
            if(connIdx < 0) {   
                yield break;
            }

            // Yield the index of each connection with the given source node ID.
            for(; connIdx < connArr.Length && connArr[connIdx].SourceId == srcNodeId; connIdx++) {
                yield return connArr[connIdx].TargetId;
            }
        }

        #endregion
    }
}
