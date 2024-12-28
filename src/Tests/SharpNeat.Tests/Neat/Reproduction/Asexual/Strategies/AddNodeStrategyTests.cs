using Redzen.Random;
using Redzen.Sorting;
using Redzen.Structures;
using SharpNeat.Graphs;
using SharpNeat.Neat.Genome;
using Xunit;
using static SharpNeat.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategies;

public class AddNodeStrategyTests
{
    /// <summary>
    /// Apply 'add node' mutations to the same initial genome multiple times.
    /// Note. The mutations are random, therefore this tests different mutations on each loop.
    /// </summary>
    [Fact]
    public void AddNode1()
    {
        var pop = CreateNeatPopulation();
        var generationSeq = new Int32Sequence();
        var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
        var genome = pop.GenomeList[0];

        var strategy = new AddNodeStrategy<double>(
            pop.MetaNeatGenome, genomeBuilder,
            pop.GenomeIdSeq, pop.InnovationIdSeq, generationSeq,
            pop.AddedNodeBuffer);

        IRandomSource rng = RandomDefaults.CreateRandomSource();

        for(int i=0; i < 10_000; i++)
        {
            using var childGenome = CreateAndTestChildGenome(genome, strategy, rng);
        }
    }

    /// <summary>
    /// Apply cumulative 'add node' mutations.
    /// This explores more complex genome structures as the mutations accumulate.
    /// </summary>
    [Fact]
    public void AddNode2()
    {
        var pop = CreateNeatPopulation();
        var generationSeq = new Int32Sequence();
        var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
        var genome = pop.GenomeList[0];

        var strategy = new AddNodeStrategy<double>(
            pop.MetaNeatGenome, genomeBuilder,
            pop.GenomeIdSeq, pop.InnovationIdSeq, generationSeq,
            pop.AddedNodeBuffer);

        IRandomSource rng = RandomDefaults.CreateRandomSource();

        for(int i = 0; i < 2000; i++)
        {
            NeatGenome<double> childGenome = CreateAndTestChildGenome(genome, strategy, rng);

            // Make the child genome the parent in the next iteration. I.e. accumulate add node mutations.
            genome.Dispose();
            genome = childGenome;
        }
    }

    /// <summary>
    /// Apply cumulative 'add node' mutations to 10 genomes, rather than the single genome in TestAddNode2().
    /// This results in some mutations occurring that have already occurred on one of the other genomes,
    /// and therefore this tests the code paths that handle re-using innovation IDs obtain from the innovation buffers.
    /// </summary>
    [Fact]
    public void AddNode3()
    {
        var pop = CreateNeatPopulation();
        var generationSeq = new Int32Sequence();
        var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
        var genome = pop.GenomeList[0];

        var strategy = new AddNodeStrategy<double>(
            pop.MetaNeatGenome, genomeBuilder,
            pop.GenomeIdSeq, pop.InnovationIdSeq, generationSeq,
            pop.AddedNodeBuffer);

        IRandomSource rng = RandomDefaults.CreateRandomSource();

        CircularBuffer<NeatGenome<double>> genomeRing = new(10);
        genomeRing.Enqueue(genome);

        for(int i=0; i < 5000; i++)
        {
            NeatGenome<double> childGenome = CreateAndTestChildGenome(genome, strategy, rng);

            // Add the new child genome to the ring.
            genomeRing.Enqueue(childGenome);

            // Take the genome at the tail of the ring for the next parent.
            genome = genomeRing[0];
        }
    }

    #region Private Static Methods

    private static NeatGenome<double> CreateAndTestChildGenome(
        NeatGenome<double> parentGenome,
        AddNodeStrategy<double> strategy,
        IRandomSource rng)
    {
        var nodeIdSet = GetNodeIdSet(parentGenome);
        var connSet = GetDirectedConnectionSet(parentGenome);

        var childGenome = strategy.CreateChildGenome(parentGenome, rng);

        // The connection genes should be sorted.
        Assert.True(SortUtils.IsSortedAscending<DirectedConnection>(childGenome.ConnectionGenes._connArr));

        // The child genome should have one more connection than parent.
        Assert.Equal(parentGenome.ConnectionGenes.Length + 1, childGenome.ConnectionGenes.Length);

        // The child genome should have one more node ID than the parent.
        var childNodeIdSet = GetNodeIdSet(childGenome);
        var newNodeIdList = new List<int>(childNodeIdSet.Except(nodeIdSet));
        Assert.Single(newNodeIdList);
        int newNodeId = newNodeIdList[0];

        // The child genome's new connections should not be a duplicate of any of the existing/parent connections.
        var childConnSet = GetDirectedConnectionSet(childGenome);
        var newConnList = new List<DirectedConnection>(childConnSet.Except(connSet));
        Assert.Equal(2, newConnList.Count);

        // The parent should have one connection that the child does not, i.e. the connection that was replaced.
        var removedConnList = new List<DirectedConnection>(connSet.Except(childConnSet));
        Assert.Single(removedConnList);

        // The two new connections should connect to the new node ID.
        var connRemoved = removedConnList[0];
        var connA = newConnList[0];
        var connB = newConnList[1];
        Assert.True(
                (connA.SourceId == connRemoved.SourceId && connA.TargetId == newNodeId && connB.SourceId == newNodeId && connB.TargetId == connRemoved.TargetId)
            ||  (connB.SourceId == connRemoved.SourceId && connB.TargetId == newNodeId && connA.SourceId == newNodeId && connA.TargetId == connRemoved.TargetId));

        return childGenome;
    }

    #endregion
}
