using System.Collections.Generic;
using System.Linq;
using Redzen.Random;
using Redzen.Sorting;
using Redzen.Structures;
using SharpNeat.Neat.Genome;
using SharpNeat.Graphs;
using SharpNeat.Graphs.Acyclic;
using SharpNeat.Graphs.Tests;
using Xunit;
using static SharpNeat.Neat.Genome.Tests.NestGenomeTestUtils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy.Tests
{
    public class AddAcyclicConnectionStrategyTests
    {
        #region Test Methods

        [Fact]
        public void AddAcyclicConnection()
        {
            var pop = CreateNeatPopulation();
            var generationSeq = new Int32Sequence();
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
            var genome = pop.GenomeList[0];

            var strategy = new AddAcyclicConnectionStrategy<double>(
                pop.MetaNeatGenome, genomeBuilder,
                pop.GenomeIdSeq, generationSeq);

            IRandomSource rng = RandomDefaults.CreateRandomSource();

            var nodeIdSet = GetNodeIdSet(genome);
            var connSet = GetDirectedConnectionSet(genome);

            CyclicGraphAnalysis cyclicGraphAnalysis = new CyclicGraphAnalysis();

            for(int i=0; i < 1000;)
            {
                var childGenome = strategy.CreateChildGenome(genome, rng);
                
                // Note. the strategy will return a null if it cannot find an acyclic connection to add;
                // test for this and try again. The test will be for N successful mutations rather than N attempts.
                if(childGenome is null) {
                    continue;
                }

                // The child genome should have one more connection than parent.
                Assert.Equal(genome.ConnectionGenes.Length + 1, childGenome.ConnectionGenes.Length);

                // The child genome's new connection should not be a duplicate of any of the existing/parent connections.
                var childConnSet = GetDirectedConnectionSet(childGenome);
                var newConnList = new List<DirectedConnection>(childConnSet.Except(connSet));
                Assert.Single(newConnList);

                // The connection genes should be sorted.
                Assert.True(SortUtils.IsSortedAscending(childGenome.ConnectionGenes._connArr));

                // The child genome should have the same set of node IDs as the parent.
                var childNodeIdSet = GetNodeIdSet(childGenome);
                Assert.True(nodeIdSet.SetEquals(childNodeIdSet));

                // The child genome should describe an acyclic graph, i.e. the new connection should not have
                // formed a cycle in the graph.
                var digraph = childGenome.DirectedGraph;
                Assert.False(cyclicGraphAnalysis.IsCyclic(digraph));

                // Increment for successful tests only.
                i++;
            }
        }

        [Fact]
        public void AddAcyclicConnection_CumulativeAdditions()
        {
            var pop = CreateNeatPopulation();
            var generationSeq = new Int32Sequence();
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(pop.MetaNeatGenome);
            var rootGenome = pop.GenomeList[0];

            var strategy = new AddAcyclicConnectionStrategy<double>(
                pop.MetaNeatGenome, genomeBuilder,
                pop.GenomeIdSeq, generationSeq);

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
                    var childGenome = strategy.CreateChildGenome(parentGenome, rng);
                
                    // Note. the strategy will return a null if it cannot find an acyclic connection to add;
                    // test for this and try again. The test will be for N successful mutations rather than N attempts.
                    if(childGenome is null) {
                        continue;
                    }

                    // The child genome should have one more connection than parent.
                    Assert.Equal(parentGenome.ConnectionGenes.Length + 1, childGenome.ConnectionGenes.Length);

                    // The child genome's new connection should not be a duplicate of any of the existing/parent connections.
                    var connSet = GetDirectedConnectionSet(parentGenome);
                    var childConnSet = GetDirectedConnectionSet(childGenome);
                    var newConnList = new List<DirectedConnection>(childConnSet.Except(connSet));
                    Assert.Single(newConnList);

                    // The connection genes should be sorted.
                    Assert.True(SortUtils.IsSortedAscending(childGenome.ConnectionGenes._connArr));

                    // The child genome should have the same set of node IDs as the parent.
                    var childNodeIdSet = GetNodeIdSet(childGenome);
                    Assert.True(nodeIdSet.SetEquals(childNodeIdSet));

                    // The child genome should describe an acyclic graph, i.e. the new connection should not have
                    // formed a cycle in the graph.
                    var digraph = childGenome.DirectedGraph;
                    Assert.False(cyclicGraphAnalysis.IsCyclic(digraph));

                    // Run the acyclic graph depth analysis algorithm.
                    GraphDepthInfo depthInfo = graphDepthAnalysis.CalculateNodeDepths(childGenome.DirectedGraph);

                    // Run again with the alternative algorithm (that uses function recursion).
                    GraphDepthInfo depthInfo2 = AcyclicGraphDepthAnalysisByRecursion.CalculateNodeDepths(childGenome.DirectedGraph);

                    Assert.Equal(nodeIdSet.Count, depthInfo._nodeDepthArr.Length);
                    Assert.Equal(nodeIdSet.Count, depthInfo2._nodeDepthArr.Length);
                    Assert.Equal(depthInfo2._nodeDepthArr, depthInfo._nodeDepthArr);

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
