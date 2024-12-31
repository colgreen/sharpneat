// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Neat.Reproduction.Recombination.Strategies;
using SharpNeat.Neat.Reproduction.Recombination.Strategies.UniformCrossover;

namespace SharpNeat.Neat.Reproduction.Recombination;

/// <inheritdoc/>
public class NeatRecombination<TScalar> : IRecombinationStrategy<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    readonly UniformCrossoverRecombinationStrategy<TScalar> _strategy;

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="metaNeatGenome">NeatGenome metadata.</param>
    /// <param name="genomeBuilder">NeatGenome builder.</param>
    /// <param name="genomeIdSeq">Genome ID sequence; for obtaining new genome IDs.</param>
    /// <param name="generationSeq">Generation sequence; for obtaining the current generation number.</param>
    /// <param name="settings">Recombination reproduction settings.</param>
    public NeatRecombination(
        MetaNeatGenome<TScalar> metaNeatGenome,
        INeatGenomeBuilder<TScalar> genomeBuilder,
        Int32Sequence genomeIdSeq,
        Int32Sequence generationSeq,
        NeatRecombinationSettings settings)
    {
        _strategy = new UniformCrossoverRecombinationStrategy<TScalar>(
                            metaNeatGenome.IsAcyclic,
                            settings.SecondaryParentGeneProbability,
                            genomeBuilder,
                            genomeIdSeq, generationSeq);
    }

    /// <inheritdoc/>
    public NeatGenome<TScalar> CreateGenome(
        NeatGenome<TScalar> parent1,
        NeatGenome<TScalar> parent2,
        IRandomSource rng)
    {
        // Invoke the reproduction strategy.
        return _strategy.CreateGenome(parent1, parent2, rng);
    }
}
