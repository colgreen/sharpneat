// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Reproduction.Asexual.Strategies;

/// <summary>
/// A NEAT genome asexual reproduction strategy based on deletion of a single connection.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
/// <remarks>
/// Offspring genomes are created by taking a clone of a single parent genome and deleting a single
/// connection, if possible.
/// </remarks>
public sealed class DeleteConnectionStrategy<TScalar> : IAsexualReproductionStrategy<TScalar>
    where TScalar : struct
{
    readonly INeatGenomeBuilder<TScalar> _genomeBuilder;
    readonly Int32Sequence _genomeIdSeq;
    readonly Int32Sequence _generationSeq;

    #region Constructor

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="genomeBuilder">NeatGenome builder.</param>
    /// <param name="genomeIdSeq">Genome ID sequence; for obtaining new genome IDs.</param>
    /// <param name="generationSeq">Generation sequence; for obtaining the current generation number.</param>
    public DeleteConnectionStrategy(
        INeatGenomeBuilder<TScalar> genomeBuilder,
        Int32Sequence genomeIdSeq,
        Int32Sequence generationSeq)
    {
        _genomeBuilder = genomeBuilder;
        _genomeIdSeq = genomeIdSeq;
        _generationSeq = generationSeq;
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public NeatGenome<TScalar>? CreateChildGenome(
        NeatGenome<TScalar> parent,
        IRandomSource rng)
    {
        // We require at least two connections in the parent, i.e. we avoid creating genomes with
        // no connections, which would be pointless.
        if(parent.ConnectionGenes.Length < 2)
            return null;

        // Select a gene at random to delete.
        var parentConnArr = parent.ConnectionGenes._connArr;
        var parentWeightArr = parent.ConnectionGenes._weightArr;
        int parentLen = parentConnArr.Length;
        int deleteIdx = rng.Next(parentLen);

        // Create the child genome's ConnectionGenes object.
        int childLen = parentLen - 1;
        var connGenes = new ConnectionGenes<TScalar>(childLen);
        var connArr = connGenes._connArr;
        var weightArr = connGenes._weightArr;

        // Copy genes up to deleteIdx.
        Array.Copy(parentConnArr, connArr, deleteIdx);
        Array.Copy(parentWeightArr, weightArr, deleteIdx);

        // Copy remaining genes (if any).
        Array.Copy(parentConnArr, deleteIdx+1, connArr, deleteIdx, childLen-deleteIdx);
        Array.Copy(parentWeightArr, deleteIdx+1, weightArr, deleteIdx, childLen-deleteIdx);

        // Get an array of hidden node IDs.
        var hiddenNodeIdArr = GetHiddenNodeIdArray(parent, deleteIdx, connArr);

        // Create and return a new genome.
        return _genomeBuilder.Create(
            _genomeIdSeq.Next(),
            _generationSeq.Peek,
            connGenes,
            hiddenNodeIdArr);
    }

    #endregion

    #region Private Static Methods

    /// <summary>
    /// Get an array of hidden node IDs in the child genome.
    /// </summary>
    private static int[] GetHiddenNodeIdArray(
        NeatGenome<TScalar> parent, int deleteIdx,
        DirectedConnection[] childConnArr)
    {
        // Determine which hidden nodes in the parent have been deleted, i.e. are not in the child.
        (int? nodeId1, int? nodeId2) = GetDeletedNodeIds(parent, deleteIdx, childConnArr);
        if(!nodeId1.HasValue && !nodeId2.HasValue)
        {
            // The connection deletion resulted in no hidden nodes being deleted, therefore we can re-use
            // the parent's hidden node ID array.
            return parent.HiddenNodeIdArray;
        }

        // Determine the length of the new ID array, and allocate memory.
        int childLen = parent.HiddenNodeIdArray.Length;
        if(nodeId1.HasValue && nodeId2.HasValue)
            childLen -= 2;
        else
            childLen--;

        int[] childIdArr = new int[childLen];

        // Copy the parent's hidden node IDs, except the removed IDs.
        int[] parentIdArr = parent.HiddenNodeIdArray;
        for(int parentIdx=0, childIdx=0; parentIdx < parentIdArr.Length; parentIdx++)
        {
            int nodeId = parentIdArr[parentIdx];
            if(nodeId != nodeId1 && nodeId != nodeId2)
                childIdArr[childIdx++] = parentIdArr[parentIdx];
        }

        return childIdArr;
    }

    /// <summary>
    /// Determine the set of hidden node IDs that have been deleted as a result of a connection deletion.
    /// I.e. a node only exists if a connection connects to it, therefore if there are no other connections
    /// referring to a node then it has been deleted, with the exception of input and output nodes that
    /// always exist.
    /// </summary>
    private static (int? nodeId1, int? nodeId2) GetDeletedNodeIds(
        NeatGenome<TScalar> parent, int deleteIdx,
        DirectedConnection[] childConnArr)
    {
        // Get the two node IDs referred to by the deleted connection.
        int? nodeId1 = parent.ConnectionGenes._connArr[deleteIdx].SourceId;
        int? nodeId2 = parent.ConnectionGenes._connArr[deleteIdx].TargetId;

        // Set IDs to null for input/output nodes (these nodes are fixed and therefore cannot be deleted).
        // Also, for cyclic networks nodeId1 and 2 could refer to the same node (a cyclic connection on the same node), if
        // so then we don't need to set nodeId2.
        if(nodeId1.Value < parent.MetaNeatGenome.InputOutputNodeCount)
            nodeId1 = null;

        if(nodeId2.Value < parent.MetaNeatGenome.InputOutputNodeCount || nodeId1 == nodeId2)
            nodeId2 = null;

        if(!nodeId1.HasValue && !nodeId2.HasValue)
            return (null, null);

        if(!nodeId1.HasValue)
        {
            // 'Shuffle up' nodeId2 into nodeId1.
            nodeId1 = nodeId2;
            nodeId2 = null;
        }

        if(!nodeId2.HasValue)
        {
            if(!IsNodeConnectedTo(childConnArr, nodeId1!.Value))
                return (nodeId1.Value, null);

            return (null, null);
        }

        (bool,bool) isConnectedTuple = AreNodesConnectedTo(childConnArr, nodeId1!.Value, nodeId2.Value);
        if(!isConnectedTuple.Item1 && !isConnectedTuple.Item2)
            return (nodeId1.Value, nodeId2.Value);

        if(!isConnectedTuple.Item1)
            return (nodeId1.Value, null);

        if(!isConnectedTuple.Item2)
            return (nodeId2.Value, null);

        return (null, null);
    }

    /// <summary>
    /// Is nodeId referred to by any of the connections in connArr.
    /// </summary>
    private static bool IsNodeConnectedTo(DirectedConnection[] connArr, int nodeId)
    {
        // Is nodeId referred to by any of the connections in connArr.
        foreach(var conn in connArr)
        {
            if(conn.SourceId == nodeId || conn.TargetId == nodeId)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Are nodeId1 and nodeId2 connected to by any of the connections in connArr.
    /// </summary>
    private static (bool isNode1Connected, bool isNode2Connected) AreNodesConnectedTo(DirectedConnection[] connArr, int nodeId1, int nodeId2)
    {
        bool id1Used = false;
        bool id2Used = false;

        foreach(var conn in connArr)
        {
            if(conn.SourceId == nodeId1 || conn.TargetId == nodeId1)
            {
                id1Used = true;
                if(id2Used)
                    break;
            }
            if(conn.SourceId == nodeId2 || conn.TargetId == nodeId2)
            {
                id2Used = true;
                if(id1Used)
                    break;
            }
        }
        return (id1Used, id2Used);
    }

    #endregion
}
