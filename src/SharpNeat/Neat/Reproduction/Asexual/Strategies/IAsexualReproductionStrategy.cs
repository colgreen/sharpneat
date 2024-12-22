// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Reproduction.Asexual.Strategies;

/// <summary>
/// Represents an asexual reproduction strategy for NEAT genomes.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public interface IAsexualReproductionStrategy<TScalar>
    where TScalar : unmanaged
{
    /// <summary>
    /// Create a new child genome from a given parent genome.
    /// </summary>
    /// <param name="parent">The parent genome.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>A new child genome.</returns>
    NeatGenome<TScalar>? CreateChildGenome(
        NeatGenome<TScalar> parent,
        IRandomSource rng);
}
