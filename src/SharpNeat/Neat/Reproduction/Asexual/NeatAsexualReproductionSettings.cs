﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
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

    /// <summary>
    /// Creates a new settings object based on the current settings object, but modified to be suitable for use when
    /// the evolution algorithm is in simplifying mode.
    /// </summary>
    /// <returns>A new instance of <see cref="NeatAsexualReproductionSettings"/>.</returns>
    public NeatAsexualReproductionSettings CreateSimplifyingSettings()
    {
        var simplifying = this with
        {
            ConnectionWeightMutationProbability = 0.6,
            AddNodeMutationProbability = 0.0,
            AddConnectionMutationProbability = 0.0,
            DeleteConnectionMutationProbability = 0.4
        };

        return simplifying;
    }

    /// <summary>
    /// Validate the settings, and throw an exception if not valid.
    /// </summary>
    /// <remarks>
    /// As a 'simple' collection of properties there is no construction time check that can be performed, therefore this method is supplied to
    /// allow consumers of a settings object to validate it before using it.
    /// </remarks>
    public void Validate()
    {
        if(!IsProbability(ConnectionWeightMutationProbability)) throw new InvalidOperationException("ConnectionWeightMutationProbability must be in the interval [0,1].");
        if(!IsProbability(AddNodeMutationProbability)) throw new InvalidOperationException("AddNodeMutationProbability must be in the interval [0,1].");
        if(!IsProbability(AddConnectionMutationProbability)) throw new InvalidOperationException("AddConnectionMutationProbability must be in the interval [0,1].");
        if(!IsProbability(DeleteConnectionMutationProbability)) throw new InvalidOperationException("DeleteConnectionMutationProbability must be in the interval [0,1].");
        if(Math.Abs((ConnectionWeightMutationProbability + AddNodeMutationProbability + AddConnectionMutationProbability + DeleteConnectionMutationProbability) - 1.0) > 1e-6)
            throw new InvalidOperationException("Mutation probabilities must sum to 1.0");

        static bool IsProbability(double p) => p >= 0 && p <= 1.0;
    }
}
