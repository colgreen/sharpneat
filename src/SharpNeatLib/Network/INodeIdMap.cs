using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a mapping of graph node IDs from one ID space to another.
    /// </summary>
    public interface INodeIdMap
    {
        /// <summary>
        /// Gets the number of mapped node IDs.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Map a given node ID
        /// </summary>
        /// <param name="id">A node ID.</param>
        /// <returns>The mapped to ID.</returns>
        int Map(int id);
    }
}
