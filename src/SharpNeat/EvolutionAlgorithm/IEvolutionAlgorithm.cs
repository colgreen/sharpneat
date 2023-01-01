// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.EvolutionAlgorithm;

/// <summary>
/// Represents a generational evolution algorithm.
/// </summary>
public interface IEvolutionAlgorithm
{
    /// <summary>
    /// Gets the current evolution algorithm statistics.
    /// </summary>
    EvolutionAlgorithmStatistics Stats { get; }

    /// <summary>
    /// Initialise the evolutionary algorithm.
    /// </summary>
    void Initialise();

    /// <summary>
    /// Perform one generation of the evolutionary algorithm.
    /// </summary>
    void PerformOneGeneration();
}
