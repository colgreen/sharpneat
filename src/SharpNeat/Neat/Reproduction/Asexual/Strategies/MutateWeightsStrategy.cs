// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategies;

/// <summary>
/// A NEAT genome asexual reproduction strategy based on mutation of connection weights.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
/// <remarks>
/// Offspring genomes are created by taking a clone of a single parent genome and applying a weight
/// mutation scheme to the connection weights of the clone.
/// </remarks>
public sealed class MutateWeightsStrategy<TScalar> : IAsexualReproductionStrategy<TScalar>
    where TScalar : struct
{
    readonly INeatGenomeBuilder<TScalar> _genomeBuilder;
    readonly Int32Sequence _genomeIdSeq;
    readonly Int32Sequence _generationSeq;
    readonly WeightMutationScheme<TScalar> _weightMutationScheme;

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="genomeBuilder">NeatGenome builder.</param>
    /// <param name="genomeIdSeq">Genome ID sequence; for obtaining new genome IDs.</param>
    /// <param name="generationSeq">Generation sequence; for obtaining the current generation number.</param>
    /// <param name="weightMutationScheme">Connection weight mutation scheme.</param>
    public MutateWeightsStrategy(
        INeatGenomeBuilder<TScalar> genomeBuilder,
        Int32Sequence genomeIdSeq,
        Int32Sequence generationSeq,
        WeightMutationScheme<TScalar> weightMutationScheme)
    {
        _genomeBuilder = genomeBuilder;
        _genomeIdSeq = genomeIdSeq;
        _generationSeq = generationSeq;
        _weightMutationScheme = weightMutationScheme;
    }

    /// <inheritdoc/>
    public NeatGenome<TScalar> CreateChildGenome(
        NeatGenome<TScalar> parent,
        IRandomSource rng)
    {
        // Clone the parent's connection weight array.
        var weightArr = (TScalar[])parent.ConnectionGenes._weightArr.Clone();

        // Apply mutation to the connection weights.
        _weightMutationScheme.MutateWeights(weightArr, rng);

        // Create the child genome's ConnectionGenes object.
        // Note. The parent genome's connection arrays are re-used; these remain unchanged because we are mutating
        // connection *weights* only, so we can avoid the cost of cloning these arrays.
        var connGenes = new ConnectionGenes<TScalar>(
            parent.ConnectionGenes._connArr,
            weightArr);

        // Create and return a new genome.
        // Note. The parent's ConnectionIndexArray and HiddenNodeIdArray can be re-used here because the new genome
        // has the same set of connections (same neural net structure).
        return _genomeBuilder.Create(
            _genomeIdSeq.Next(),
            _generationSeq.Peek,
            connGenes,
            parent.HiddenNodeIdArray,
            parent.NodeIndexByIdMap,
            parent.DirectedGraph,
            parent.ConnectionIndexMap);
    }
}
