using Redzen.Collections;
using Xunit;

namespace SharpNeat.Graphs.Tests;

public class CyclicGraphCheckTests
{
    #region Test Methods [Acyclic]

    [Fact]
    public void SimpleAcyclic()
    {
        // Simple acyclic graph.
        var connList = new LightweightList<DirectedConnection>
        {
            new DirectedConnection(0, 3),
            new DirectedConnection(1, 3),
            new DirectedConnection(2, 3),
            new DirectedConnection(2, 4),
            new DirectedConnection(4, 1)
        };

        // Create graph.
        var connSpan = connList.AsSpan();
        connSpan.Sort();
        var digraph = DirectedGraphBuilder.Create(connSpan, 0, 0);

        // Test if cyclic.
        var cyclicGraphCheck = new CyclicGraphCheck();
        bool isCyclic = cyclicGraphCheck.IsCyclic(digraph);
        Assert.False(isCyclic);
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
            new DirectedConnection(12, 14),
            new DirectedConnection(14, 11)
        };

        // Create graph.
        var connSpan = connList.AsSpan();
        connSpan.Sort();
        var digraph = DirectedGraphBuilder.Create(connSpan, 0, 10);

        // Test if cyclic.
        var cyclicGraphCheck = new CyclicGraphCheck();
        bool isCyclic = cyclicGraphCheck.IsCyclic(digraph);
        Assert.False(isCyclic);
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
            new DirectedConnection(102, 104),
            new DirectedConnection(104, 101)
        };

        // Create graph.
        var connSpan = connList.AsSpan();
        connSpan.Sort();
        var digraph = DirectedGraphBuilder.Create(connSpan, 0, 10);

        // Test if cyclic.
        var cyclicGraphCheck = new CyclicGraphCheck();
        bool isCyclic = cyclicGraphCheck.IsCyclic(digraph);
        Assert.False(isCyclic);
    }

    [Fact]
    public void Regression1()
    {
        // Simple acyclic graph.
        var connList = new LightweightList<DirectedConnection>
        {
            new DirectedConnection(0, 2),
            new DirectedConnection(0, 3),
            new DirectedConnection(0, 4),
            new DirectedConnection(2, 5),
            new DirectedConnection(3, 6),
            new DirectedConnection(4, 7),
            new DirectedConnection(5, 8),
            new DirectedConnection(6, 4),
            new DirectedConnection(6, 9),
            new DirectedConnection(7, 10),
            new DirectedConnection(8, 1),
            new DirectedConnection(9, 1),
            new DirectedConnection(10, 1)
        };

        // Create graph.
        var connSpan = connList.AsSpan();
        connSpan.Sort();
        var digraph = DirectedGraphBuilder.Create(connSpan, 0, 0);

        // Test if cyclic.
        var cyclicGraphCheck = new CyclicGraphCheck();
        bool isCyclic = cyclicGraphCheck.IsCyclic(digraph);
        Assert.False(isCyclic);
    }

    #endregion

    #region Test Methods [Cyclic]

    [Fact]
    public void SimpleCyclic()
    {
        // Simple acyclic graph.
        var connList = new LightweightList<DirectedConnection>
        {
            new DirectedConnection(0, 3),
            new DirectedConnection(1, 3),
            new DirectedConnection(2, 3),
            new DirectedConnection(2, 4),
            new DirectedConnection(4, 1),
            new DirectedConnection(1, 2)
        };

        // Create graph.
        var connSpan = connList.AsSpan();
        connSpan.Sort();
        var digraph = DirectedGraphBuilder.Create(connSpan, 0, 0);

        // Test if cyclic.
        var cyclicGraphCheck = new CyclicGraphCheck();
        bool isCyclic = cyclicGraphCheck.IsCyclic(digraph);
        Assert.True(isCyclic);
    }

    [Fact]
    public void SimpleCyclic_DefinedNodes()
    {
        // Simple acyclic graph.
        var connList = new LightweightList<DirectedConnection>
        {
            new DirectedConnection(10, 13),
            new DirectedConnection(11, 13),
            new DirectedConnection(12, 13),
            new DirectedConnection(12, 14),
            new DirectedConnection(14, 11),
            new DirectedConnection(11, 12)
        };

        // Create graph.
        var connSpan = connList.AsSpan();
        connSpan.Sort();
        var digraph = DirectedGraphBuilder.Create(connSpan, 0, 10);

        // Test if cyclic.
        var cyclicGraphCheck = new CyclicGraphCheck();
        bool isCyclic = cyclicGraphCheck.IsCyclic(digraph);
        Assert.True(isCyclic);
    }

    [Fact]
    public void SimpleCyclic_DefinedNodes_NodeIdGap()
    {
        // Simple acyclic graph.
        var connList = new LightweightList<DirectedConnection>
        {
            new DirectedConnection(100, 103),
            new DirectedConnection(101, 103),
            new DirectedConnection(102, 103),
            new DirectedConnection(102, 104),
            new DirectedConnection(104, 101),
            new DirectedConnection(101, 102)
        };

        // Create graph.
        var connSpan = connList.AsSpan();
        connSpan.Sort();
        var digraph = DirectedGraphBuilder.Create(connSpan, 0, 10);

        // Test if cyclic.
        var cyclicGraphCheck = new CyclicGraphCheck();
        bool isCyclic = cyclicGraphCheck.IsCyclic(digraph);
        Assert.True(isCyclic);
    }

    #endregion
}
