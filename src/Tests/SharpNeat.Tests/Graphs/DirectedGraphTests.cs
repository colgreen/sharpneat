using Redzen.Collections;
using Xunit;

namespace SharpNeat.Graphs.Tests;

public class DirectedGraphTests
{
    [Fact]
    public void SimpleAcyclic()
    {
        // Simple acyclic graph.
        var connList = new LightweightList<DirectedConnection>
        {
            new DirectedConnection(0, 3),
            new DirectedConnection(1, 3),
            new DirectedConnection(2, 3),
            new DirectedConnection(2, 4)
        };

        // Create graph.
        var digraph = DirectedGraphBuilder.Create(connList.AsSpan(), 0, 0);

        // The graph should be unchanged from the input connections.
        CompareConnectionLists(connList.AsSpan(), digraph.ConnectionIds);

        // Check the node count.
        Assert.Equal(5, digraph.TotalNodeCount);
    }

    [Fact]
    public void SimpleAcyclic_DefinedNodes()
    {
        // Simple acyclic graph.
        var connList = new LightweightList<DirectedConnection>
        {
            new DirectedConnection(10, 13),
            new DirectedConnection(11, 13),
            new DirectedConnection(12, 13),
            new DirectedConnection(12, 14)
        };

        // Create graph.
        var digraph = DirectedGraphBuilder.Create(connList.AsSpan(), 0, 10);

        // The graph should be unchanged from the input connections.
        CompareConnectionLists(connList.AsSpan(), digraph.ConnectionIds);

        // Check the node count.
        Assert.Equal(15, digraph.TotalNodeCount);
    }

    [Fact]
    public void SimpleAcyclic_DefinedNodes_NodeIdGap()
    {
        // Simple acyclic graph.
        var connList = new LightweightList<DirectedConnection>
        {
            new DirectedConnection(100, 103),
            new DirectedConnection(101, 103),
            new DirectedConnection(102, 103),
            new DirectedConnection(102, 104)
        };

        // Create graph.
        var digraph = DirectedGraphBuilder.Create(connList.AsSpan(), 0, 10);

        // The gaps in the node IDs should be removed such that node IDs form a contiguous span starting from zero.
        var connListExpected = new LightweightList<DirectedConnection>
        {
            new DirectedConnection(10, 13),
            new DirectedConnection(11, 13),
            new DirectedConnection(12, 13),
            new DirectedConnection(12, 14)
        };

        CompareConnectionLists(connListExpected.AsSpan(), digraph.ConnectionIds);

        // Check the node count.
        Assert.Equal(15, digraph.TotalNodeCount);
    }

    #region Private Static Methods

    private static void CompareConnectionLists(Span<DirectedConnection> x, in ConnectionIds connIds)
    {
        ReadOnlySpan<int> srcIdArr = connIds.GetSourceIdSpan();
        ReadOnlySpan<int> tgtIdArr = connIds.GetTargetIdSpan();

        Assert.Equal(x.Length, srcIdArr.Length);
        Assert.Equal(x.Length, tgtIdArr.Length);

        for(int i=0; i < x.Length; i++)
        {
            Assert.Equal(x[i].SourceId, srcIdArr[i]);
            Assert.Equal(x[i].TargetId, tgtIdArr[i]);
        }
    }

    #endregion
}
