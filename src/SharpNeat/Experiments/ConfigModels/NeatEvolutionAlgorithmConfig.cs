// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Experiments.ConfigModels;

/// <summary>
/// Model type for NEAT evolution algorithm configuration.
/// </summary>
public sealed record NeatEvolutionAlgorithmConfig
{
    /// <summary>
    /// The species count.
    /// </summary>
    public int? SpeciesCount { get; init; }

    /// <summary>
    /// Elitism proportion.
    /// We sort species genomes by fitness and keep the top N%, the other genomes are
    /// removed to make way for the offspring.
    /// </summary>
    public double? ElitismProportion { get; init; }

    /// <summary>
    /// Selection proportion.
    /// We sort species genomes by fitness and select parent genomes for producing offspring from
    /// the top N%. Selection is performed prior to elitism being applied, therefore selecting from more
    /// genomes than will be made elite is possible.
    /// </summary>
    public double? SelectionProportion { get; init; }

    /// <summary>
    /// The proportion of offspring to be produced via asexual reproduction (mutation).
    /// </summary>
    public double? OffspringAsexualProportion { get; init; }

    /// <summary>
    /// The proportion of offspring to be produced via recombination reproduction.
    /// </summary>
    public double? OffspringRecombinationProportion { get; init; }

    /// <summary>
    /// The proportion of recombination reproductions that will use genomes from different species.
    /// </summary>
    public double? InterspeciesMatingProportion { get; init; }

    /// <summary>
    /// Length of the history buffer used for calculating the moving average for best fitness, mean fitness and mean complexity.
    /// </summary>
    public int? StatisticsMovingAverageHistoryLength { get; init; }
}
