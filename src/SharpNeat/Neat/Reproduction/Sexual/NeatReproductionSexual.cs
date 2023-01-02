// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Neat.Reproduction.Sexual.Strategy;
using SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;

namespace SharpNeat.Neat.Reproduction.Sexual;

/// <summary>
/// Creation of offspring given two parents (sexual reproduction).
/// </summary>
/// <typeparam name="T">Neural net numeric data type.</typeparam>
public class NeatReproductionSexual<T> : ISexualReproductionStrategy<T>
    where T : struct
{
    readonly ISexualReproductionStrategy<T> _strategy;

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="metaNeatGenome">NeatGenome metadata.</param>
    /// <param name="genomeBuilder">NeatGenome builder.</param>
    /// <param name="genomeIdSeq">Genome ID sequence; for obtaining new genome IDs.</param>
    /// <param name="generationSeq">Generation sequence; for obtaining the current generation number.</param>
    /// <param name="settings">Sexual reproduction settings.</param>
    public NeatReproductionSexual(
        MetaNeatGenome<T> metaNeatGenome,
        INeatGenomeBuilder<T> genomeBuilder,
        Int32Sequence genomeIdSeq,
        Int32Sequence generationSeq,
        NeatReproductionSexualSettings settings)
    {
        _strategy = new UniformCrossoverReproductionStrategy<T>(
                            metaNeatGenome.IsAcyclic,
                            settings.SecondaryParentGeneProbability,
                            genomeBuilder,
                            genomeIdSeq, generationSeq);
    }

    /// <inheritdoc/>
    public NeatGenome<T> CreateGenome(
        NeatGenome<T> parent1,
        NeatGenome<T> parent2,
        IRandomSource rng)
    {
        // Invoke the reproduction strategy.
        return _strategy.CreateGenome(parent1, parent2, rng);
    }
}
