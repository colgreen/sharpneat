using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using Redzen.Sorting;
using Redzen.Structures;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using static SharpNeat.Tests.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeat.Tests.Neat.Reproduction.Asexual.Strategy
{
    [TestClass]
    public class DeleteConnectionStrategyTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("AsexualReproduction")]
        public void TestDeleteConnection()
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
                Assert.AreEqual(genome.ConnectionGenes.Length - 1, childGenome.ConnectionGenes.Length);

                // The child genome's connections should be a proper subset of the parent genome's.
                var childConnSet = GetDirectedConnectionSet(childGenome);
                Assert.IsTrue(childConnSet.IsProperSubsetOf(connSet));

                // The connection genes should be sorted.
                Assert.IsTrue(SortUtils.IsSortedAscending(childGenome.ConnectionGenes._connArr));

                // Test that the array of hidden node IDs is correct, i.e. corresponds with the hidden node IDs described by the connections.
                Assert.IsTrue(ConnectionGenesUtils.ValidateHiddenNodeIds(
                    childGenome.HiddenNodeIdArray,
                    childGenome.ConnectionGenes._connArr,
                    childGenome.MetaNeatGenome.InputOutputNodeCount));
            }
        }

        #endregion
    }
}
