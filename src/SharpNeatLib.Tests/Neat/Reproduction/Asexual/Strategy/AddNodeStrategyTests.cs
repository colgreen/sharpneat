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
    public class AddNodeStrategyTests
    {
        [TestMethod]
        [TestCategory("AsexualReproduction")]
        public void TestAddNode()
        {
            var pop = CreateNeatPopulation();
            var genome = pop.GenomeList[0];
            var strategy = new AddNodeStrategy<double>(pop.MetaNeatGenome, pop.GenomeIdSeq, pop.InnovationIdSeq, pop.GenerationSeq, pop.AddedNodeBuffer, pop.AddedConnectionBuffer);

            for(int i=0; i<1000; i++)
            {
                var nodeIdSet = GetNodeIdSet(genome);
                var connSet = GetDirectedConnectionSet(genome);

                var childGenome = strategy.CreateChildGenome(genome);

                // The connection genes should be sorted.
                Assert.IsTrue(DirectedConnectionUtils.IsSorted(childGenome.ConnectionGenes._connArr));                

                // ConnectionIndexArray should describe the genes in innovation ID sort order.
                Assert.IsTrue(ConnectionGenesUtils.IsSorted(childGenome.ConnectionIndexArray, childGenome.ConnectionGenes._idArr));

                // The child genome should have one more connection than parent.
                Assert.AreEqual(genome.ConnectionGenes.Length + 1, childGenome.ConnectionGenes.Length);

                // The child genome should have one more node ID than the parent.
                var childNodeIdSet = GetNodeIdSet(childGenome);
                var newNodeIdList = new List<int>(childNodeIdSet.Except(nodeIdSet));
                Assert.AreEqual(1, newNodeIdList.Count);
                int newNodeId = newNodeIdList[0];

                // The child genome's new connections should not be a duplicate of any of the existing/parent connections.
                var childConnSet = GetDirectedConnectionSet(childGenome);
                var newConnList = new List<DirectedConnection>(childConnSet.Except(connSet));
                Assert.AreEqual(2, newConnList.Count);

                // The parent should have one connection that the child does not, i.e. the connection that was replaced.
                var removedConnList = new List<DirectedConnection>(connSet.Except(childConnSet));
                Assert.AreEqual(1, removedConnList.Count);

                // The two new connections should connect to the new node ID.
                var connRemoved = removedConnList[0];
                var connA = newConnList[0];
                var connB = newConnList[1];
                Assert.IsTrue(
                        (connA.SourceId == connRemoved.SourceId && connA.TargetId == newNodeId && connB.SourceId == newNodeId && connB.TargetId == connRemoved.TargetId) 
                    ||  (connB.SourceId == connRemoved.SourceId && connB.TargetId == newNodeId && connA.SourceId == newNodeId && connA.TargetId == connRemoved.TargetId));

                // Make the child genome the parent in the next iteration. I.e. accumulate add node mutations.
                genome = childGenome;
            }
        }
    }
}
