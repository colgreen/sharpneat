// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Graphs.Acyclic;

/// <summary>
/// Represents a node and connection index that represent the last node and connection in a given layer
/// in an acyclic graph.
///
/// The nodes and connections on an acyclic graph are ordered by the layer they are in. For more details
/// on how the layers are determined/defined see <see cref="DirectedGraphAcyclic"/>.
///
/// Connections are defined as being in the same layer as their source node.
/// </summary>
public readonly struct LayerInfo
{
    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="endNodeIdx">Specifies the last node in the current layer. Specifically, the index of that
    /// node plus one.</param>
    /// <param name="endConnectionIdx">Specifies the last connection in the current layer. Specifically, the index of that
    /// connection plus one.</param>
    public LayerInfo(int endNodeIdx, int endConnectionIdx)
    {
        EndNodeIdx = endNodeIdx;
        EndConnectionIdx = endConnectionIdx;
    }

    /// <summary>
    /// Specifies the last node in the current layer. Specifically, the index of that
    /// node plus one.
    /// </summary>
    public int EndNodeIdx { get; }

    /// <summary>
    /// Specifies the last connection in the current layer. Specifically, the index of that
    /// connection plus one.
    /// </summary>
    public int EndConnectionIdx { get; }
}
