using Redzen.Random;
using Redzen.Sorting;
using Redzen.Structures;
using SharpNeat.Graphs;
using SharpNeat.Neat.Genome;
using Xunit;
using static SharpNeat.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeat.Neat.Reproduction.Recombination.Strategies.UniformCrossover;

public class UniformCrossoverReproductionStrategyTests
{
    [Fact]
    public void CreateGenome()
    {
        var metaNeatGenome =
            MetaNeatGenome<double>.CreateAcyclic(
                inputNodeCount: 10,
                outputNodeCount: 20,
                activationFn: new NeuralNets.ActivationFunctions.ReLU());

        var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

        int count = 100;
        NeatPopulation<double> pop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 0.1, count, RandomDefaults.CreateRandomSource());
        var generationSeq = new Int32Sequence();

        var strategy = new UniformCrossoverRecombinationStrategy<double>(
            pop.MetaNeatGenome.IsAcyclic,
            0.02,
            genomeBuilder,
            pop.GenomeIdSeq, generationSeq);

        IRandomSource rng = RandomDefaults.CreateRandomSource(0);

        var cyclicGraphCheck = new CyclicGraphCheck();

        for(int i=0; i < 1000; i++)
        {
            // Randomly select two parents from the population.
            var genome1 = pop.GenomeList[rng.Next(count)];
            var genome2 = pop.GenomeList[rng.Next(count)];

            var childGenome = strategy.CreateGenome(genome1, genome2, rng);

            // The connection genes should be sorted.
            Assert.True(SortUtils.IsSortedAscending<DirectedConnection>(childGenome.ConnectionGenes._connArr));

            // The child genome should describe an acyclic graph, i.e. the new connection should not have
            // formed a cycle in the graph.
            var digraph = childGenome.DirectedGraph;
            Assert.False(cyclicGraphCheck.IsCyclic(digraph));

            // The child genome node IDs should be a superset of those from parent1 + parent2.
            var childNodeIdSet = GetNodeIdSet(childGenome);
            var parentIdSet = GetNodeIdSet(genome1);
            parentIdSet.IntersectWith(GetNodeIdSet(genome2));

            Assert.True(childNodeIdSet.IsSupersetOf(parentIdSet));
        }
    }
}
