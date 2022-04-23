// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Neat.ComplexityRegulation;

/// <summary>
/// A complexity regulation strategy that remains fixed in complexifying mode throughout
/// the lifetime of the evolution algorithm.
/// </summary>
public sealed class NullComplexityRegulationStrategy : IComplexityRegulationStrategy
{
    #region IComplexityRegulationStrategy

    /// <summary>
    /// Gets the current complexity regulation mode.
    /// </summary>
    public ComplexityRegulationMode CurrentMode => ComplexityRegulationMode.Complexifying;

    /// <summary>
    /// Determine the complexity regulation mode that the evolution algorithm search should be in given the
    /// provided evolution algorithm statistics object, and set the current mode to that mode.
    /// </summary>
    /// <param name="eaStats">Evolution algorithm statistics.</param>
    /// <param name="popStats">Population statistics.</param>
    /// <returns>The determined <see cref="ComplexityRegulationMode"/>.</returns>
    public ComplexityRegulationMode UpdateMode(
        EvolutionAlgorithmStatistics eaStats,
        PopulationStatistics popStats)
    {
        // This is the null strategy, therefore do nothing.
        return ComplexityRegulationMode.Complexifying;
    }

    #endregion
}
