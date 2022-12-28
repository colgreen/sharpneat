// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Graphs;

#pragma warning disable CA1036 // Override methods on comparable types

/// <summary>
/// Represents a connection between two nodes, combined with a connection weight.
/// </summary>
/// <remarks>
/// This type does not inherit from DirectedConnection as might be expected because these are structs (value types)
/// and therefore inheritance is not possible.
/// </remarks>
/// <typeparam name="T">Connection weight data type.</typeparam>
public readonly struct WeightedDirectedConnection<T> : IComparable<WeightedDirectedConnection<T>>
    where T : struct
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
    public T Weight { get; }

    /// <summary>
    /// Construct with the provided source and target node IDs, and weight.
    /// </summary>
    /// <param name="srcId">Connection source node ID.</param>
    /// <param name="tgtId">Connection target node ID.</param>
    /// <param name="weight">Connection weight.</param>
    public WeightedDirectedConnection(int srcId, int tgtId, T weight)
    {
        SourceId = srcId;
        TargetId = tgtId;
        Weight = weight;
    }

    #region IComparable<T>

    /// <inheritdoc/>
    public int CompareTo(WeightedDirectedConnection<T> other)
    {
        // Notes.
        // The comparison here uses subtraction rather than comparing IDs, this eliminates a number of branches
        // which gives better performance. The code works and is safe because the source and target node IDs
        // always have non-negative values, and therefore have a possible range of [0, (2^31)-1]. And if we
        // subtract the largest possible value from zero we get -(2^31)-1 which is still within the range of
        // an Int32, i.e., the result of that subtraction does not overflow and is therefore a negative value
        // as required, giving a valid comparison result.
        int diff = SourceId - other.SourceId;
        if(diff != 0)
            return diff;

        return TargetId - other.TargetId;
    }

    #endregion
}
