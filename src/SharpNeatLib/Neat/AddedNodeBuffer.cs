using Redzen.Structures;
using SharpNeat.Network;

namespace SharpNeat.Neat
{
    // TODO: Consider moving the most recently used structure to the head of the buffer to increase its lifespan.
    /// <summary>
    /// Stores a history of previously added nodes, keyed by the ID of the connection that was split to create the node.
    ///
    /// Used when adding new nodes to check if an identical node (connection split) has been added to a genome elsewhere 
    /// in the population. This allows re-use of the same innovation ID for like nodes.
    /// </summary>
    public class AddedNodeBuffer
    {
        KeyedCircularBuffer<DirectedConnection,int> _buffer;
        
        #region Constructor

        public AddedNodeBuffer(int capacity)
        {
            _buffer = new KeyedCircularBuffer<DirectedConnection,int>(capacity);
        }

        #endregion

        #region Public Methods

        public void Register(DirectedConnection conn, int addedNodeId)
        {
            _buffer.Enqueue(conn, addedNodeId);
        }

        /// <summary>
        /// Get the AddedNodeInfo from a previous 'add node' mutation based on splitting a connection with the given ID.
        /// </summary>
        /// <remarks>
        /// <param name="connectionId">The connection ID to look-up.</param>
        /// <returns>True if a node was found, otherwise false</returns>
        public bool TryLookup(DirectedConnection conn, out int addedNodeId)
        {
            return _buffer.TryGetValue(conn, out addedNodeId);
        }

        #endregion
    }
}
