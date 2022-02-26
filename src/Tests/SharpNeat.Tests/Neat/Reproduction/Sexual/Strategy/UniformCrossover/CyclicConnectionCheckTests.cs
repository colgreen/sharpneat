using System;
using SharpNeat.Graphs;
using SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;
using Xunit;

namespace SharpNeat.Sexual.Strategy.UniformCrossover.Tests;

public class CyclicConnectionCheckTests
{
    [Fact]
    public void IsConnectionCyclic1()
    {
        var cyclicCheck = new CyclicConnectionCheck();

        var connArr = new DirectedConnection[3];
        connArr[0] = new DirectedConnection(0, 1);
        connArr[1] = new DirectedConnection(1, 2);
        connArr[2] = new DirectedConnection(2, 3);

        // True tests (cycle).
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(0, 0)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(1, 1)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(2, 2)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(3, 3)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(1, 0)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(2, 0)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(3, 0)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(2, 1)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(3, 1)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(3, 2)));

        // False tests (no cycle).
        Assert.False(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(0, 2)));
        Assert.False(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(0, 3)));
        Assert.False(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(1, 3)));
        Assert.False(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(2, 3)));
    }

    [Fact]
    public void IsConnectionCyclic2()
    {
        var cyclicCheck = new CyclicConnectionCheck();

        var connArr = new DirectedConnection[8];
        connArr[0] = new DirectedConnection(0, 1);
        connArr[1] = new DirectedConnection(0, 2);
        connArr[2] = new DirectedConnection(0, 3);
        connArr[3] = new DirectedConnection(1, 4);
        connArr[4] = new DirectedConnection(4, 2);
        connArr[5] = new DirectedConnection(2, 5);
        connArr[6] = new DirectedConnection(3, 6);
        connArr[7] = new DirectedConnection(3, 2);
        Array.Sort(connArr);

        // True tests (cycle).
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(2, 1)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(2, 4)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(5, 2)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(5, 0)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(5, 4)));
        Assert.True(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(6, 0)));

        // False tests (no cycle).
        Assert.False(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(3, 5)));
        Assert.False(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(1, 3)));
        Assert.False(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(6, 1)));
        Assert.False(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(6, 2)));
        Assert.False(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(6, 4)));
        Assert.False(cyclicCheck.IsConnectionCyclic(connArr, new DirectedConnection(3, 4)));
    }
}
