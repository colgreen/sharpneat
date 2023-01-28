using Redzen.Random;
using Redzen.Sorting;
using Redzen.Structures;
using SharpNeat.Graphs;
using SharpNeat.Neat.Genome;
using Xunit;
using static SharpNeat.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy;

public class DeleteConnectionStrategyTests
{
    [Fact]
    public void DeleteConnection()
    {
        var pop = CreateNeatPopulation();
        var generationSeq = new Int32Sequence();
        var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
        var genome = pop.GenomeList[0];

        var strategy = new DeleteConnectionStrategy<double>(
            genomeBuilder, pop.GenomeIdSeq, generationSeq);

        IRandomSource rng = RandomDefaults.CreateRandomSource();

        var connSet = GetDirectedConnectionSet(genome);

        for(int i=0; i < 1000; i++)
        {
            var childGenome = strategy.CreateChildGenome(genome, rng);

            // The child genome should have one less connection than the parent.
            Assert.Equal(genome.ConnectionGenes.Length - 1, childGenome.ConnectionGenes.Length);

            // The child genome's connections should be a proper subset of the parent genome's.
            var childConnSet = GetDirectedConnectionSet(childGenome);
            Assert.True(childConnSet.IsProperSubsetOf(connSet));

            // The connection genes should be sorted.
            Assert.True(SortUtils.IsSortedAscending<DirectedConnection>(childGenome.ConnectionGenes._connArr));

            // Test that the array of hidden node IDs is correct, i.e. corresponds with the hidden node IDs described by the connections.
            Assert.True(ConnectionGenesUtils.ValidateHiddenNodeIds(
                childGenome.HiddenNodeIdArray,
                childGenome.ConnectionGenes._connArr,
                childGenome.MetaNeatGenome.InputOutputNodeCount));
        }
    }
}
