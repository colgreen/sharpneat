﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using Redzen.Numerics.Distributions;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.Reproduction.Asexual.Strategies;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;

namespace SharpNeat.Neat.Reproduction.Asexual;

/// <summary>
/// Creation of offspring given a single parent (asexual reproduction).
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public class NeatAsexualReproduction<TScalar> : IAsexualReproductionStrategy<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    readonly MutationTypeDistributions _mutationTypeDistributionsComplexifying;
    readonly MutationTypeDistributions _mutationTypeDistributionsSimplifying;
    MutationTypeDistributions _mutationTypeDistributionsCurrent;

    // Asexual reproduction strategies.
    readonly MutateWeightsStrategy<TScalar> _mutateWeightsStrategy;
    readonly DeleteConnectionStrategy<TScalar> _deleteConnectionStrategy;
    readonly IAsexualReproductionStrategy<TScalar> _addConnectionStrategy;
    readonly AddNodeStrategy<TScalar> _addNodeStrategy;

    #region Constructor

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="metaNeatGenome">NeatGenome metadata.</param>
    /// <param name="genomeBuilder">NeatGenome builder.</param>
    /// <param name="genomeIdSeq">Genome ID sequence; for obtaining new genome IDs.</param>
    /// <param name="innovationIdSeq">Innovation ID sequence; for obtaining new innovation IDs.</param>
    /// <param name="generationSeq">Generation sequence; for obtaining the current generation number.</param>
    /// <param name="addedNodeBuffer">A history buffer of added nodes.</param>
    /// <param name="settings">Asexual reproduction settings.</param>
    /// <param name="weightMutationScheme">Connection weight mutation scheme.</param>
    public NeatAsexualReproduction(
        MetaNeatGenome<TScalar> metaNeatGenome,
        INeatGenomeBuilder<TScalar> genomeBuilder,
        Int32Sequence genomeIdSeq,
        Int32Sequence innovationIdSeq,
        Int32Sequence generationSeq,
        AddedNodeBuffer addedNodeBuffer,
        NeatAsexualReproductionSettings settings,
        WeightMutationScheme<TScalar> weightMutationScheme)
    {
        var settingsComplexifying = settings;
        var settingsSimplifying = settings.CreateSimplifyingSettings();

        _mutationTypeDistributionsComplexifying = new MutationTypeDistributions(settingsComplexifying);
        _mutationTypeDistributionsSimplifying = new MutationTypeDistributions(settingsSimplifying);
        _mutationTypeDistributionsCurrent = _mutationTypeDistributionsComplexifying;

        // Instantiate reproduction strategies.
        _mutateWeightsStrategy = new MutateWeightsStrategy<TScalar>(genomeBuilder, genomeIdSeq, generationSeq, weightMutationScheme);
        _deleteConnectionStrategy = new DeleteConnectionStrategy<TScalar>(genomeBuilder, genomeIdSeq, generationSeq);

        // Add connection mutation; select acyclic/cyclic strategy as appropriate.
        if(metaNeatGenome.IsAcyclic)
        {
            _addConnectionStrategy = new AddAcyclicConnectionStrategy<TScalar>(
                metaNeatGenome, genomeBuilder,
                genomeIdSeq, generationSeq);
        }
        else
        {
            _addConnectionStrategy = new AddCyclicConnectionStrategy<TScalar>(
                metaNeatGenome, genomeBuilder,
                genomeIdSeq, generationSeq);
        }

        _addNodeStrategy = new AddNodeStrategy<TScalar>(metaNeatGenome, genomeBuilder, genomeIdSeq, innovationIdSeq, generationSeq, addedNodeBuffer);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Notify the strategy of a change in complexity regulation mode in the evolution algorithm.
    /// </summary>
    /// <param name="mode">The current mode.</param>
    public void NotifyComplexityRegulationMode(ComplexityRegulationMode mode)
    {
        _mutationTypeDistributionsCurrent = mode switch
        {
            ComplexityRegulationMode.Complexifying => _mutationTypeDistributionsComplexifying,
            ComplexityRegulationMode.Simplifying => _mutationTypeDistributionsSimplifying,
            _ => throw new ArgumentException("Unexpected complexity regulation mode."),
        };
    }

    #endregion

    #region IAsexualReproductionStrategy

    /// <inheritdoc/>
    public NeatGenome<TScalar> CreateChildGenome(NeatGenome<TScalar> parent, IRandomSource rng)
    {
        // Get a discrete distribution over the set of possible mutation types.
        var mutationTypeDist = GetMutationTypeDistribution(parent);

        // Keep trying until a child genome is created.
        while(true)
        {
            NeatGenome<TScalar>? childGenome = Create(parent, rng, ref mutationTypeDist);
            if(childGenome is not null)
                return childGenome;
        }
    }

    #endregion

    #region Private Methods [Create Subroutines]

    private NeatGenome<TScalar>? Create(
        NeatGenome<TScalar> parent,
        IRandomSource rng,
        ref DiscreteDistribution<double> mutationTypeDist)
    {
        // Determine the type of mutation to attempt.
        MutationType mutationTypeId = (MutationType)mutationTypeDist.Sample(rng);

        // Attempt to create a child genome using the selected mutation type.
        NeatGenome<TScalar>? childGenome = mutationTypeId switch
        {
            // Note. These subroutines will return null if they cannot produce a child genome,
            // e.g. 'delete connection' will not succeed if there is only one connection.
            MutationType.ConnectionWeight => _mutateWeightsStrategy.CreateChildGenome(parent, rng),
            MutationType.AddNode => _addNodeStrategy.CreateChildGenome(parent, rng),
            MutationType.AddConnection => _addConnectionStrategy.CreateChildGenome(parent, rng),
            MutationType.DeleteConnection => _deleteConnectionStrategy.CreateChildGenome(parent, rng),
            _ => throw new InvalidOperationException($"Unexpected mutationTypeId [{mutationTypeId}]."),
        };

        if(childGenome is not null)
            return childGenome;

        // The chosen mutation type was not possible; remove that type from the set of possible types.
        mutationTypeDist = mutationTypeDist.RemoveOutcome((int)mutationTypeId);

        // Sanity test.
        if(mutationTypeDist.Probabilities.Length == 0)
        {
            // This shouldn't be possible, hence this is an exceptional circumstance.
            // Note. Connection weight and 'add node' mutations should always be possible, because there should
            // always be at least one connection.
            throw new InvalidOperationException("All types of genome mutation failed.");
        }

        return null;
    }

    #endregion

    #region Private Methods

    private DiscreteDistribution<double> GetMutationTypeDistribution(NeatGenome<TScalar> parent)
    {
        // If there is only one connection then avoid destructive mutations to avoid the
        // creation of genomes with no connections.
        DiscreteDistribution<double> dist = (parent.ConnectionGenes.Length < 2) ?
              _mutationTypeDistributionsCurrent.MutationTypeDistributionNonDestructive
            : _mutationTypeDistributionsCurrent.MutationTypeDistribution;

        return dist;
    }

    #endregion
}
