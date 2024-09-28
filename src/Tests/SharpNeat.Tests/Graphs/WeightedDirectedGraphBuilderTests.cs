using Redzen.Collections;
using Xunit;
using static SharpNeat.Graphs.NetworkUtils;

namespace SharpNeat.Graphs;

public class WeightedDirectedGraphFactoryTests
{
    [Fact]
    public void SimpleAcyclic()
    {
        // Simple acyclic graph.
        var connList = new LightweightList<WeightedDirectedConnection<double>>
        {
            new(0, 3, 0.0),
            new(1, 3, 1.0),
            new(2, 3, 2.0),
            new(2, 4, 3.0)
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
            new(10, 13, 0.0),
            new(11, 13, 1.0),
            new(12, 13, 2.0),
            new(12, 14, 3.0)
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
            new(100, 103, 0.0),
            new(101, 103, 1.0),
            new(102, 103, 2.0),
            new(102, 104, 3.0)
        };
        var connSpan = connList.AsSpan();

        // Create graph.
        var digraph = WeightedDirectedGraphBuilder<double>.Create(connSpan, 0, 10);

        // The gaps in the node IDs should be removed such that node IDs form a contiguous span starting from zero.
        var connListExpected = new LightweightList<WeightedDirectedConnection<double>>
        {
            new(10, 13, 0.0),
            new(11, 13, 1.0),
            new(12, 13, 2.0),
            new(12, 14, 3.0)
        };
        var connSpanExpected = connListExpected.AsSpan();

        // The graph should be unchanged from the input connections.
        CompareConnectionLists(connSpanExpected, digraph.ConnectionIds, digraph.WeightArray);

        // Check the node count.
        Assert.Equal(15, digraph.TotalNodeCount);
    }
}
