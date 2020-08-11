using System;
using SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;
using SharpNeat.Graphs;
using Xunit;

namespace SharpNeat.Sexual.Strategy.UniformCrossover.Tests
{
    public class CyclicConnectionTestTests
    {
        [Fact]
        public void IsConnectionCyclic1()
        {
            var cyclicTest = new CyclicConnectionTest();

            var connArr = new DirectedConnection[3];
            connArr[0] = new DirectedConnection(0, 1);
            connArr[1] = new DirectedConnection(1, 2);
            connArr[2] = new DirectedConnection(2, 3);

            // True tests (cycle).
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(0, 0)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(1, 1)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 2)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 3)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(1, 0)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 0)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 0)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 1)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 1)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 2)));

            // False tests (no cycle).
            Assert.False(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(0, 2)));
            Assert.False(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(0, 3)));
            Assert.False(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(1, 3)));
            Assert.False(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 3)));
        }

        [Fact]
        public void IsConnectionCyclic2()
        {
            var cyclicTest = new CyclicConnectionTest();

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
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 1)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 4)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(5, 2)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(5, 0)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(5, 4)));
            Assert.True(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(6, 0)));

            // False tests (no cycle).
            Assert.False(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 5)));
            Assert.False(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(1, 3)));
            Assert.False(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(6, 1)));
            Assert.False(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(6, 2)));
            Assert.False(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(6, 4)));
            Assert.False(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 4)));
        }
    }
}
