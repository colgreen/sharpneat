using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using Redzen.Sorting;
using Redzen.Structures;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;
using SharpNeat.Tests.Network;
using static SharpNeat.Tests.Neat.Genome.NestGenomeTestUtils;

namespace SharpNeat.Tests.Neat.Reproduction.Asexual.Strategy
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
            var generationSeq = new Int32Sequence();
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
            var genome = pop.GenomeList[0];

            var strategy = new AddAcyclicConnectionStrategy<double>(
                pop.MetaNeatGenome, genomeBuilder,
                pop.GenomeIdSeq, pop.InnovationIdSeq, generationSeq);

            IRandomSource rng = RandomDefaults.CreateRandomSource();

            var nodeIdSet = GetNodeIdSet(genome);
            var connSet = GetDirectedConnectionSet(genome);

            CyclicGraphAnalysis cyclicGraphAnalysis = new CyclicGraphAnalysis();

            for(int i=0; i < 1000;)
            {
                var childGenome = strategy.CreateChildGenome(genome, rng);
                
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
                Assert.IsTrue(SortUtils.IsSortedAscending(childGenome.ConnectionGenes._connArr));

                // The child genome should have the same set of node IDs as the parent.
                var childNodeIdSet = GetNodeIdSet(childGenome);
                Assert.IsTrue(nodeIdSet.SetEquals(childNodeIdSet));

                // The child genome should describe an acyclic graph, i.e. the new connection should not have
                // formed a cycle in the graph.
                var digraph = childGenome.DirectedGraph;
                Assert.IsFalse(cyclicGraphAnalysis.IsCyclic(digraph));

                // Increment for successful tests only.
                i++;
            }
        }

        [TestMethod]
        [TestCategory("AsexualReproduction")]
        public void TestAddAcyclicConnection_CumulativeAdditions()
        {
            var pop = CreateNeatPopulation();
            var generationSeq = new Int32Sequence();
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
            var rootGenome = pop.GenomeList[0];

            var strategy = new AddAcyclicConnectionStrategy<double>(
                pop.MetaNeatGenome, genomeBuilder,
                pop.GenomeIdSeq, pop.InnovationIdSeq, generationSeq);

            IRandomSource rng = RandomDefaults.CreateRandomSource();

            var nodeIdSet = GetNodeIdSet(rootGenome);

            CyclicGraphAnalysis cyclicGraphAnalysis = new CyclicGraphAnalysis();

            AcyclicGraphDepthAnalysis graphDepthAnalysis = new AcyclicGraphDepthAnalysis();

            // Run the inner loop test multiple times.
            // Note. The add-connection mutations are random, thus each loop accumulates a different set of mutations.
            for(int i=0; i < 50; i++)
            {
                var parentGenome = rootGenome;

                // Accumulate random mutations for some number of loops.
                for(int j=0; j < 20;)
                {
                    var childGenome = strategy.CreateChildGenome(rootGenome, rng);
                
                    // Note. the strategy will return a null if it cannot find an acyclic connection to add;
                    // test for this and try again. The test will be for N successful mutations rather than N attempts.
                    if(null == childGenome) {
                        continue;
                    }

                    // The child genome should have one more connection than parent.
                    Assert.AreEqual(rootGenome.ConnectionGenes.Length + 1, childGenome.ConnectionGenes.Length);

                    // The child genome's new connection should not be a duplicate of any of the existing/parent connections.
                    var connSet = GetDirectedConnectionSet(rootGenome);
                    var childConnSet = GetDirectedConnectionSet(childGenome);
                    var newConnList = new List<DirectedConnection>(childConnSet.Except(connSet));
                    Assert.AreEqual(1, newConnList.Count);

                    // The connection genes should be sorted.
                    Assert.IsTrue(SortUtils.IsSortedAscending(childGenome.ConnectionGenes._connArr));

                    // The child genome should have the same set of node IDs as the parent.
                    var childNodeIdSet = GetNodeIdSet(childGenome);
                    Assert.IsTrue(nodeIdSet.SetEquals(childNodeIdSet));

                    // The child genome should describe an acyclic graph, i.e. the new connection should not have
                    // formed a cycle in the graph.
                    var digraph = childGenome.DirectedGraph;
                    Assert.IsFalse(cyclicGraphAnalysis.IsCyclic(digraph));

                    // Run the acyclic graph depth analysis algorithm.
                    GraphDepthInfo depthInfo = graphDepthAnalysis.CalculateNodeDepths(childGenome.DirectedGraph);

                    // Run again with the alternative algorithm (that uses function recursion).
                    GraphDepthInfo depthInfo2 = AcyclicGraphDepthAnalysisByRecursion.CalculateNodeDepths(childGenome.DirectedGraph);

                    Assert.AreEqual(nodeIdSet.Count, depthInfo._nodeDepthArr.Length);
                    Assert.AreEqual(nodeIdSet.Count, depthInfo2._nodeDepthArr.Length);
                    ArrayTestUtils.Compare(depthInfo2._nodeDepthArr, depthInfo._nodeDepthArr);

                    // Set the child genome to be the new parent, thus we accumulate random new connections over time.
                    parentGenome = childGenome;

                    // Increment for successful tests only.
                    j++;
                }
            }
        }

        #endregion
    }
}
