using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using SharpNeat.Network;
using static SharpNeatLib.Tests.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeatLib.Tests.Neat.Reproduction.Asexual.Strategy
{
    [TestClass]
    public class DeleteConnectionReproductionStrategyTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("AsexualReproduction")]
        public void TestDeleteConnection()
        {
            var pop = CreateNeatPopulation();
            var genome = pop.GenomeList[0];
            var strategy = new DeleteConnectionReproductionStrategy<double>(pop.MetaNeatGenome, pop.GenomeIdSeq, pop.GenerationSeq);
            var nodeIdSet = GetNodeIdSet(genome);
            var connSet = GetDirectedConnectionSet(genome);

            for(int i=0; i<100; i++)
            {
                var childGenome = strategy.CreateChildGenome(genome);
                
                // The child genome should have one less connection than the parent.
                Assert.AreEqual(genome.ConnectionGeneArray.Length - 1, childGenome.ConnectionGeneArray.Length);

                // The child genome's connections should be a proper subset of the parent genome's.
                var childConnSet = GetDirectedConnectionSet(childGenome);
                Assert.IsTrue(childConnSet.IsProperSubsetOf(connSet));

                // The connection genes should be sorted.
                ConnectionGeneUtils.IsSorted(childGenome.ConnectionGeneArray);
            }
        }

        #endregion
    }
}
