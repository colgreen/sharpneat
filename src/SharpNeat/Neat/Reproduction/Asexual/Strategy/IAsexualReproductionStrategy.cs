// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy;

/// <summary>
/// Represents a NEAT genome asexual reproduction strategy.
/// </summary>
/// <typeparam name="T">Connection weight data type.</typeparam>
public interface IAsexualReproductionStrategy<T> where T : struct
{
    /// <summary>
    /// Create a new child genome from a given parent genome.
    /// </summary>
    /// <param name="parent">The parent genome.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>A new child genome.</returns>
    NeatGenome<T>? CreateChildGenome(NeatGenome<T> parent, IRandomSource rng);
}
