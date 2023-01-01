// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Drawing;
using SharpNeat.Graphs;

namespace SharpNeat.Drawing;

/// <summary>
/// Represents a directed graph, with supplementary data suitable for producing a 2D visual representation of the graph.
/// </summary>
public sealed class DirectedGraphViewModel
{
    /// <summary>
    /// Represents the directed graph topology.
    /// </summary>
    public DirectedGraph DirectedGraph { get; }

    /// <summary>
    /// Graph connection/vertex weights.
    /// </summary>
    public float[] WeightArr { get; }

    /// <summary>
    /// Provides a ID/label for each node.
    /// </summary>
    public INodeIdMap NodeIdByIdx { get; }

    /// <summary>
    /// Provides a 2D position for each node.
    /// </summary>
    public Point[] NodePosByIdx { get; }

    #region Construction

    /// <summary>
    /// Construct with the provided digraph, weights, and node IDs.
    /// The node positions array is allocated, but must be updated with actual positions outside of this constructor.
    /// </summary>
    /// <param name="digraph">Directed graph.</param>
    /// <param name="weightArr">Graph connection/vertex weights.</param>
    /// <param name="nodeIdByIdx">Provides a ID/label for each node.</param>
    public DirectedGraphViewModel(
        DirectedGraph digraph,
        float[] weightArr,
        INodeIdMap nodeIdByIdx)
    {
        if(weightArr.Length != digraph.ConnectionIds.Length)
            throw new ArgumentException("Weight and connection ID arrays must have same length.", nameof(weightArr));

        if(nodeIdByIdx.Count != digraph.TotalNodeCount)
            throw new ArgumentException("Node counts must match.", nameof(nodeIdByIdx));

        DirectedGraph = digraph;
        WeightArr = weightArr;
        NodeIdByIdx = nodeIdByIdx;
        NodePosByIdx = new Point[digraph.TotalNodeCount];
    }

    /// <summary>
    /// Construct with the provided digraph, weights, node IDs, and node positions.
    /// </summary>
    /// <param name="digraph">Directed graph.</param>
    /// <param name="weightArr">Graph connection/vertex weights.</param>
    /// <param name="nodeIdByIdx">Provides a ID/label for each node.</param>
    /// <param name="nodePosByIdx">Provides a 2D position for each node.</param>
    public DirectedGraphViewModel(
        DirectedGraph digraph,
        float[] weightArr,
        INodeIdMap nodeIdByIdx,
        Point[] nodePosByIdx)
    {
        if(weightArr.Length != digraph.ConnectionIds.Length)
            throw new ArgumentException("Weight and connection ID arrays must have same length.", nameof(weightArr));

        if(nodeIdByIdx.Count != digraph.TotalNodeCount)
            throw new ArgumentException("Node counts must match.", nameof(nodeIdByIdx));

        if(nodePosByIdx.Length != digraph.TotalNodeCount)
            throw new ArgumentException("Node counts must match.", nameof(nodePosByIdx));

        DirectedGraph = digraph;
        WeightArr = weightArr;
        NodeIdByIdx = nodeIdByIdx;
        NodePosByIdx = nodePosByIdx;
    }

    #endregion
}
