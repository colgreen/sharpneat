// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Reproduction.Asexual;

/// <summary>
/// Settings related to <see cref="NeatAsexualReproduction{T}"/>.
/// </summary>
public sealed record NeatAsexualReproductionSettings
{
    /// <summary>
    /// Probability that a genome mutation is a connection weights mutation.
    /// </summary>
    public double ConnectionWeightMutationProbability { get; set; } = 0.94;

    /// <summary>
    /// Probability that a genome mutation is an 'add node' mutation.
    /// </summary>
    public double AddNodeMutationProbability { get; set; } = 0.01;

    /// <summary>
    /// Probability that a genome mutation is an 'add connection' mutation.
    /// </summary>
    public double AddConnectionMutationProbability { get; set; } = 0.025;

    /// <summary>
    /// Probability that a genome mutation is a 'delete connection' mutation.
    /// </summary>
    public double DeleteConnectionMutationProbability { get; set; } = 0.025;
}
