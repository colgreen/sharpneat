using Redzen.Collections;
using Xunit;

namespace SharpNeat.Graphs;

public class CyclicGraphCheckTests
{
    #region Test Methods [Acyclic]

    [Fact]
    public void SimpleAcyclic()
    {
        // Simple acyclic graph.
        var connList = new LightweightList<DirectedConnection>
        {
            new(0, 3),
            new(1, 3),
            new(2, 3),
            new(2, 4),
            new(4, 1)
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
            new(10, 13),
            new(11, 13),
            new(12, 13),
            new(12, 14),
            new(14, 11)
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
            new(100, 103),
            new(101, 103),
            new(102, 103),
            new(102, 104),
            new(104, 101)
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
            new(0, 2),
            new(0, 3),
            new(0, 4),
            new(2, 5),
            new(3, 6),
            new(4, 7),
            new(5, 8),
            new(6, 4),
            new(6, 9),
            new(7, 10),
            new(8, 1),
            new(9, 1),
            new(10, 1)
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
            new(0, 3),
            new(1, 3),
            new(2, 3),
            new(2, 4),
            new(4, 1),
            new(1, 2)
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
            new(10, 13),
            new(11, 13),
            new(12, 13),
            new(12, 14),
            new(14, 11),
            new(11, 12)
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
            new(100, 103),
            new(101, 103),
            new(102, 103),
            new(102, 104),
            new(104, 101),
            new(101, 102)
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
