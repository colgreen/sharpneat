using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using Redzen.Sorting;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;
using SharpNeat.Network;
using static SharpNeat.Tests.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeat.Tests.Neat.Reproduction.Sexual.Strategy.UniformCrossover
{
    [TestClass]
    public class UniformCrossoverReproductionStrategyTests
    {
        [TestMethod]
        [TestCategory("SexualReproduction")]
        public void TestCreateGenome()
        {
            var metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 10,
                outputNodeCount: 20,
                isAcyclic: true,
                activationFn: new SharpNeat.NeuralNet.Double.ActivationFunctions.ReLU());

            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

            int count = 100;
            NeatPopulation<double> pop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 0.1, count, RandomDefaults.CreateRandomSource());

            var strategy = new UniformCrossoverReproductionStrategy<double>(
                pop.MetaNeatGenome, genomeBuilder,
                pop.GenomeIdSeq, pop.GenerationSeq,
                RandomDefaults.CreateRandomSource());

            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            for(int i=0; i<1000; i++)
            {
                // Randomly select two parents from the population.
                var genome1 = pop.GenomeList[rng.Next(count)];
                var genome2 = pop.GenomeList[rng.Next(count)];

                var childGenome = strategy.CreateGenome(genome1, genome2);

                // The connection genes should be sorted.
                Assert.IsTrue(SortUtils.IsSortedAscending(childGenome.ConnectionGenes._connArr));

                // The child genome should describe an acyclic graph, i.e. the new connection should not have
                // formed a cycle in the graph.
                var digraph = childGenome.DirectedGraph;
                Assert.IsFalse(CyclicGraphAnalysis.IsCyclicStatic(digraph));

                // The child genome node IDs should be a superset of those from parent1 + parent2.
                var childNodeIdSet = GetNodeIdSet(childGenome);
                var parentIdSet = GetNodeIdSet(genome1);
                parentIdSet.IntersectWith(GetNodeIdSet(genome2));

                Assert.IsTrue(childNodeIdSet.IsSupersetOf(parentIdSet));                
            }
        }
    }
}
