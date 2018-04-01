using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Sorting;
using Redzen.Structures;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using SharpNeat.Network;
using static SharpNeat.Tests.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeat.Tests.Neat.Reproduction.Asexual.Strategy
{
    [TestClass]
    public class AddNodeStrategyTests
    {
        #region Public Methods

        /// <summary>
        /// Apply 'add node' mutations to the same initial genome multiple times.
        /// Note. The mutations are random, therefore this tests different mutations on each loop.
        /// </summary>
        [TestMethod]
        [TestCategory("AsexualReproduction")]
        public void TestAddNode1()
        {
            var pop = CreateNeatPopulation();
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
            var genome = pop.GenomeList[0];
            var strategy = new AddNodeStrategy<double>(
                pop.MetaNeatGenome, genomeBuilder, 
                pop.GenomeIdSeq, pop.InnovationIdSeq,
                pop.GenerationSeq, pop.AddedNodeBuffer);

            for(int i=0; i<10000; i++) {
                NeatGenome<double> childGenome = CreateAndTestChildGenome(genome, strategy);
            }
        }

        /// <summary>
        /// Apply cumulative 'add node' mutations.
        /// This explores more complex genome structures as the mutations accumulate.
        /// </summary>
        [TestMethod]
        [TestCategory("AsexualReproduction")]
        public void TestAddNode2()
        {
            var pop = CreateNeatPopulation();
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
            var genome = pop.GenomeList[0];
            var strategy = new AddNodeStrategy<double>(
                pop.MetaNeatGenome, genomeBuilder, 
                pop.GenomeIdSeq, pop.InnovationIdSeq,
                pop.GenerationSeq, pop.AddedNodeBuffer);

            for(int i=0; i<2000; i++)
            {
                NeatGenome<double> childGenome = CreateAndTestChildGenome(genome, strategy);

                // Make the child genome the parent in the next iteration. I.e. accumulate add node mutations.
                genome = childGenome;
            }
        }

        /// <summary>
        /// Apply cumulative 'add node' mutations to 10 genomes, rather than the single genome in TestAddNode2().
        /// This results in some mutations occurring that have already occurred on one of the other genomes,
        /// and therefore this tests the code paths that handle re-using innovation IDs obtain from the innovation buffers.
        /// </summary>
        [TestMethod]
        [TestCategory("AsexualReproduction")]
        public void TestAddNode3()
        {
            var pop = CreateNeatPopulation();
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
            var genome = pop.GenomeList[0];
            var strategy = new AddNodeStrategy<double>(
                pop.MetaNeatGenome, genomeBuilder, 
                pop.GenomeIdSeq, pop.InnovationIdSeq,
                pop.GenerationSeq, pop.AddedNodeBuffer);

            CircularBuffer<NeatGenome<double>> genomeRing = new CircularBuffer<NeatGenome<double>>(10);
            genomeRing.Enqueue(genome);

            for(int i=0; i<5000; i++)
            {
                NeatGenome<double> childGenome = CreateAndTestChildGenome(genome, strategy);

                // Add the new child genome to the ring.
                genomeRing.Enqueue(childGenome);

                // Take the genome at the tail of the ring for the next parent.
                genome = genomeRing[0];
            }
        }

        #endregion

        #region Private Static Methods

        private static NeatGenome<double> CreateAndTestChildGenome(NeatGenome<double> parentGenome, AddNodeStrategy<double> strategy)
        {
            var nodeIdSet = GetNodeIdSet(parentGenome);
            var connSet = GetDirectedConnectionSet(parentGenome);

            var childGenome = strategy.CreateChildGenome(parentGenome);

            // The connection genes should be sorted.
            Assert.IsTrue(SortUtils.IsSortedAscending(childGenome.ConnectionGenes._connArr));

            // The child genome should have one more connection than parent.
            Assert.AreEqual(parentGenome.ConnectionGenes.Length + 1, childGenome.ConnectionGenes.Length);

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

            return childGenome;
        }

        #endregion
    }
}
