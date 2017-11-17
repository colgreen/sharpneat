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
            var connArr = new ConnectionGene<double>[4];
            connArr[0] = new ConnectionGene<double>(0, 0, 3, 0.0);
            connArr[1] = new ConnectionGene<double>(1, 1, 3, 1.0);
            connArr[2] = new ConnectionGene<double>(2, 2, 3, 2.0);
            connArr[3] = new ConnectionGene<double>(3, 2, 4, 3.0);
            
            // Create graph.
            var digraph = NeatDirectedGraphFactory<double>.Create(connArr, 0, 0);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connArr, digraph.ConnectionIdArrays, digraph.WeightArray);

            // Check the node count.
            Assert.AreEqual(5, digraph.TotalNodeCount);
        }

        [TestMethod]
        [TestCategory("NeatDirectedGraphFactory")]
        public void SimpleAcyclic_DefinedNodes()
        {
            // Simple acyclic graph.
            var connArr = new ConnectionGene<double>[4];
            connArr[0] = new ConnectionGene<double>(0, 10, 13, 0.0);
            connArr[1] = new ConnectionGene<double>(1, 11, 13, 1.0);
            connArr[2] = new ConnectionGene<double>(2, 12, 13, 2.0);
            connArr[3] = new ConnectionGene<double>(3, 12, 14, 3.0);

            // Create graph.
            var digraph = NeatDirectedGraphFactory<double>.Create(connArr, 0, 10);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connArr, digraph.ConnectionIdArrays, digraph.WeightArray);

            // Check the node count.
            Assert.AreEqual(15, digraph.TotalNodeCount);
        }

        [TestMethod]
        [TestCategory("NeatDirectedGraphFactory")]
        public void SimpleAcyclic_DefinedNodes_NodeIdGap()
        {
            // Simple acyclic graph.
            var connArr = new ConnectionGene<double>[4];
            connArr[0] = new ConnectionGene<double>(0, 100, 103, 0.0);
            connArr[1] = new ConnectionGene<double>(1, 101, 103, 1.0);
            connArr[2] = new ConnectionGene<double>(2, 102, 103, 2.0);
            connArr[3] = new ConnectionGene<double>(3, 102, 104, 3.0);

            // Create graph.
            var digraph = NeatDirectedGraphFactory<double>.Create(connArr, 0, 10);

            // The gaps in the node IDs should be removed such that node IDs form a contiguous span starting from zero.
            var connArrExpected = new ConnectionGene<double>[4];
            connArrExpected[0] = new ConnectionGene<double>(0, 10, 13, 0.0);
            connArrExpected[1] = new ConnectionGene<double>(1, 11, 13, 1.0);
            connArrExpected[2] = new ConnectionGene<double>(2, 12, 13, 2.0);
            connArrExpected[3] = new ConnectionGene<double>(3, 12, 14, 3.0);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connArrExpected, digraph.ConnectionIdArrays, digraph.WeightArray);

            // Check the node count.
            Assert.AreEqual(15, digraph.TotalNodeCount);
        }

        #endregion
    }
}
