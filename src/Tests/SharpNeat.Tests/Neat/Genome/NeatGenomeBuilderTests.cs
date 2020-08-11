using SharpNeat.Graphs;
using SharpNeat.NeuralNets.Double.ActivationFunctions;
using Xunit;
using static SharpNeat.Graphs.Tests.ConnectionCompareUtils;

namespace SharpNeat.Neat.Genome.Tests
{
    public class NeatGenomeBuilderTests
    {
        #region Test Methods

        [Fact]
        public void Simple()
        {
            var metaNeatGenome = new MetaNeatGenome<double>(0, 0, false, new ReLU());
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

            // Simple acyclic graph.
            var connGenes = new ConnectionGenes<double>(4);
            connGenes[0] = (0, 3, 0.0);
            connGenes[1] = (1, 3, 1.0);
            connGenes[2] = (2, 3, 2.0);
            connGenes[3] = (2, 4, 3.0);

            // Wrap in a genome.
            var genome = genomeBuilder.Create(0, 0, connGenes);

            // Note. The genome builder creates a digraph representation of the genome and attaches/caches it on the genome object.
            var digraph = genome.DirectedGraph;

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connGenes, digraph.ConnectionIdArrays);

            // Check the node count.
            Assert.Equal(5, digraph.TotalNodeCount);
        }

        [Fact]
        public void Simple_DefinedNodes()
        {
            var metaNeatGenome = new MetaNeatGenome<double>(0, 10, false, new ReLU());
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

            // Simple acyclic graph.
            var connGenes = new ConnectionGenes<double>(4);
            connGenes[0] = (10, 13, 0.0);
            connGenes[1] = (11, 13, 1.0);
            connGenes[2] = (12, 13, 2.0);
            connGenes[3] = (12, 14, 3.0);

            // Wrap in a genome.
            var genome = genomeBuilder.Create(0, 0, connGenes);

            // Note. The genome builder creates a digraph representation of the genome and attaches/caches it on the genome object.
            var digraph = genome.DirectedGraph;

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connGenes, digraph.ConnectionIdArrays);

            // Check the node count.
            Assert.Equal(15, digraph.TotalNodeCount);
        }

        [Fact]
        public void Simple_DefinedNodes_NodeIdGap()
        {
            var metaNeatGenome = new MetaNeatGenome<double>(0, 10, false, new ReLU());
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

            // Simple acyclic graph.
            var connGenes = new ConnectionGenes<double>(4);
            connGenes[0] = (100, 103, 0.0);
            connGenes[1] = (101, 103, 1.0);
            connGenes[2] = (102, 103, 2.0);
            connGenes[3] = (102, 104, 3.0);

            // Wrap in a genome.
            var genome = genomeBuilder.Create(0, 0, connGenes);

            // Note. The genome builder creates a digraph representation of the genome and attaches/caches it on the genome object.
            var digraph = genome.DirectedGraph;

            // The gaps in the node IDs should be removed such that node IDs form a contiguous span starting from zero.
            var connArrExpected = new DirectedConnection[4];
            var weightArrExpected = new double[4];
            connArrExpected[0] = new DirectedConnection(10, 13); weightArrExpected[0] = 0.0;
            connArrExpected[1] = new DirectedConnection(11, 13); weightArrExpected[1] = 1.0;
            connArrExpected[2] = new DirectedConnection(12, 13); weightArrExpected[2] = 2.0;
            connArrExpected[3] = new DirectedConnection(12, 14); weightArrExpected[3] = 3.0;

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connArrExpected, digraph.ConnectionIdArrays);
            
            // Check the node count.
            Assert.Equal(15, digraph.TotalNodeCount);
        }

        #endregion
    }
}
