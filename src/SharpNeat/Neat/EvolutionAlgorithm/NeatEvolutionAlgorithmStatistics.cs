// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.EvolutionAlgorithm;

/// <summary>
/// NEAT specific evolution algorithm statistics.
/// </summary>
public class NeatEvolutionAlgorithmStatistics : EvolutionAlgorithmStatistics
{
    /// <summary>
    /// The total number of offspring genomes created since the evolution algorithm started.
    /// </summary>
    public ulong TotalOffspringCount { get; set; }

    /// <summary>
    /// The total number of offspring genomes created through asexual reproduction since the evolution algorithm started.
    /// </summary>
    public int TotalOffspringAsexualCount { get; set; }

    /// <summary>
    /// The total number of offspring genomes created through recombination reproduction since the evolution algorithm started.
    /// </summary>
    public int TotalOffspringRecombinationCount { get; set; }

    /// <summary>
    /// The total number of offspring genomes created through inter-species recombination reproduction since the evolution algorithm started.
    /// This number is included in <see cref="TotalOffspringRecombinationCount"/>.
    /// </summary>
    public int TotalOffspringInterspeciesCount { get; set; }
}
