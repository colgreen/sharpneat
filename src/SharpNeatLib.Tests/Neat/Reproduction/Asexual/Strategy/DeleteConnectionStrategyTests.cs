using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using SharpNeat.Network;
using SharpNeatLib.Neat.Genome;
using static SharpNeatLib.Tests.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeatLib.Tests.Neat.Reproduction.Asexual.Strategy
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
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
            var genome = pop.GenomeList[0];
            var strategy = new DeleteConnectionStrategy<double>(pop.MetaNeatGenome, genomeBuilder, pop.GenomeIdSeq, pop.GenerationSeq);
            var nodeIdSet = GetNodeIdSet(genome);
            var connSet = GetDirectedConnectionSet(genome);

            for(int i=0; i<1000; i++)
            {
                var childGenome = strategy.CreateChildGenome(genome);
                
                // The child genome should have one less connection than the parent.
                Assert.AreEqual(genome.ConnectionGenes.Length - 1, childGenome.ConnectionGenes.Length);

                // The child genome's connections should be a proper subset of the parent genome's.
                var childConnSet = GetDirectedConnectionSet(childGenome);
                Assert.IsTrue(childConnSet.IsProperSubsetOf(connSet));

                // The connection genes should be sorted.
                Assert.IsTrue(DirectedConnectionUtils.IsSorted(childGenome.ConnectionGenes._connArr));

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
