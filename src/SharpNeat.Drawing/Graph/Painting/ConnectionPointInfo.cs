// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Drawing.Graph.Painting;

/// <summary>
/// For tracking connection points on nodes when drawing backwards directed connections, i.e. when the
/// connection target node is vertically higher than the source node.
/// </summary>
public struct ConnectionPointInfo
{
    /// <summary>
    /// Running connection count for top left of node.
    /// </summary>
    public int UpperLeft;

    /// <summary>
    /// Running connection count for top right of node.
    /// </summary>
    public int UpperRight;

    /// <summary>
    /// Running connection count for bottom left of node.
    /// </summary>
    public int LowerLeft;

    /// <summary>
    /// Running connection count for bottom right of node.
    /// </summary>
    public int LowerRight;
}
