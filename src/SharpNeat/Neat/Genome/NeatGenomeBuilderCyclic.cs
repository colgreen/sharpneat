// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using SharpNeat.Graphs;

namespace SharpNeat.Neat.Genome;

/// <summary>
/// For building instances of <see cref="NeatGenome{T}"/>. For use when evolving cyclic graphs only.
/// </summary>
/// <typeparam name="T">Connection weight data type.</typeparam>
public sealed class NeatGenomeBuilderCyclic<T> : INeatGenomeBuilder<T>
    where T : struct
{
    readonly MetaNeatGenome<T> _metaNeatGenome;
    readonly HashSet<int> _workingIdSet;

    #region Constructor

    /// <summary>
    /// Construct with the given NEAT genome metadata.
    /// </summary>
    /// <param name="metaNeatGenome">NEAT genome metadata.</param>
    public NeatGenomeBuilderCyclic(MetaNeatGenome<T> metaNeatGenome)
    {
        Debug.Assert(metaNeatGenome is not null && !metaNeatGenome.IsAcyclic);
        _metaNeatGenome = metaNeatGenome;
        _workingIdSet = new HashSet<int>();
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public NeatGenome<T> Create(
        int id,
        int birthGeneration,
        ConnectionGenes<T> connGenes)
    {
        // Determine the set of node IDs, and create a mapping from node IDs to node indexes.
        int[] hiddenNodeIdArr = ConnectionGenesUtils.CreateHiddenNodeIdArray(connGenes._connArr, _metaNeatGenome.InputOutputNodeCount, _workingIdSet);

        return Create(id, birthGeneration, connGenes, hiddenNodeIdArr);
    }

    /// <inheritdoc/>
    public NeatGenome<T> Create(
        int id, int birthGeneration,
        ConnectionGenes<T> connGenes,
        int[] hiddenNodeIdArr)
    {
        // Create a mapping from node IDs to node indexes.
        INodeIdMap nodeIndexByIdMap = DirectedGraphBuilderUtils.CompileNodeIdMap(
            _metaNeatGenome.InputOutputNodeCount, hiddenNodeIdArr);

        return Create(id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap);
    }

    /// <inheritdoc/>
    public NeatGenome<T> Create(
        int id, int birthGeneration,
        ConnectionGenes<T> connGenes,
        int[] hiddenNodeIdArr,
        INodeIdMap nodeIndexByIdMap)
    {
        // Create a digraph from the genome.
        DirectedGraph digraph = NeatGenomeBuilderUtils.CreateDirectedGraph(
            _metaNeatGenome, connGenes, nodeIndexByIdMap);

        return new NeatGenome<T>(_metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap, digraph, null);
    }

    /// <inheritdoc/>
    public NeatGenome<T> Create(
        int id, int birthGeneration,
        ConnectionGenes<T> connGenes,
        int[] hiddenNodeIdArr,
        INodeIdMap nodeIndexByIdMap,
        DirectedGraph digraph,
        int[]? connectionIndexMap)
    {
        // This should always be null when evolving cyclic genomes/graphs.
        Debug.Assert(connectionIndexMap is null);

        return new NeatGenome<T>(_metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap, digraph, null);
    }

    #endregion
}
