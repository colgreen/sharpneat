using Redzen.Structures;
using SharpNeat.Network;

namespace SharpNeat.Neat
{
    // TODO: Consider moving the most recently used structure to the head of the buffer to increase its lifespan.
    /// <summary>
    /// Stores a history of previously added connections, keyed by their source and target node ID.
    ///
    /// Used when adding new connections to check if an identical connection has been added to a genome elsewhere 
    /// in the population. This allows re-use of the same innovation ID for like connections.
    /// </summary>
    public class AddedConnectionBuffer
    {
        #region Instance Fields

        KeyedCircularBuffer<DirectedConnection,int> _buffer;
        int _inputNodeCount;
        int _outputNodeCount;
        int _ioNodeCount;
        
        #endregion

        #region Constructor

        public AddedConnectionBuffer(int capacity, int inputNodeCount, int outputNodeCount)
        {
            _buffer = new KeyedCircularBuffer<DirectedConnection,int>(capacity);
            _inputNodeCount = inputNodeCount;
            _outputNodeCount = outputNodeCount;
            _ioNodeCount = inputNodeCount + _outputNodeCount;
        }

        #endregion

        #region Public Methods

        public void Register(DirectedConnection key, int connectionId)
        {
            if(!IOConnectionUtils.IsInputOutputConnection(key, _inputNodeCount, _outputNodeCount)) {   
                _buffer.Enqueue(key, connectionId);
            }
        }

        public bool TryLookup(DirectedConnection key, out int connectionId)
        {
            // Handle special case.
            // Connections directly from an input node to an output node are assigned a predetermined innovation ID.
            if(IOConnectionUtils.TryGetInputOutputConnectionId(key, _inputNodeCount, _outputNodeCount, out connectionId)) {
                return true;
            }

            // Not an input-to-output connection, so lookup in the history buffer.
            return _buffer.TryGetValue(key, out connectionId);
        }

        #endregion
    }
}
