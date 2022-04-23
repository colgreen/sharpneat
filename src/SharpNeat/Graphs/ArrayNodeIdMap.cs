// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Graphs;

/// <summary>
/// An <see cref="INodeIdMap"/> that is backed by an array of node ID mappings. The mapping is from array index
/// to the array element integer at that index, therefore the source node IDs of the mappings must be a contiguous
/// sequence.
/// </summary>
public sealed class ArrayNodeIdMap : INodeIdMap
{
    readonly int[] _mapArr;

    /// <summary>
    /// Construct with the given array of mappings.
    /// </summary>
    /// <param name="mapArr">An array representing node ID mappings.</param>
    public ArrayNodeIdMap(int[] mapArr)
    {
        _mapArr = mapArr ?? throw new ArgumentNullException(nameof(mapArr));
    }

    /// <summary>
    /// Gets the number of mapped node IDs.
    /// </summary>
    public int Count => _mapArr.Length;

    /// <summary>
    /// Map a node ID from the source ID space, to the target ID space.
    /// </summary>
    /// <param name="id">A node ID in the source ID space.</param>
    /// <returns>The mapped to ID from the target ID space.</returns>
    public int Map(int id)
    {
        return _mapArr[id];
    }

    /// <summary>
    /// Create a new <see cref="INodeIdMap"/> that represents the inverse of the current mapping.
    /// </summary>
    /// <returns>A new <see cref="INodeIdMap"/>.</returns>
    public INodeIdMap CreateInverseMap()
    {
        // This method is not needed at time of writing.
        throw new NotImplementedException();
    }
}
