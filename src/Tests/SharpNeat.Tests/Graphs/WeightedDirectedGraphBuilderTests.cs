using Redzen.Collections;
using SharpNeat.Graphs;
using Xunit;
using static SharpNeat.Graphs.Tests.NetworkUtils;

namespace SharpNeat.Tests
{
    public class WeightedDirectedGraphFactoryTests
    {
        #region Test Methods

        [Fact]
        public void SimpleAcyclic()
        {
            // Simple acyclic graph.
            var connList = new LightweightList<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 3, 0.0),
                new WeightedDirectedConnection<double>(1, 3, 1.0),
                new WeightedDirectedConnection<double>(2, 3, 2.0),
                new WeightedDirectedConnection<double>(2, 4, 3.0)
            };
            var connSpan = connList.AsSpan();

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connSpan, 0, 0);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connSpan, digraph.ConnectionIds, digraph.WeightArray);

            // Check the node count.
            Assert.Equal(5, digraph.TotalNodeCount);
        }

        [Fact]
        public void SimpleAcyclic_DefinedNodes()
        {
            // Simple acyclic graph.
            var connList = new LightweightList<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(10, 13, 0.0),
                new WeightedDirectedConnection<double>(11, 13, 1.0),
                new WeightedDirectedConnection<double>(12, 13, 2.0),
                new WeightedDirectedConnection<double>(12, 14, 3.0)
            };
            var connSpan = connList.AsSpan();

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connSpan, 0, 10);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connSpan, digraph.ConnectionIds, digraph.WeightArray);

            // Check the node count.
            Assert.Equal(15, digraph.TotalNodeCount);
        }

        [Fact]
        public void SimpleAcyclic_DefinedNodes_NodeIdGap()
        {
            // Simple acyclic graph.
            var connList = new LightweightList<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(100, 103, 0.0),
                new WeightedDirectedConnection<double>(101, 103, 1.0),
                new WeightedDirectedConnection<double>(102, 103, 2.0),
                new WeightedDirectedConnection<double>(102, 104, 3.0)
            };
            var connSpan = connList.AsSpan();

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connSpan, 0, 10);

            // The gaps in the node IDs should be removed such that node IDs form a contiguous span starting from zero.
            var connListExpected = new LightweightList<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(10, 13, 0.0),
                new WeightedDirectedConnection<double>(11, 13, 1.0),
                new WeightedDirectedConnection<double>(12, 13, 2.0),
                new WeightedDirectedConnection<double>(12, 14, 3.0)
            };
            var connSpanExpected = connListExpected.AsSpan();

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connSpanExpected, digraph.ConnectionIds, digraph.WeightArray);

            // Check the node count.
            Assert.Equal(15, digraph.TotalNodeCount);
        }

        #endregion
    }
}
