// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Numerics;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.Neat.Genome;

/// <summary>
/// For building instances of <see cref="NeatGenome{T}"/>. For use when evolving acyclic graphs only.
/// </summary>
/// <typeparam name="TWeight">Connection weight data type.</typeparam>
public sealed class NeatGenomeBuilderAcyclic<TWeight> : INeatGenomeBuilder<TWeight>
    where TWeight : unmanaged, IBinaryFloatingPointIeee754<TWeight>
{
    readonly MetaNeatGenome<TWeight> _metaNeatGenome;
    readonly AcyclicGraphDepthAnalysis _graphDepthAnalysis;
    readonly HashSet<int> _workingIdSet;

    // Temp working data for timsort. Allocated once and re-used to minimise object allocate and GC overhead.
    int[]? _timesortWorkArr;
    int[]? _timesortWorkVArr;

    #region Constructor

    /// <summary>
    /// Construct with the given NEAT genome metadata.
    /// </summary>
    /// <param name="metaNeatGenome">NEAT genome metadata.</param>
    /// <param name="validateAcyclic">Enable acyclic graph validation.</param>
    /// <remarks>
    /// If the caller can guarantee that calls to Create() will provide acyclic graphs only, then
    /// <paramref name="validateAcyclic"/> can be set to false to avoid the cost of the cyclic graph check (which is relatively expensive to perform).
    /// </remarks>
    public NeatGenomeBuilderAcyclic(MetaNeatGenome<TWeight> metaNeatGenome, bool validateAcyclic)
    {
        Debug.Assert(metaNeatGenome is not null && metaNeatGenome.IsAcyclic);
        _metaNeatGenome = metaNeatGenome;
        _graphDepthAnalysis = new AcyclicGraphDepthAnalysis(validateAcyclic);
        _workingIdSet = [];
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public NeatGenome<TWeight> Create(
        int id,
        int birthGeneration,
        ConnectionGenes<TWeight> connGenes)
    {
        // Determine the set of node IDs, and create a mapping from node IDs to node indexes.
        int[] hiddenNodeIdArr = ConnectionGenesUtils.CreateHiddenNodeIdArray(connGenes._connArr, _metaNeatGenome.InputOutputNodeCount, _workingIdSet);

        return Create(id, birthGeneration, connGenes, hiddenNodeIdArr);
    }

    /// <inheritdoc/>
    public NeatGenome<TWeight> Create(
        int id, int birthGeneration,
        ConnectionGenes<TWeight> connGenes,
        int[] hiddenNodeIdArr)
    {
        int inputCount = _metaNeatGenome.InputNodeCount;

        // Create a mapping from node IDs to node indexes.
        Dictionary<int,int> nodeIdxById = BuildNodeIndexById(hiddenNodeIdArr);

        // Create a DictionaryNodeIdMap.
        DictionaryNodeIdMap nodeIndexByIdMap = new(inputCount, nodeIdxById);

        // Create a digraph from the genome.
        DirectedGraph digraph = NeatGenomeBuilderUtils.CreateDirectedGraph(
            _metaNeatGenome, connGenes, nodeIndexByIdMap);

        // Calc the depth of each node in the digraph.
        GraphDepthInfo depthInfo = _graphDepthAnalysis.CalculateNodeDepths(digraph);

        // Create a weighted acyclic digraph.
        // Note. This also outputs connectionIndexMap. For each connection in the acyclic graph this gives
        // the index of the same connection in the genome; this is because connections are re-ordered based
        // on node depth in the acyclic graph.
        DirectedGraphAcyclic acyclicDigraph = DirectedGraphAcyclicBuilderUtils.CreateDirectedGraphAcyclic(
            digraph,
            depthInfo,
            out int[] newIdByOldId,
            out int[] connectionIndexMap,
            ref _timesortWorkArr,
            ref _timesortWorkVArr);

        // TODO: Write unit tests to cover this!
        // Update nodeIdxById with the new depth based node index allocations.
        // Notes.
        // The current nodeIndexByIdMap maps node IDs (also know as innovation IDs in NEAT) to a compact
        // ID space in which any gaps have been removed, i.e. a compacted set of IDs that can be used as indexes,
        // i.e. if there are N nodes in total then the highest node ID will be N-1.
        //
        // Here we map the new compact IDs to an alternative ID space that is also compact, but ensures that nodeIDs
        // reflect the depth of a node in the acyclic graph.
        UpdateNodeIndexById(nodeIdxById, hiddenNodeIdArr, newIdByOldId);

        // Create the neat genome.
        return new NeatGenome<TWeight>(
            _metaNeatGenome, id, birthGeneration,
            connGenes,
            hiddenNodeIdArr,
            nodeIndexByIdMap,
            acyclicDigraph,
            connectionIndexMap);
    }

    /// <inheritdoc/>
    public NeatGenome<TWeight> Create(
        int id, int birthGeneration,
        ConnectionGenes<TWeight> connGenes,
        int[] hiddenNodeIdArr,
        INodeIdMap nodeIndexByIdMap)
    {
        // Note. Not required for acyclic graphs.
        // In acyclic graphs nodeIndexByIdMap is so closely related/tied to digraph and connectionIndexMap that
        // these three objects exist as a logical unit, i.e. we get all three or none at all.
        throw new NotImplementedException();
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
        return new NeatGenome<TWeight>(_metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap, digraph, connectionIndexMap);
    }

    #endregion

    #region Private Methods

    private Dictionary<int,int> BuildNodeIndexById(int[] hiddenNodeIdArr)
    {
        int inputCount = _metaNeatGenome.InputNodeCount;
        int outputCount = _metaNeatGenome.OutputNodeCount;
        int inputOutputCount = _metaNeatGenome.InputOutputNodeCount;

        // Create a mapping from node IDs to node indexes.
        var nodeIdxById = new Dictionary<int,int>(outputCount + hiddenNodeIdArr.Length);

        // Insert fixed output node IDs (these will become unfixed later, hence they are added to the dictionary
        // rather than just being covered by DictionaryNodeIdMap.fixedNodeCount.)
        // Output node indexes start after the last input node index.
        for(int nodeIdx = inputCount; nodeIdx < inputOutputCount; nodeIdx++)
            nodeIdxById.Add(nodeIdx, nodeIdx);

        // Insert the hidden node ID mappings. Hidden nodes are allocated node indexes starting directly
        // after the last output node index.
        for(int i=0, nodeIdx = inputOutputCount; i < hiddenNodeIdArr.Length; i++)
            nodeIdxById.Add(hiddenNodeIdArr[i], nodeIdx + i);

        return nodeIdxById;
    }

    private void UpdateNodeIndexById(
        Dictionary<int,int> nodeIdxById,
        int[] hiddenNodeIdArr,
        int[] newIdxByOldIdx)
    {
        // Note. This method essentially repeats the logic in BuildNodeIndexById() but
        // the values placed into the dictionary are mapped to different values, and
        // we are updating existing dictionary entries rather than inserting new ones.
        //
        // This still requires dictionary lookups and so can be optimised further with
        // a customised dictionary implementation that allows direct access and updating
        // of the keyed values. We could do that here by wrapping each entry in an object
        // reference (i.e. boxing), but that creates additional overhead (object header
        // allocation, heap allocation, garbage collection, etc.).
        int inputCount = _metaNeatGenome.InputNodeCount;
        int inputOutputCount = _metaNeatGenome.InputOutputNodeCount;

        // Update the fixed output node IDs.
        // Pre-mapped output node indexes start after the last input node index.
        for(int nodeIdx = inputCount; nodeIdx < inputOutputCount; nodeIdx++)
            nodeIdxById[nodeIdx] = newIdxByOldIdx[nodeIdx];

        // Update the hidden node ID mappings.
        // Pre-mapped hidden nodes are allocated node indexes starting directly
        // after the last output node index.
        for(int i=0, nodeIdx = inputOutputCount; i < hiddenNodeIdArr.Length; i++)
            nodeIdxById[hiddenNodeIdArr[i]] = newIdxByOldIdx[nodeIdx + i];
    }

    #endregion
}
