// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Graphs;

/// <summary>
/// Represents a connection between two nodes. Used primarily as a key into a Dictionary that
/// uniquely identifies connections by their end points.
/// </summary>
public readonly struct DirectedConnection
    : IEquatable<DirectedConnection>,
    IComparable<DirectedConnection>
{
    /// <summary>
    /// Connection source node ID.
    /// </summary>
    public int SourceId { get; }
    /// <summary>
    /// Connection target node ID.
    /// </summary>
    public int TargetId { get; }

    #region Constructors

    /// <summary>
    /// Construct with the provided source and target node IDs.
    /// </summary>
    /// <param name="srcId">Connection source node ID.</param>
    /// <param name="tgtId">Connection target node ID.</param>
    public DirectedConnection(int srcId, int tgtId)
    {
        this.SourceId = srcId;
        this.TargetId = tgtId;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="copyFrom">The directed connection to copy.</param>
    public DirectedConnection(in DirectedConnection copyFrom)
    {
        this.SourceId = copyFrom.SourceId;
        this.TargetId = copyFrom.TargetId;
    }

    #endregion

    #region IEquatable / IComparable

    /// <inheritdoc/>
    public bool Equals(DirectedConnection other)
    {
        return (this.SourceId == other.SourceId)
            && (this.TargetId == other.TargetId);
    }

    /// <inheritdoc/>
    public int CompareTo(DirectedConnection other)
    {
        // Notes.
        // The comparison here uses subtraction rather than comparing IDs, this eliminates a number of branches
        // which gives better performance. The code works and is safe because the source and target node IDs
        // always have non-negative values, and therefore have a possible range of [0, (2^31)-1]. And if we
        // subtract the largest possible value from zero we get -(2^31)-1 which is still within the range of
        // an Int32, i.e., the result of that subtraction does not overflow and is therefore a negative value
        // as required, giving a valid comparison result.
        int diff = this.SourceId - other.SourceId;
        if(diff != 0) return diff;
        return this.TargetId - other.TargetId;
    }

    #endregion

    #region Overrides

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is DirectedConnection connection && this.Equals(connection);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(SourceId, TargetId);
    }

    /// <summary>
    /// Determines whether two <see cref="DirectedConnection"/>s have the same value.
    /// </summary>
    /// <param name="x">The first <see cref="DirectedConnection"/> to compare.</param>
    /// <param name="y">The second <see cref="DirectedConnection"/> to compare.</param>
    /// <returns>true if the two <see cref="DirectedConnection"/>s are equal; otherwise false.</returns>
    public static bool operator ==(in DirectedConnection x, in DirectedConnection y)
    {
        return (x.SourceId == y.SourceId)
            && (x.TargetId == y.TargetId);
    }

    /// <summary>
    /// Determines whether two <see cref="DirectedConnection"/>s have a different value.
    /// </summary>
    /// <param name="x">The first <see cref="DirectedConnection"/> to compare.</param>
    /// <param name="y">The second <see cref="DirectedConnection"/> to compare.</param>
    /// <returns>true if the two <see cref="DirectedConnection"/>s are different; otherwise false.</returns>
    public static bool operator !=(in DirectedConnection x, in DirectedConnection y)
    {
        return (x.SourceId != y.SourceId)
            || (x.TargetId != y.TargetId);
    }

    /// <summary>
    /// Determines whether a specified <see cref="DirectedConnection"/> is less than another specified <see cref="DirectedConnection"/>.
    /// </summary>
    /// <param name="x">The first <see cref="DirectedConnection"/> to compare.</param>
    /// <param name="y">The second <see cref="DirectedConnection"/> to compare.</param>
    /// <returns>true if <paramref name="x" /> is less than <paramref name="y" />; otherwise, false.</returns>
    public static bool operator <(in DirectedConnection x, in DirectedConnection y)
    {
        return x.CompareTo(y) < 0;
    }

    /// <summary>
    /// Determines whether a specified <see cref="DirectedConnection"/> is greater than another specified <see cref="DirectedConnection"/>.
    /// </summary>
    /// <param name="x">The first <see cref="DirectedConnection"/> to compare.</param>
    /// <param name="y">The second <see cref="DirectedConnection"/> to compare.</param>
    /// <returns>true if <paramref name="x" /> is greater than <paramref name="y" />; otherwise, false.</returns>
    public static bool operator >(in DirectedConnection x, in DirectedConnection y)
    {
        return x.CompareTo(y) > 0;
    }

    /// <summary>
    /// Determines whether a specified <see cref="DirectedConnection"/> is less than or equal to another specified
    /// <see cref="DirectedConnection"/>.
    /// </summary>
    /// <param name="x">The first <see cref="DirectedConnection"/> to compare.</param>
    /// <param name="y">The second <see cref="DirectedConnection"/> to compare.</param>
    /// <returns>true if <paramref name="x" /> is less than <paramref name="y" />; otherwise, false.</returns>
    public static bool operator <=(in DirectedConnection x, in DirectedConnection y)
    {
        return x.CompareTo(y) <= 0;
    }

    /// <summary>
    /// Determines whether a specified <see cref="DirectedConnection"/> is greater than or equal to another
    /// specified <see cref="DirectedConnection"/>.
    /// </summary>
    /// <param name="x">The first <see cref="DirectedConnection"/> to compare.</param>
    /// <param name="y">The second <see cref="DirectedConnection"/> to compare.</param>
    /// <returns>true if <paramref name="x" /> is greater than <paramref name="y" />; otherwise, false.</returns>
    public static bool operator >=(in DirectedConnection x, in DirectedConnection y)
    {
        return x.CompareTo(y) >= 0;
    }

    #endregion
}
