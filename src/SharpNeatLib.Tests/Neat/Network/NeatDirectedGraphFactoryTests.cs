using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Network;
using static SharpNeatLib.Tests.Neat.Network.ConnectionGeneCompareUtils;

namespace SharpNeatLib.Tests.Neat.Network
{
    [TestClass]
    public class NeatDirectedGraphFactoryTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("NeatDirectedGraphFactory")]
        public void SimpleAcyclic()
        {
            // Simple acyclic graph.
            var connGenes = new ConnectionGenes<double>(4);
            connGenes[0] = (0, 3, 0.0, 0);
            connGenes[1] = (1, 3, 1.0, 1);
            connGenes[2] = (2, 3, 2.0, 2);
            connGenes[3] = (2, 4, 3.0, 3);
            
            // Create graph.
            var digraph = NeatDirectedGraphFactory<double>.Create(connGenes, 0, 0);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connGenes, digraph.ConnectionIdArrays, digraph.WeightArray);

            // Check the node count.
            Assert.AreEqual(5, digraph.TotalNodeCount);
        }

        [TestMethod]
        [TestCategory("NeatDirectedGraphFactory")]
        public void SimpleAcyclic_DefinedNodes()
        {
            // Simple acyclic graph.
            var connGenes = new ConnectionGenes<double>(4);
            connGenes[0] = (10, 13, 0.0, 0);
            connGenes[1] = (11, 13, 1.0, 1);
            connGenes[2] = (12, 13, 2.0, 2);
            connGenes[3] = (12, 14, 3.0, 3);

            // Create graph.
            var digraph = NeatDirectedGraphFactory<double>.Create(connGenes, 0, 10);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connGenes, digraph.ConnectionIdArrays, digraph.WeightArray);

            // Check the node count.
            Assert.AreEqual(15, digraph.TotalNodeCount);
        }

        [TestMethod]
        [TestCategory("NeatDirectedGraphFactory")]
        public void SimpleAcyclic_DefinedNodes_NodeIdGap()
        {
            // Simple acyclic graph.
            var connGenes = new ConnectionGenes<double>(4);
            connGenes[0] = (100, 103, 0.0, 0);
            connGenes[1] = (101, 103, 1.0, 1);
            connGenes[2] = (102, 103, 2.0, 2);
            connGenes[3] = (102, 104, 3.0, 3);

            // Create graph.
            var digraph = NeatDirectedGraphFactory<double>.Create(connGenes, 0, 10);

            // The gaps in the node IDs should be removed such that node IDs form a contiguous span starting from zero.
            var connGenesExpected = new ConnectionGenes<double>(4);
            connGenesExpected[0] = (10, 13, 0.0, 0);
            connGenesExpected[1] = (11, 13, 1.0, 1);
            connGenesExpected[2] = (12, 13, 2.0, 2);
            connGenesExpected[3] = (12, 14, 3.0, 3);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connGenesExpected, digraph.ConnectionIdArrays, digraph.WeightArray);

            // Check the node count.
            Assert.AreEqual(15, digraph.TotalNodeCount);
        }

        #endregion
    }
}
