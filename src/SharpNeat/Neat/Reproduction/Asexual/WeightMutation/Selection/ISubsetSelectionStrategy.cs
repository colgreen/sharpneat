// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

/// <summary>
/// Strategy for selecting a sub-set of items from a superset.
/// </summary>
public interface ISubsetSelectionStrategy
{
    /// <summary>
    /// Select a subset of items from a superset of a given size.
    /// </summary>
    /// <param name="supersetCount">The size of the superset to select from.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>An array of indexes that are the selected items.</returns>
    int[] SelectSubset(int supersetCount, IRandomSource rng);
}
