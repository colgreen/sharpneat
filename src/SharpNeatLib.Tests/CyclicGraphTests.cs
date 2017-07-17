using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Network2;

namespace SharpNeatLib.Tests
{
    [TestClass]
    public class CyclicGraphTests
    {
        #region Test Methods [Acyclic]

        [TestMethod]
        [TestCategory("CyclicGraph")]
        public void SimpleAcyclic()
        {
            // Simple acyclic graph.
            var connList = new List<IDirectedConnection>();
            connList.Add(new DirectedConnection(0, 3));
            connList.Add(new DirectedConnection(1, 3));
            connList.Add(new DirectedConnection(2, 3));
            connList.Add(new DirectedConnection(2, 4));
            connList.Add(new DirectedConnection(4, 1));

            // Create graph.
            var digraph = DirectedGraphFactory.Create(connList, null);

            // Test if cyclic.
            bool isCyclic = CyclicGraphTest.IsCyclic(digraph);
            Assert.IsFalse(isCyclic);
        }

        [TestMethod]
        [TestCategory("CyclicGraph")]
        public void SimpleAcyclic_DefinedNodes()
        {
            // Simple acyclic graph.
            var connList = new List<IDirectedConnection>();
            connList.Add(new DirectedConnection(10, 13));
            connList.Add(new DirectedConnection(11, 13));
            connList.Add(new DirectedConnection(12, 13));
            connList.Add(new DirectedConnection(12, 14));
            connList.Add(new DirectedConnection(14, 11));

            // Create graph.
            var digraph = DirectedGraphFactory.Create(connList, Enumerable.Range(0, 10));

            // Test if cyclic.
            bool isCyclic = CyclicGraphTest.IsCyclic(digraph);
            Assert.IsFalse(isCyclic);
        }

        [TestMethod]
        [TestCategory("CyclicGraph")]
        public void SimpleAcyclic_DefinedNodes_NodeIdGap()
        {
            // Simple acyclic graph.
            var connList = new List<IDirectedConnection>();
            connList.Add(new DirectedConnection(100, 103));
            connList.Add(new DirectedConnection(101, 103));
            connList.Add(new DirectedConnection(102, 103));
            connList.Add(new DirectedConnection(102, 104));
            connList.Add(new DirectedConnection(104, 101));

            // Create graph.
            var digraph = DirectedGraphFactory.Create(connList, Enumerable.Range(0, 10));

            // Test if cyclic.
            bool isCyclic = CyclicGraphTest.IsCyclic(digraph);
            Assert.IsFalse(isCyclic);
        }

        #endregion

        #region Test Methods [Acyclic]

        [TestMethod]
        [TestCategory("CyclicGraph")]
        public void SimpleCyclic()
        {
            // Simple acyclic graph.
            var connList = new List<IDirectedConnection>();
            connList.Add(new DirectedConnection(0, 3));
            connList.Add(new DirectedConnection(1, 3));
            connList.Add(new DirectedConnection(2, 3));
            connList.Add(new DirectedConnection(2, 4));
            connList.Add(new DirectedConnection(4, 1));
            connList.Add(new DirectedConnection(1, 2));

            // Create graph.
            var digraph = DirectedGraphFactory.Create(connList, null);

            // Test if cyclic.
            bool isCyclic = CyclicGraphTest.IsCyclic(digraph);
            Assert.IsTrue(isCyclic);
        }

        [TestMethod]
        [TestCategory("CyclicGraph")]
        public void SimpleCyclic_DefinedNodes()
        {
            // Simple acyclic graph.
            var connList = new List<IDirectedConnection>();
            connList.Add(new DirectedConnection(10, 13));
            connList.Add(new DirectedConnection(11, 13));
            connList.Add(new DirectedConnection(12, 13));
            connList.Add(new DirectedConnection(12, 14));
            connList.Add(new DirectedConnection(14, 11));
            connList.Add(new DirectedConnection(11, 12));

            // Create graph.
            var digraph = DirectedGraphFactory.Create(connList, Enumerable.Range(0, 10));

            // Test if cyclic.
            bool isCyclic = CyclicGraphTest.IsCyclic(digraph);
            Assert.IsTrue(isCyclic);
        }

        [TestMethod]
        [TestCategory("CyclicGraph")]
        public void SimpleCyclic_DefinedNodes_NodeIdGap()
        {
            // Simple acyclic graph.
            var connList = new List<IDirectedConnection>();
            connList.Add(new DirectedConnection(100, 103));
            connList.Add(new DirectedConnection(101, 103));
            connList.Add(new DirectedConnection(102, 103));
            connList.Add(new DirectedConnection(102, 104));
            connList.Add(new DirectedConnection(104, 101));
            connList.Add(new DirectedConnection(101, 102));

            // Create graph.
            var digraph = DirectedGraphFactory.Create(connList, Enumerable.Range(0, 10));

            // Test if cyclic.
            bool isCyclic = CyclicGraphTest.IsCyclic(digraph);
            Assert.IsTrue(isCyclic);
        }

        #endregion
    }
}
