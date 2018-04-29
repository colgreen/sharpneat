using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using SharpNeat.Network;

namespace SharpNeat.Tests.Neat.Reproduction.Asexual.Strategy
{
    [TestClass]
    public class CyclicConnectionTestTest
    {
        [TestMethod]
        [TestCategory("CyclicConnectionTestWithIndexes")]
        public void TestIsConnectionCyclic1()
        {
            var cyclicTest= new CyclicConnectionTest();

            var connArr = new DirectedConnection[3];
            connArr[0] = new DirectedConnection(0, 1);
            connArr[1] = new DirectedConnection(1, 2);
            connArr[2] = new DirectedConnection(2, 3);

            INodeIdMap nodeIdxByIdMap = DirectedGraphUtils.CompileNodeIdMap_InputOutputCount_HiddenNodeIdArr(
                0, new int[]{ 0, 1, 2, 3 });

            // True tests (cycle).
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(0, 0)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(1, 1)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(2, 2)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(3, 3)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(1, 0)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(2, 0)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(3, 0)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(2, 1)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(3, 1)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(3, 2)));

            // False tests (no cycle).
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(0, 2)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(0, 3)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(1, 3)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 4, new DirectedConnection(2, 3)));
        }

        [TestMethod]
        [TestCategory("CyclicConnectionTestWithIndexes")]
        public void TestIsConnectionCyclic2()
        {
            var cyclicTest= new CyclicConnectionTest();

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

            INodeIdMap nodeIdxByIdMap = DirectedGraphUtils.CompileNodeIdMap_InputOutputCount_HiddenNodeIdArr(
                0, new int[]{ 0, 1, 2, 3, 4, 5, 6 });

            // True tests (cycle).
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(2, 1)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(2, 4)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(5, 2)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(5, 0)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(5, 4)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(6, 0)));

            // False tests (no cycle).
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(3, 5)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(1, 3)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(6, 1)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(6, 2)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(6, 4)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(3, 4)));
        }

        [TestMethod]
        [TestCategory("CyclicConnectionTestWithIndexes")]
        public void TestIsConnectionCyclic3()
        {
            var cyclicTest= new CyclicConnectionTest();

            var connArr = new DirectedConnection[8];
            connArr[0] = new DirectedConnection(0, 10);
            connArr[1] = new DirectedConnection(0, 20);
            connArr[2] = new DirectedConnection(0, 30);
            connArr[3] = new DirectedConnection(10, 40);
            connArr[4] = new DirectedConnection(40, 20);
            connArr[5] = new DirectedConnection(20, 50);
            connArr[6] = new DirectedConnection(30, 60);
            connArr[7] = new DirectedConnection(30, 20);
            Array.Sort(connArr);

            INodeIdMap nodeIdxByIdMap = DirectedGraphUtils.CompileNodeIdMap_InputOutputCount_HiddenNodeIdArr(
                0, new int[]{ 0, 10, 20, 30, 40, 50, 60 });

            // True tests (cycle).
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7,new DirectedConnection(20, 10)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(20, 40)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(50, 20)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(50, 0)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(50, 40)));
            Assert.IsTrue(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(60, 0)));

            // False tests (no cycle).
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(30, 50)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(10, 30)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(60, 10)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(60, 20)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(60, 40)));
            Assert.IsFalse(cyclicTest.IsConnectionCyclic(connArr, nodeIdxByIdMap, 7, new DirectedConnection(30, 40)));
        }
    }
}
