using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using SharpNeat.Network;
using static SharpNeatLib.Tests.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeatLib.Tests.Neat.Reproduction.Asexual.Strategy
{
    [TestClass]
    public class AddCyclicConnectionReproductionStrategyTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("AsexualReproduction")]
        public void TestAddCyclicConnection()
        {
            var pop = CreateNeatPopulation();
            var genome = pop.GenomeList[0];
            var strategy = new AddCyclicConnectionReproductionStrategy<double>(pop);
            var nodeIdSet = GetNodeIdSet(genome);
            var connSet = GetDirectedConnectionSet(genome);

            for(int i=0; i<100; i++)
            {
                var childGenome = strategy.CreateChildGenome(genome);
                
                // The child genome should have one more connection than parent.
                Assert.AreEqual(genome.ConnectionGeneArray.Length + 1, childGenome.ConnectionGeneArray.Length);

                // The child genome's new connection should not be a duplicate of any of the existing/parent connections.
                var childConnSet = GetDirectedConnectionSet(childGenome);
                var newConnList = new List<DirectedConnection>(childConnSet.Except(connSet));
                Assert.AreEqual(1, newConnList.Count);

                // The connection genes should be sorted.
                DirectedConnectionUtils.IsSorted((IList<IDirectedConnection>)childGenome.ConnectionGeneArray);

                // The child genome should have the same set of node IDs as the parent.
                var childNodeIdSet = GetNodeIdSet(childGenome);
                Assert.IsTrue(nodeIdSet.SetEquals(childNodeIdSet));
            }
        }

        #endregion
    }
}
