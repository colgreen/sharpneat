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
            var connGenes = new ConnectionGenes<double>(4);
            connGenes[0] = (0, 3, 0.0, 0);
            connGenes[1] = (1, 3, 1.0, 1);
            connGenes[2] = (2, 3, 2.0, 2);
            connGenes[3] = (2, 4, 3.0, 3);

            // Create graph.
            var digraph = NeatAcyclicDirectedGraphFactory<double>.Create(connGenes, 3, 2);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connGenes, digraph.ConnectionIdArrays, digraph.WeightArray);

            // Check the node count.
            Assert.AreEqual(5, digraph.TotalNodeCount);
        }

        [TestMethod]
        [TestCategory("NeatAcyclicDirectedGraphFactory")]
        public void DepthNodeReorderTest()
        {
            // Define graph connections.
            var connGenes = new ConnectionGenes<double>(5);
            connGenes[0] = (0, 4, 0.0, 0);
            connGenes[1] = (4, 5, 1.0, 1);
            connGenes[2] = (5, 2, 2.0, 2);
            connGenes[3] = (1, 2, 3.0, 3);
            connGenes[4] = (2, 3, 4.0, 4);

            // Create graph.
            connGenes.Sort();
            var digraph = NeatAcyclicDirectedGraphFactory<double>.Create(connGenes, 2, 2);

            // The nodes should have IDs allocated based on depth, i.e. the layer they are in.
            // And connections should be ordered by source node ID.
            // TODO: Use DirectedConnection[] for expected results instead of ConnectionGenes<>.
            var connGenesExpected = new ConnectionGenes<double>(5);
            connGenesExpected[0] = (0, 2, 0.0, 0);
            connGenesExpected[1] = (1, 4, 3.0, 1);
            connGenesExpected[2] = (2, 3, 1.0, 2);
            connGenesExpected[3] = (3, 4, 2.0, 3);
            connGenesExpected[4] = (4, 5, 4.0, 4);

            // Compare actual and expected connections.
            CompareConnectionLists(connGenesExpected, digraph.ConnectionIdArrays, digraph.WeightArray);

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
