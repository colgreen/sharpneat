// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Numerics.Distributions;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

/// <summary>
/// Strategy for selecting a sub-set of items from a superset.
/// The number of items to select is a fixed number (the selection cardinality), unless the superset is smaller
/// in which case all items in the superset are selected.
/// </summary>
public sealed class CardinalSubsetSelectionStrategy : ISubsetSelectionStrategy
{
    readonly int _selectCount;

    /// <summary>
    /// Construct with the given selection count (selection cardinality).
    /// </summary>
    /// <param name="selectCount">The number of items to select.</param>
    public CardinalSubsetSelectionStrategy(int selectCount)
    {
        _selectCount = selectCount;
    }

    /// <inheritdoc/>
    public int[] SelectSubset(int supersetCount, IRandomSource rng)
    {
        // Note. Ideally we'd return a sorted list of indexes to improve performance of the code that consumes them,
        // however, the sampling process inherently produces samples in randomized order, thus the decision of whether
        // to sort or not depends on the cost to the code using the samples. I.e. don't sort here!
        int selectionCount = Math.Min(_selectCount, supersetCount);
        int[] idxArr = new int[selectionCount];
        DiscreteDistributionUtils.SampleUniformWithoutReplacement(supersetCount, idxArr, rng);
        return idxArr;
    }
}
