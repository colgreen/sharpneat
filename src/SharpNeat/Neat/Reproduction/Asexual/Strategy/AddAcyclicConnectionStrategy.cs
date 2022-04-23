// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using Redzen.Numerics.Distributions;
using Redzen.Random;
using Redzen.Structures;
using SharpNeat.Graphs;
using SharpNeat.Graphs.Acyclic;
using SharpNeat.Neat.Genome;
using static SharpNeat.Neat.Reproduction.Asexual.Strategy.AddConnectionUtils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy;

/// <summary>
/// A NEAT genome asexual reproduction strategy based on adding a single acyclic connection.
/// </summary>
/// <typeparam name="T">Connection weight data type.</typeparam>
/// <remarks>
/// Offspring genomes are created by taking a clone of a single parent genome and adding a single acyclic connection,
/// if possible.
/// </remarks>
public sealed class AddAcyclicConnectionStrategy<T> : IAsexualReproductionStrategy<T>
    where T : struct
{
    #region Instance Fields

    readonly MetaNeatGenome<T> _metaNeatGenome;
    readonly INeatGenomeBuilder<T> _genomeBuilder;
    readonly Int32Sequence _genomeIdSeq;
    readonly Int32Sequence _generationSeq;

    readonly IStatelessSampler<T> _weightSamplerA;
    readonly IStatelessSampler<T> _weightSamplerB;
    readonly CyclicConnectionCheck _cyclicCheck;

    #endregion

    #region Constructor

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="metaNeatGenome">NEAT genome metadata.</param>
    /// <param name="genomeBuilder">NeatGenome builder.</param>
    /// <param name="genomeIdSeq">Genome ID sequence; for obtaining new genome IDs.</param>
    /// <param name="generationSeq">Generation sequence; for obtaining the current generation number.</param>
    public AddAcyclicConnectionStrategy(
        MetaNeatGenome<T> metaNeatGenome,
        INeatGenomeBuilder<T> genomeBuilder,
        Int32Sequence genomeIdSeq,
        Int32Sequence generationSeq)
    {
        _metaNeatGenome = metaNeatGenome;
        _genomeBuilder = genomeBuilder;
        _genomeIdSeq = genomeIdSeq;
        _generationSeq = generationSeq;

        _weightSamplerA = UniformDistributionSamplerFactory.CreateStatelessSampler<T>(metaNeatGenome.ConnectionWeightScale, true);
        _weightSamplerB = UniformDistributionSamplerFactory.CreateStatelessSampler<T>(metaNeatGenome.ConnectionWeightScale * 0.01, true);
        _cyclicCheck = new CyclicConnectionCheck();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Create a new child genome from a given parent genome.
    /// </summary>
    /// <param name="parent">The parent genome.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>A new child genome.</returns>
    public NeatGenome<T>? CreateChildGenome(NeatGenome<T> parent, IRandomSource rng)
    {
        Debug.Assert(_metaNeatGenome == parent.MetaNeatGenome, "Parent genome has unexpected MetaNeatGenome.");

        // Attempt to find a new connection that we can add to the genome.
        if(!TryGetConnection(parent, rng, out DirectedConnection directedConn, out int insertIdx))
        {
            // Failed to find a new connection.
            return null;
        }

        // Determine the connection weight.
        // 50% of the time use weights very close to zero.
        // Note. this recreates the strategy used in SharpNEAT 2.x.
        // ENHANCEMENT: Reconsider the distribution of new weights and if there are better approaches (distributions) we could use.
        T weight = rng.NextBool() ? _weightSamplerB.Sample(rng) : _weightSamplerA.Sample(rng);

        // Create a new connection gene array that consists of the parent connection genes plus the new gene
        // inserted at the correct (sorted) position.
        var parentConnArr = parent.ConnectionGenes._connArr;
        var parentWeightArr = parent.ConnectionGenes._weightArr;
        int parentLen = parentConnArr.Length;

        // Create the child genome's ConnectionGenes object.
        int childLen = parentLen + 1;
        var connGenes = new ConnectionGenes<T>(childLen);
        var connArr = connGenes._connArr;
        var weightArr = connGenes._weightArr;

        // Copy genes up to insertIdx.
        Array.Copy(parentConnArr, connArr, insertIdx);
        Array.Copy(parentWeightArr, weightArr, insertIdx);

        // Copy the new genome into its insertion point.
        connArr[insertIdx] = new DirectedConnection(
            directedConn.SourceId,
            directedConn.TargetId);

        weightArr[insertIdx] = weight;

        // Copy remaining genes (if any).
        Array.Copy(parentConnArr, insertIdx, connArr, insertIdx+1, parentLen-insertIdx);
        Array.Copy(parentWeightArr, insertIdx, weightArr, insertIdx+1, parentLen-insertIdx);

        // Create and return a new genome.
        // Notes.
        // The set of hidden node IDs remains unchanged from the parent, therefore we are able to re-use parent.HiddenNodeIdArray.
        // However, the presence of a new connection invalidates parent.NodeIndexByIdMap for use in the new genome, because the allocated
        // node indexes are dependent on node depth in the acyclic graph, which in turn can be modified by the presence of a new connection.
        return _genomeBuilder.Create(
            _genomeIdSeq.Next(),
            _generationSeq.Peek,
            connGenes,
            parent.HiddenNodeIdArray);
    }

    #endregion

    #region Private Methods

    // TODO / ENHANCEMENT: parent.DirectedGraph contains a pre-built DirectedGraphAcyclic, we can probably use this to intelligently select new
    // acyclic connections, instead of the approach here of selecting random connections and testing if they are acyclic or not.

    // rather than he current rejection sampling approach.
    private bool TryGetConnection(
        NeatGenome<T> parent,
        IRandomSource rng,
        out DirectedConnection conn,
        out int insertIdx)
    {
        // Make several attempts at find a new connection, if not successful then give up.
        for(int attempts=0; attempts < 5; attempts++)
        {
            if(TryGetConnectionInner(parent, rng, out conn, out insertIdx))
                return true;
        }

        conn = default;
        insertIdx = default;
        return false;
    }

    private bool TryGetConnectionInner(
        NeatGenome<T> parent,
        IRandomSource rng,
        out DirectedConnection conn,
        out int insertIdx)
    {
        int inputCount = _metaNeatGenome.InputNodeCount;
        int outputCount = _metaNeatGenome.OutputNodeCount;
        int hiddenCount = parent.HiddenNodeIdArray.Length;

        // Select a source node at random.

        // Note. Valid source nodes are input and hidden nodes. Output nodes are not source node candidates
        // for acyclic nets, because that can prevent future connections from targeting the output if it would
        // create a cycle.
        int inputHiddenCount = inputCount + hiddenCount;
        int srcIdx = rng.Next(inputHiddenCount);

        if(srcIdx >= inputCount)
            srcIdx += outputCount;

        int srcId = GetNodeIdFromIndex(parent, srcIdx);

        // Select a target node at random.
        // Note. Valid target nodes are all hidden and output nodes (cannot be an input node).
        int outputHiddenCount = outputCount + hiddenCount;
        int tgtId = GetNodeIdFromIndex(parent, inputCount + rng.Next(outputHiddenCount));

        // Test for simplest cyclic connectivity - node connects to itself.
        if(srcId == tgtId)
        {
            conn = default;
            insertIdx = default;
            return false;
        }

        // Test if the chosen connection already exists.
        // Note. Connection genes are always sorted by sourceId then targetId, so we can use a binary search to
        // find an existing connection in O(log(n)) time.
        conn = new DirectedConnection(srcId, tgtId);

        if((insertIdx = Array.BinarySearch(parent.ConnectionGenes._connArr, conn)) >= 0)
        {
            // The proposed new connection already exists.
            conn = default;
            insertIdx = default;
            return false;
        }

        // Test if the connection would form a cycle if added to the parent genome.
        if(_cyclicCheck.IsConnectionCyclic(parent.DirectedGraph, DirectedConnectionUtils.CloneAndMap(conn, parent.NodeIndexByIdMap)))
        {
            conn = default;
            insertIdx = default;
            return false;
        }

        // Get the position in parent.ConnectionGeneArray that the new connection should be inserted at (to maintain sort order).
        insertIdx = ~insertIdx;
        return true;
    }

    #endregion
}
