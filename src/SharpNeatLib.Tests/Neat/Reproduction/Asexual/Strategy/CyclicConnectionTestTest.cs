using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using SharpNeat.Network;

namespace SharpNeatLib.Tests.Neat.Reproduction.Asexual.Strategy
{
    [TestClass]
    public class CyclicConnectionTestTest
    {
        [TestMethod]
        [TestCategory("CyclicConnectionTest")]
        public void TestIsConnectionCyclic1()
        {
            var cyclicTest= new CyclicConnectionTest<double>();

            var connArr = new ConnectionGene<double>[3];
            connArr[0] = new ConnectionGene<double>(0, 0, 1, 1.0);
            connArr[1] = new ConnectionGene<double>(1, 1, 2, 1.0);
            connArr[2] = new ConnectionGene<double>(2, 2, 3, 1.0);

            // True tests (cycle).
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(0, 0)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(1, 1)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 2)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 3)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(1, 0)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 0)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 0)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 1)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 1)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 2)));

            // False tests (no cycle).
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(0, 2)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(0, 3)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(1, 3)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 3)));
        }

        [TestMethod]
        [TestCategory("CyclicConnectionTest")]
        public void TestIsConnectionCyclic2()
        {
            var cyclicTest= new CyclicConnectionTest<double>();

            var connArr = new ConnectionGene<double>[8];
            connArr[0] = new ConnectionGene<double>(0, 0, 1, 1.0);
            connArr[1] = new ConnectionGene<double>(1, 0, 2, 1.0);
            connArr[2] = new ConnectionGene<double>(2, 0, 3, 1.0);
            connArr[3] = new ConnectionGene<double>(3, 1, 4, 1.0);
            connArr[4] = new ConnectionGene<double>(4, 4, 2, 1.0);
            connArr[5] = new ConnectionGene<double>(5, 2, 5, 1.0);
            connArr[6] = new ConnectionGene<double>(6, 3, 6, 1.0);
            connArr[7] = new ConnectionGene<double>(7, 3, 2, 1.0);
            Array.Sort(connArr, DirectedConnectionComparer.__Instance);

            // True tests (cycle).
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 1)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(2, 4)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(5, 2)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(5, 0)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(5, 4)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(6, 0)));

            // False tests (no cycle).
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 5)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(1, 3)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(6, 1)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(6, 2)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(6, 4)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, new DirectedConnection(3, 4)));
        }
    }
}
