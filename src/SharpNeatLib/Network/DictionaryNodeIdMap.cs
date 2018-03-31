using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNeatLib.Network
{
    /// <summary>
    /// An INodeIdMap implemented with a dictionary keyed by node ID.
    /// </summary>
    internal class DictionaryNodeIdMap : INodeIdMap
    {
        readonly int _inputOutputCount;
        readonly Dictionary<int,int> _hiddenNodeIdxById;

        #region Constructor

        public DictionaryNodeIdMap(
            int inputOutputCount, 
            Dictionary<int,int> hiddenNodeIdxById)
        {
            _inputOutputCount = inputOutputCount;
            _hiddenNodeIdxById = hiddenNodeIdxById;
        }

        #endregion

        #region INodeIdMap

        public int Map(int id)
        {
            // Input/output node IDs are fixed.
            if (id < _inputOutputCount)
            {
                return id;
            }
            // Hidden nodes have mappings stored in a dictionary.
            return _hiddenNodeIdxById[id];
        }

        #endregion
    }
}
