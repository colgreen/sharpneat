// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Graphs;

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
    /// Map a node ID from the source ID space, to the target ID space.
    /// </summary>
    /// <param name="id">A node ID in the source ID space.</param>
    /// <returns>The mapped to ID from the target ID space.</returns>
    int Map(int id);

    /// <summary>
    /// Create a new <see cref="INodeIdMap"/> that represents the inverse of the current mapping.
    /// </summary>
    /// <returns>A new <see cref="INodeIdMap"/>.</returns>
    INodeIdMap CreateInverseMap();
}
