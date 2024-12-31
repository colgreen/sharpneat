// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using Redzen.Numerics.Distributions;
using static SharpNeat.Neat.Reproduction.Recombination.Strategies.UniformCrossover.UniformCrossoverRecombinationStrategyUtils;

namespace SharpNeat.Neat.Reproduction.Recombination.Strategies.UniformCrossover;

/// <summary>
/// A recombination strategy that utilises genetic crossover between two parent genomes.
/// Uniform crossover.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
/// <remarks>
/// The genes of the two parent genomes are aligned by innovation ID. The new child genome
/// takes genes from each of the parents with a probability (e.g. 50%).
/// </remarks>
public sealed class UniformCrossoverRecombinationStrategy<TScalar> : IRecombinationStrategy<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    readonly bool _isAcyclic;
    readonly double _secondaryParentGeneProbability;
    readonly INeatGenomeBuilder<TScalar> _genomeBuilder;
    readonly Int32Sequence _genomeIdSeq;
    readonly Int32Sequence _generationSeq;
    readonly ConnectionGeneListBuilder<TScalar> _connGeneListBuilder;

    #region Constructor

    /// <summary>
    /// Construct with the given strategy arguments.
    /// </summary>
    /// <param name="isAcyclic">Indicates that the strategy will be operating on acyclic graphs/genomes.</param>
    /// <param name="secondaryParentGeneProbability">The probability that a gene that exists only on the secondary parent is copied into the child genome.</param>
    /// <param name="genomeBuilder">A neat genome builder.</param>
    /// <param name="genomeIdSeq">Genome ID sequence; for obtaining new genome IDs.</param>
    /// <param name="generationSeq">A sequence that provides the current generation number.</param>
    public UniformCrossoverRecombinationStrategy(
        bool isAcyclic,
        double secondaryParentGeneProbability,
        INeatGenomeBuilder<TScalar> genomeBuilder,
        Int32Sequence genomeIdSeq,
        Int32Sequence generationSeq)
    {
        _isAcyclic = isAcyclic;
        _secondaryParentGeneProbability = secondaryParentGeneProbability;
        _genomeBuilder = genomeBuilder;
        _genomeIdSeq = genomeIdSeq;
        _generationSeq = generationSeq;

        _connGeneListBuilder = new ConnectionGeneListBuilder<TScalar>(_isAcyclic, 1024);
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public NeatGenome<TScalar> CreateGenome(
        NeatGenome<TScalar> parent1,
        NeatGenome<TScalar> parent2,
        IRandomSource rng)
    {
        try
        {
            return CreateGenomeInner(parent1, parent2, rng);
        }
        finally
        {
            // Clear down ready for re-use of the builder on the next call to CreateGenome().
            // Re-using in this way avoids having to de-alloc and re-alloc memory, thus reducing garbage collection overhead.
            _connGeneListBuilder.Clear();
        }
    }

    #endregion

    #region Private Methods

    private NeatGenome<TScalar> CreateGenomeInner(
        NeatGenome<TScalar> parent1,
        NeatGenome<TScalar> parent2,
        IRandomSource rng)
    {
        // Resolve a flag that determines if *all* disjoint genes from the secondary parent will be included in the child genome, or not.
        // This approach is from SharpNEAT v2.x and is preserved to act as baseline in v4.x, but better strategies may exist.
        bool includeSecondaryParentGene = DiscreteDistributionUtils.SampleBernoulli(_secondaryParentGeneProbability, rng);

        // Enumerate over the connection genes in both parents.
        foreach((int idx1, int idx2) in EnumerateParentGenes(parent1.ConnectionGenes, parent2.ConnectionGenes))
        {
            // Create a connection gene based on the current position in both parents.
            ConnectionGene<TScalar>? connGene = CreateConnectionGene(
                parent1.ConnectionGenes, parent2.ConnectionGenes,
                idx1, idx2,
                includeSecondaryParentGene, rng);

            if(connGene.HasValue)
            {   // Attempt to add the gene to the child genome we are building.
                _connGeneListBuilder.TryAddGene(connGene.Value);
            }
        }

        // Convert the genes to the structure required by NeatGenome.
        ConnectionGenes<TScalar> connGenes = _connGeneListBuilder.ToConnectionGenes();

        // Create and return a new genome.
        return _genomeBuilder.Create(
            _genomeIdSeq.Next(),
            _generationSeq.Peek,
            connGenes);
    }

    #endregion

    #region Private Methods [Low Level]

    private static ConnectionGene<TScalar>? CreateConnectionGene(
        ConnectionGenes<TScalar> connGenes1,
        ConnectionGenes<TScalar> connGenes2,
        int idx1, int idx2,
        bool includeSecondaryParentGene,
        IRandomSource rng)
    {
        // Select gene at random if it is present on both parents.
        if(idx1 != -1 && idx2 != -1)
        {
            if(rng.NextBool())
                return CreateConnectionGene(connGenes1, idx1);
            else
                return CreateConnectionGene(connGenes2, idx2);
        }

        // Use the primary parent's gene if it has one.
        if(idx1 != -1)
            return CreateConnectionGene(connGenes1, idx1);

        // Otherwise use the secondary parent's gene if the 'includeSecondaryParentGene' flag is set.
        if(includeSecondaryParentGene)
            return CreateConnectionGene(connGenes2, idx2);

        return null;
    }

    private static ConnectionGene<TScalar> CreateConnectionGene(ConnectionGenes<TScalar> connGenes, int idx)
    {
        return new ConnectionGene<TScalar>(
            connGenes._connArr[idx],
            connGenes._weightArr[idx]);
    }

    #endregion
}
