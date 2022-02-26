using Redzen.Random;
using Redzen.Sorting;
using Redzen.Structures;
using SharpNeat.Graphs;
using SharpNeat.Neat.Genome;
using Xunit;
using static SharpNeat.Neat.Genome.Tests.NestGenomeTestUtils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy.Tests;

public class AddCyclicConnectionStrategyTests
{
    #region Test Methods

    [Fact]
    public void AddCyclicConnection()
    {
        var pop = CreateNeatPopulation();
        var generationSeq = new Int32Sequence();
        var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
        var genome = pop.GenomeList[0];

        var strategy = new AddCyclicConnectionStrategy<double>(
            pop.MetaNeatGenome, genomeBuilder,
            pop.GenomeIdSeq, generationSeq);

        IRandomSource rng = RandomDefaults.CreateRandomSource();

        var nodeIdSet = GetNodeIdSet(genome);
        var connSet = GetDirectedConnectionSet(genome);

        const int loops = 1000;
        int nullResponseCount = 0;

        for(int i=0; i < loops; i++)
        {
            var childGenome = strategy.CreateChildGenome(genome, rng);

            // The strategy may return null if no appropriately connection could be found to add.
            if(childGenome is null)
            {
                nullResponseCount++;
                continue;
            }

            // The child genome should have one more connection than parent.
            Assert.Equal(genome.ConnectionGenes.Length + 1, childGenome.ConnectionGenes.Length);

            // The child genome's new connection should not be a duplicate of any of the existing/parent connections.
            var childConnSet = GetDirectedConnectionSet(childGenome);
            var newConnList = new List<DirectedConnection>(childConnSet.Except(connSet));
            Assert.Single(newConnList);

            // The connection genes should be sorted.
            Assert.True(SortUtils.IsSortedAscending<DirectedConnection>(childGenome.ConnectionGenes._connArr));

            // The child genome should have the same set of node IDs as the parent.
            var childNodeIdSet = GetNodeIdSet(childGenome);
            Assert.True(nodeIdSet.SetEquals(childNodeIdSet));
        }

        // nullResponseProportion will typically be 0, with 1.0% being so unlikely
        // it probably will never be observed.
        double nullResponseProportion = nullResponseCount / (double)loops;
        Assert.True(nullResponseProportion <= 0.01);
    }

    #endregion
}
