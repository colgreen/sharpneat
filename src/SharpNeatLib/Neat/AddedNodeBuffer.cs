using Redzen.Structures;

namespace SharpNeat.Neat
{
    /// <summary>
    /// Stores a history of previously added nodes, keyed by the ID of the connection that was split to create he node.
    ///
    /// Used when adding new nodes to check if an identical node (connection split) has been added to a genome elsewhere 
    /// in the population. This allows re-use of the same innovation ID for like nodes.
    /// </summary>
    public class AddedNodeBuffer
    {
        KeyedCircularBuffer<uint,AddedNodeInfo> _buffer;
        
        #region Constructor

        public AddedNodeBuffer(int capacity)
        {
            _buffer = new KeyedCircularBuffer<uint, AddedNodeInfo>(capacity);
        }

        #endregion

        #region Public Methods

        public void Register(uint connectionId, AddedNodeInfo addedNodeInfo)
        {
            _buffer.Enqueue(connectionId, addedNodeInfo);
        }

        /// <summary>
        /// Get the AddedNodeInfo from a previous 'add node' mutation based on splitting a connection with the given ID.
        /// </summary>
        /// <remarks>
        /// <param name="connectionId">The connection ID to look-up.</param>
        /// <returns>True if a node was found, otherwise false</returns>
        public bool TryGetNodeId(uint connectionId, out AddedNodeInfo addedNodeInfo)
        {
            return _buffer.TryGetValue(connectionId, out addedNodeInfo);
        }

        #endregion
    }
}
