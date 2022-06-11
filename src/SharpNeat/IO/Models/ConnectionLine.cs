// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.IO.Models;

/// <summary>
/// Represents a connection line in a 'net' file.
/// </summary>
public class ConnectionLine
{
    /// <summary>
    /// Connection source node ID.
    /// </summary>
    public int SourceId { get; }

    /// <summary>
    /// Connection target node ID.
    /// </summary>
    public int TargetId { get; }

    /// <summary>
    /// Connection weight.
    /// </summary>
    public double Weight { get; }

    /// <summary>
    /// Construct with the provided source and target node IDs, and weight.
    /// </summary>
    /// <param name="srcId">Connection source node ID.</param>
    /// <param name="tgtId">Connection target node ID.</param>
    /// <param name="weight">Connection weight.</param>
    public ConnectionLine(int srcId, int tgtId, double weight)
    {
        this.SourceId = srcId;
        this.TargetId = tgtId;
        this.Weight = weight;
    }
}
