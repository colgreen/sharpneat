using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Network;
using SharpNeat.Network.Acyclic;
using static SharpNeatLib.Tests.Neat.Network.ConnectionGeneCompareUtils;

namespace SharpNeatLib.Tests.Neat.Network
{
    [TestClass]
    public class NeatAcyclicDirectedGraphFactoryTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("NeatAcyclicDirectedGraphFactory")]
        public void SimpleAcyclic()
        {
            // Simple acyclic graph.
            var connArr = new ConnectionGene<double>[4];
            connArr[0] = new ConnectionGene<double>(0, 0, 3, 0.0);
            connArr[1] = new ConnectionGene<double>(1, 1, 3, 1.0);
            connArr[2] = new ConnectionGene<double>(2, 2, 3, 2.0);
            connArr[3] = new ConnectionGene<double>(3, 2, 4, 3.0);

            // Create graph.
            var digraph = NeatAcyclicDirectedGraphFactory<double>.Create(connArr, 3, 2);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connArr, digraph.ConnectionIdArrays, digraph.WeightArray);

            // Check the node count.
            Assert.AreEqual(5, digraph.TotalNodeCount);
        }

        [TestMethod]
        [TestCategory("NeatAcyclicDirectedGraphFactory")]
        public void DepthNodeReorderTest()
        {
            // Define graph connections.
            var connArr = new ConnectionGene<double>[5];
            connArr[0] = new ConnectionGene<double>(0, 0, 4, 0.0);
            connArr[1] = new ConnectionGene<double>(1, 4, 5, 1.0);
            connArr[2] = new ConnectionGene<double>(2, 5, 2, 2.0);
            connArr[3] = new ConnectionGene<double>(3, 1, 2, 3.0);
            connArr[4] = new ConnectionGene<double>(4, 2, 3, 4.0);

            // Create graph.
            ConnectionGeneUtils.Sort(connArr);
            var digraph = NeatAcyclicDirectedGraphFactory<double>.Create(connArr, 2, 2);

            // The nodes should have IDs allocated based on depth, i.e. the layer they are in.
            // And connections should be ordered by source node ID.
            var connArrExpected = new ConnectionGene<double>[5];
            connArrExpected[0] = new ConnectionGene<double>(0, 0, 2, 0.0);
            connArrExpected[1] = new ConnectionGene<double>(1, 1, 4, 3.0);
            connArrExpected[2] = new ConnectionGene<double>(2, 2, 3, 1.0);
            connArrExpected[3] = new ConnectionGene<double>(3, 3, 4, 2.0);
            connArrExpected[4] = new ConnectionGene<double>(4, 4, 5, 4.0);

            // Compare actual and expected connections.
            CompareConnectionLists(connArrExpected, digraph.ConnectionIdArrays, digraph.WeightArray);

            // Test layer info.
            LayerInfo[] layerArrExpected = new LayerInfo[5];
            layerArrExpected[0] = new LayerInfo(2, 2);
            layerArrExpected[1] = new LayerInfo(3, 3);
            layerArrExpected[2] = new LayerInfo(4, 4);
            layerArrExpected[3] = new LayerInfo(5, 5);
            layerArrExpected[4] = new LayerInfo(6, 5);
            Assert.AreEqual(5, digraph.LayerArray.Length);

            // Check the node count.
            Assert.AreEqual(6, digraph.TotalNodeCount);
        }

        #endregion



    }
}
