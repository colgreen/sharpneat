using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Network;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using SharpNeat.Network;
using static SharpNeatLib.Tests.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeatLib.Tests.Neat.Reproduction.Asexual.Strategy
{
    [TestClass]
    public class AddAcyclicConnectionStrategyTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("AsexualReproduction")]
        public void TestAddAcyclicConnection()
        {
            var pop = CreateNeatPopulation();
            var genome = pop.GenomeList[0];
            var strategy = new AddAcyclicConnectionStrategy<double>(pop.MetaNeatGenome, pop.GenomeIdSeq, pop.InnovationIdSeq, pop.GenerationSeq);
            var nodeIdSet = GetNodeIdSet(genome);
            var connSet = GetDirectedConnectionSet(genome);

            for(int i=0; i<1000;)
            {
                var childGenome = strategy.CreateChildGenome(genome);
                
                // Note. the strategy will return a null if it cannot find an acyclic connection to add;
                // test for this and try again. The test will be for N successful mutations rather than N attempts.
                if(null == childGenome) {
                    continue;
                }

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

                // The child genome should describe an acyclic graph, i.e. the new connection should not have
                // formed a cycle in the graph.
                var digraph = NeatDirectedGraphFactory<double>.Create(childGenome);
                Assert.IsFalse(CyclicGraphAnalysis.IsCyclicStatic(digraph));

                // Increment for successful tests only.
                i++;
            }
        }

        #endregion
    }
}
