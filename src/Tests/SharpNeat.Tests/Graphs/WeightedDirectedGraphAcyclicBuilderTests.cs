using Redzen.Collections;
using SharpNeat.Graphs.Acyclic;
using Xunit;
using static SharpNeat.Graphs.NetworkUtils;

namespace SharpNeat.Graphs;

public class WeightedDirectedGraphAcyclicBuilderTests
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
        var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connSpan, 3, 2);

        // The graph should be unchanged from the input connections.
        CompareConnectionLists(connSpan, digraph.ConnectionIds, digraph.WeightArray);

        // Check the node count.
        Assert.Equal(5, digraph.TotalNodeCount);
    }

    [Fact]
    public void DepthNodeReorderTest()
    {
        // Define graph connections.
        var connList = new LightweightList<WeightedDirectedConnection<double>>
        {
            new(0, 4, 0.0),
            new(4, 5, 1.0),
            new(5, 2, 2.0),
            new(1, 2, 3.0),
            new(2, 3, 4.0)
        };

        // Create graph.
        var connSpan = connList.AsSpan();
        connSpan.Sort(WeightedDirectedConnectionComparer<double>.Default);
        var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connSpan, 2, 2);

        // The nodes should have IDs allocated based on depth, i.e. the layer they are in.
        // And connections should be ordered by source node ID.
        var connListExpected = new LightweightList<WeightedDirectedConnection<double>>
        {
            new(0, 2, 0.0),
            new(1, 4, 3.0),
            new(2, 3, 1.0),
            new(3, 4, 2.0),
            new(4, 5, 4.0)
        };

        // Compare actual and expected connections.
        var connSpanExpected = connListExpected.AsSpan();
        CompareConnectionLists(connSpanExpected, digraph.ConnectionIds, digraph.WeightArray);

        // Test layer info.
        LayerInfo[] layerArrExpected =
        [
            new LayerInfo(2, 2),
            new LayerInfo(3, 3),
            new LayerInfo(4, 4),
            new LayerInfo(5, 5),
            new LayerInfo(6, 5),
        ];
        Assert.Equal(5, digraph.LayerArray.Length);

        // Check the node count.
        Assert.Equal(6, digraph.TotalNodeCount);
    }
}
