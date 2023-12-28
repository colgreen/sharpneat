// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Reproduction.Recombination.Strategies.UniformCrossover;

/// <summary>
/// Represents a single connection gene.
/// </summary>
/// <typeparam name="TWeight">Connection weight data type.</typeparam>
internal readonly struct ConnectionGene<TWeight>
    where TWeight : struct
{
    /// <summary>
    /// The source and target node IDs.
    /// </summary>
    public readonly DirectedConnection Endpoints;

    /// <summary>
    /// Connection weight.
    /// </summary>
    public readonly TWeight Weight;

    /// <summary>
    /// Construct with the given connection node endpoints, and connection weight.
    /// </summary>
    /// <param name="endpoints">The connection endpoints, i.e., the connection's source and target node IDs.</param>
    /// <param name="weight">The connection weight.</param>
    public ConnectionGene(in DirectedConnection endpoints, TWeight weight)
    {
        Endpoints = endpoints;
        Weight = weight;
    }
}
