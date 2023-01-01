// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Drawing;
using SharpNeat.Graphs;

namespace SharpNeat.Drawing.Graph;

/// <summary>
/// Represents a scheme for assigning a 2D position to each node in a graph.
/// </summary>
public interface IGraphLayoutScheme
{
    /// <summary>
    /// Layout the nodes of the provided directed in a 2D area specified by <paramref name="layoutArea"/>.
    /// </summary>
    /// <param name="digraph">The directed graph to be laid out.</param>
    /// <param name="layoutArea">The area to layout nodes within.</param>
    /// <param name="nodePosByIdx">A span that will be populated with a 2D position for each node, within the provided layout area.</param>
    void Layout(
        DirectedGraph digraph,
        Size layoutArea,
        Span<Point> nodePosByIdx);

    /// <summary>
    /// Layout the nodes of the provided directed in a 2D area specified by <paramref name="layoutArea"/>.
    /// </summary>
    /// <param name="digraph">The directed graph to be laid out.</param>
    /// <param name="layoutArea">The area to layout nodes within.</param>
    /// <param name="nodePosByIdx">A span that will be populated with a 2D position for each node, within the provided layout area.</param>
    /// <param name="layoutSchemeData">An optional object that conveys layout data specific to the layout scheme.</param>
    void Layout(
        DirectedGraph digraph,
        Size layoutArea,
        Span<Point> nodePosByIdx,
        ref object? layoutSchemeData);
}
