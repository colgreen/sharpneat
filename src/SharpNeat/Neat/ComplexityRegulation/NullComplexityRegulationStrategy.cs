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
    /// <inheritdoc/>
    public ComplexityRegulationMode CurrentMode => ComplexityRegulationMode.Complexifying;

    /// <inheritdoc/>
    public ComplexityRegulationMode UpdateMode(
        EvolutionAlgorithmStatistics eaStats,
        PopulationStatistics popStats)
    {
        // This is the null strategy, therefore do nothing.
        return ComplexityRegulationMode.Complexifying;
    }
}
