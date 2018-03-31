using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using SharpNeat.Network;
using static SharpNeatLib.Tests.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeatLib.Tests.Neat.Reproduction.Asexual.Strategy
{
    [TestClass]
    public class AddCyclicConnectionStrategyTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("AsexualReproduction")]
        public void TestAddCyclicConnection()
        {
            var pop = CreateNeatPopulation();
            var genomeFactory = new NeatGenomeFactory<double>();
            var genome = pop.GenomeList[0];
            var strategy = new AddCyclicConnectionStrategy<double>(pop.MetaNeatGenome, genomeFactory, pop.GenomeIdSeq, pop.InnovationIdSeq, pop.GenerationSeq);
            var nodeIdSet = GetNodeIdSet(genome);
            var connSet = GetDirectedConnectionSet(genome);

            for(int i=0; i<1000; i++)
            {
                var childGenome = strategy.CreateChildGenome(genome);
                
                // The child genome should have one more connection than parent.
                Assert.AreEqual(genome.ConnectionGenes.Length + 1, childGenome.ConnectionGenes.Length);

                // The child genome's new connection should not be a duplicate of any of the existing/parent connections.
                var childConnSet = GetDirectedConnectionSet(childGenome);
                var newConnList = new List<DirectedConnection>(childConnSet.Except(connSet));
                Assert.AreEqual(1, newConnList.Count);

                // The connection genes should be sorted.
                Assert.IsTrue(DirectedConnectionUtils.IsSorted(childGenome.ConnectionGenes._connArr));

                // The child genome should have the same set of node IDs as the parent.
                var childNodeIdSet = GetNodeIdSet(childGenome);
                Assert.IsTrue(nodeIdSet.SetEquals(childNodeIdSet));
            }
        }

        #endregion
    }
}
