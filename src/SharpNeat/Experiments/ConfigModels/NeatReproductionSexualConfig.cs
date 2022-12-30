// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Experiments.ConfigModels;

/// <summary>
/// Model type for NEAT sexual reproduction configuration.
/// </summary>
public sealed record NeatReproductionSexualConfig
{
    /// <summary>
    /// The probability that a gene that exists only on the secondary parent is copied into the child genome.
    /// </summary>
    public double? SecondaryParentGeneProbability { get; init; }
}
