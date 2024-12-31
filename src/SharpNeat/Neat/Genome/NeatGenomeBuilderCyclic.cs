// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Numerics;

namespace SharpNeat.Neat.Genome;

/// <summary>
/// For building instances of <see cref="NeatGenome{T}"/>. For use when evolving cyclic graphs only.
/// </summary>
/// <typeparam name="TWeight">Connection weight data type.</typeparam>
public sealed class NeatGenomeBuilderCyclic<TWeight> : INeatGenomeBuilder<TWeight>
    where TWeight : unmanaged, IBinaryFloatingPointIeee754<TWeight>
{
    readonly MetaNeatGenome<TWeight> _metaNeatGenome;
    readonly HashSet<int> _workingIdSet;

    /// <summary>
    /// Construct with the given NEAT genome metadata.
    /// </summary>
    /// <param name="metaNeatGenome">NEAT genome metadata.</param>
    public NeatGenomeBuilderCyclic(MetaNeatGenome<TWeight> metaNeatGenome)
    {
        Debug.Assert(metaNeatGenome is not null && !metaNeatGenome.IsAcyclic);
        _metaNeatGenome = metaNeatGenome;
        _workingIdSet = [];
    }

    #region Public Methods

    /// <inheritdoc/>
    public NeatGenome<TWeight> Create(
        int id,
        int birthGeneration,
        ConnectionGenes<TWeight> connGenes)
    {
        // Determine the set of node IDs, and create a mapping from node IDs to node indexes.
        int[] hiddenNodeIdArr = ConnectionGenesUtils.CreateHiddenNodeIdArray(
            connGenes._connArr,
            _metaNeatGenome.InputOutputNodeCount,
            _workingIdSet);

        return Create(id, birthGeneration, connGenes, hiddenNodeIdArr);
    }

    /// <inheritdoc/>
    public NeatGenome<TWeight> Create(
        int id, int birthGeneration,
        ConnectionGenes<TWeight> connGenes,
        int[] hiddenNodeIdArr)
    {
        // Create a mapping from node IDs to node indexes.
        INodeIdMap nodeIndexByIdMap = DirectedGraphBuilderUtils.CompileNodeIdMap(
            _metaNeatGenome.InputOutputNodeCount, hiddenNodeIdArr);

        return Create(id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap);
    }

    /// <inheritdoc/>
    public NeatGenome<TWeight> Create(
        int id, int birthGeneration,
        ConnectionGenes<TWeight> connGenes,
        int[] hiddenNodeIdArr,
        INodeIdMap nodeIndexByIdMap)
    {
        // Create a digraph from the genome.
        DirectedGraph digraph = NeatGenomeBuilderUtils.CreateDirectedGraph(
            _metaNeatGenome, connGenes, nodeIndexByIdMap);

        return new NeatGenome<TWeight>(
            _metaNeatGenome,
            id,
            birthGeneration,
            connGenes,
            hiddenNodeIdArr,
            nodeIndexByIdMap,
            digraph,
            null);
    }

    /// <inheritdoc/>
    public NeatGenome<TWeight> Create(
        int id, int birthGeneration,
        ConnectionGenes<TWeight> connGenes,
        int[] hiddenNodeIdArr,
        INodeIdMap nodeIndexByIdMap,
        DirectedGraph digraph,
        int[]? connectionIndexMap)
    {
        // This should always be null when evolving cyclic genomes/graphs.
        Debug.Assert(connectionIndexMap is null);

        return new NeatGenome<TWeight>(
            _metaNeatGenome,
            id,
            birthGeneration,
            connGenes,
            hiddenNodeIdArr,
            nodeIndexByIdMap,
            digraph,
            null);
    }

    #endregion
}
