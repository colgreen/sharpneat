using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Network;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;
using SharpNeat.NeuralNet.Double.ActivationFunctions;
using SharpNeatLib.Neat.Genome;
using static SharpNeatLib.Tests.Neat.Network.ConnectionCompareUtils;

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
            var metaNeatGenome = new MetaNeatGenome<double>(3, 2, true, new ReLU());
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

            // Simple acyclic graph.
            var connGenes = new ConnectionGenes<double>(4);
            connGenes[0] = (0, 3, 0.0);
            connGenes[1] = (1, 3, 1.0);
            connGenes[2] = (2, 3, 2.0);
            connGenes[3] = (2, 4, 3.0);

            // Wrap in a genome.
            var genome = genomeBuilder.Create(0, 0, connGenes);

            // Create graph.
            var digraph = NeatAcyclicDirectedGraphFactory<double>.Create(genome);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connGenes, digraph.ConnectionIdArrays, digraph.WeightArray);

            // Check the node count.
            Assert.AreEqual(5, digraph.TotalNodeCount);
        }

        [TestMethod]
        [TestCategory("NeatAcyclicDirectedGraphFactory")]
        public void DepthNodeReorderTest()
        {
            var metaNeatGenome = new MetaNeatGenome<double>(2, 2, true, new ReLU());
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

            // Define graph connections.
            var connGenes = new ConnectionGenes<double>(5);
            connGenes[0] = (0, 4, 0.0);
            connGenes[1] = (4, 5, 1.0);
            connGenes[2] = (5, 2, 2.0);
            connGenes[3] = (1, 2, 3.0);
            connGenes[4] = (2, 3, 4.0);
            connGenes.Sort();

            // Wrap in a genome.
            var genome = genomeBuilder.Create(0, 0, connGenes);

            // Create graph.
            var digraph = NeatAcyclicDirectedGraphFactory<double>.Create(genome);

            // The nodes should have IDs allocated based on depth, i.e. the layer they are in.
            // And connections should be ordered by source node ID.
            var connArrExpected = new DirectedConnection[5];
            var weightArrExpected = new double[5];
            connArrExpected[0] = new DirectedConnection(0, 2); weightArrExpected[0] = 0.0;
            connArrExpected[1] = new DirectedConnection(1, 4); weightArrExpected[1] = 3.0;
            connArrExpected[2] = new DirectedConnection(2, 3); weightArrExpected[2] = 1.0;
            connArrExpected[3] = new DirectedConnection(3, 4); weightArrExpected[3] = 2.0;
            connArrExpected[4] = new DirectedConnection(4, 5); weightArrExpected[4] = 4.0;

            // Compare actual and expected connections.
            CompareConnectionLists(connArrExpected, weightArrExpected, digraph.ConnectionIdArrays, digraph.WeightArray);

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
