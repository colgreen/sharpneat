using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Network;
using SharpNeat.Network;
using SharpNeat.NeuralNets.Double.ActivationFunctions;
using static SharpNeatLib.Tests.Neat.Network.ConnectionCompareUtils;

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
            connGenes[0] = (0, 3, 0.0);
            connGenes[1] = (1, 3, 1.0);
            connGenes[2] = (2, 3, 2.0);
            connGenes[3] = (2, 4, 3.0);

            // Wrap in a genome.
            var genome = NeatGenomeFactory<double>.Create(
                new MetaNeatGenome<double>(0, 0, false, new ReLU()),
                0, 0, connGenes);
            
            // Create graph.
            var digraph = NeatDirectedGraphFactory<double>.Create(genome);

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
            connGenes[0] = (10, 13, 0.0);
            connGenes[1] = (11, 13, 1.0);
            connGenes[2] = (12, 13, 2.0);
            connGenes[3] = (12, 14, 3.0);

            // Wrap in a genome.
            var genome = NeatGenomeFactory<double>.Create(
                new MetaNeatGenome<double>(0, 10, false, new ReLU()),
                0, 0, connGenes);

            // Create graph.
            var digraph = NeatDirectedGraphFactory<double>.Create(genome);

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
            connGenes[0] = (100, 103, 0.0);
            connGenes[1] = (101, 103, 1.0);
            connGenes[2] = (102, 103, 2.0);
            connGenes[3] = (102, 104, 3.0);

            // Wrap in a genome.
            var genome = NeatGenomeFactory<double>.Create(
                new MetaNeatGenome<double>(0, 10, false, new ReLU()),
                0, 0, connGenes);

            // Create graph.
            var digraph = NeatDirectedGraphFactory<double>.Create(genome);

            // The gaps in the node IDs should be removed such that node IDs form a contiguous span starting from zero.
            var connArrExpected = new DirectedConnection[4];
            var weightArrExpected = new double[4];
            connArrExpected[0] = new DirectedConnection(10, 13); weightArrExpected[0] = 0.0;
            connArrExpected[1] = new DirectedConnection(11, 13); weightArrExpected[1] = 1.0;
            connArrExpected[2] = new DirectedConnection(12, 13); weightArrExpected[2] = 2.0;
            connArrExpected[3] = new DirectedConnection(12, 14); weightArrExpected[3] = 3.0;

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connArrExpected, weightArrExpected, digraph.ConnectionIdArrays, digraph.WeightArray);
            
            // Check the node count.
            Assert.AreEqual(15, digraph.TotalNodeCount);
        }

        #endregion
    }
}
