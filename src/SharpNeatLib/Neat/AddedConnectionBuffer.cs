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
            if(!IsInputOutputConnection(key)) {   
                _buffer.Enqueue(key, connectionId);
            }
        }

        public bool TryLookup(DirectedConnection key, out int connectionId)
        {
            // Handle special case.
            // Connections directly from an input node to an output node are assigned a predetermined innovation ID.
            if(TryGetInputOutputConnectionId(key, out connectionId)) {
                return true;
            }

            // Not an input-to-output connection, so lookup in the history buffer.
            return _buffer.TryGetValue(key, out connectionId);
        }

        #endregion

        #region Private Methods

        private bool TryGetInputOutputConnectionId(DirectedConnection key, out int connectionId)
        {
            // Test for a source node that is one of the input nodes, and a target node that is one of the output nodes.
            if(IsInputOutputConnection(key))
            {
                // Adjust for the fact that the output node IDs start where the input node IDs finish.
                int outputIdx = key.TargetId - _inputNodeCount;
                connectionId = (key.SourceId * _outputNodeCount) + outputIdx + _ioNodeCount;
                return true;
            }

            connectionId = default(int);
            return false;
        }

        private bool IsInputOutputConnection(DirectedConnection key)
        {
            return (key.SourceId < _inputNodeCount) && (key.TargetId >= _inputNodeCount && key.TargetId < _inputNodeCount + _outputNodeCount);
        }

        #endregion
    }
}
