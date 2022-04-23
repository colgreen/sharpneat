// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Genome.IO;

/// <summary>
/// <see cref="NeatPopulationLoader{Double}"/> factory.
/// </summary>
public static class NeatPopulationLoaderFactory
{
    /// <summary>
    /// Create a new instance of <see cref="NeatPopulationLoader{Double}"/>.
    /// </summary>
    /// <param name="metaNeatGenome">Meta neat genome.</param>
    /// <returns>A new instance of <see cref="NeatPopulationLoader{Double}"/>.</returns>
    public static NeatPopulationLoader<double> CreateLoaderDouble(
        MetaNeatGenome<double> metaNeatGenome)
    {
        NeatGenomeLoader<double> genomeLoader = NeatGenomeLoaderFactory.CreateLoaderDouble(metaNeatGenome);
        return new NeatPopulationLoader<double>(genomeLoader);
    }
}
