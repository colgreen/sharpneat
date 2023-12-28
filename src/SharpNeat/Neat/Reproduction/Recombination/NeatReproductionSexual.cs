// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Neat.Reproduction.Sexual.Strategy;
using SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;

namespace SharpNeat.Neat.Reproduction.Sexual;

/// <summary>
/// Creation of offspring given two parents (sexual reproduction).
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public class NeatReproductionSexual<TScalar> : IRecombinationStrategy<TScalar>
    where TScalar : struct
{
    readonly IRecombinationStrategy<TScalar> _strategy;

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="metaNeatGenome">NeatGenome metadata.</param>
    /// <param name="genomeBuilder">NeatGenome builder.</param>
    /// <param name="genomeIdSeq">Genome ID sequence; for obtaining new genome IDs.</param>
    /// <param name="generationSeq">Generation sequence; for obtaining the current generation number.</param>
    /// <param name="settings">Sexual reproduction settings.</param>
    public NeatReproductionSexual(
        MetaNeatGenome<TScalar> metaNeatGenome,
        INeatGenomeBuilder<TScalar> genomeBuilder,
        Int32Sequence genomeIdSeq,
        Int32Sequence generationSeq,
        NeatReproductionSexualSettings settings)
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
