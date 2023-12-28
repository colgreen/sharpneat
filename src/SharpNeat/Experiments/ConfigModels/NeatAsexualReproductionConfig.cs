// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Experiments.ConfigModels;

/// <summary>
/// Model type for NEAT asexual reproduction configuration.
/// </summary>
public sealed record NeatAsexualReproductionConfig
{
    /// <summary>
    /// Probability that a genome mutation is a connection weights mutation.
    /// </summary>
    public double? ConnectionWeightMutationProbability { get; init; }

    /// <summary>
    /// Probability that a genome mutation is an 'add node' mutation.
    /// </summary>
    public double? AddNodeMutationProbability { get; init; }

    /// <summary>
    /// Probability that a genome mutation is an 'add connection' mutation.
    /// </summary>
    public double? AddConnectionMutationProbability { get; init; }

    /// <summary>
    /// Probability that a genome mutation is a 'delete connection' mutation.
    /// </summary>
    public double? DeleteConnectionMutationProbability { get; init; }
}
