using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Network;

namespace SharpNeat.Tests.Network
{
    [TestClass]
    public class DirectedGraphTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("DirectedGraph")]
        public void SimpleAcyclic()
        {
            // Simple acyclic graph.
            var connList = new List<DirectedConnection>
            {
                new DirectedConnection(0, 3),
                new DirectedConnection(1, 3),
                new DirectedConnection(2, 3),
                new DirectedConnection(2, 4)
            };

            // Create graph.
            var digraph = DirectedGraphBuilder.Create(connList, 0, 0);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connList, digraph.ConnectionIdArrays);

            // Check the node count.
            Assert.AreEqual(5, digraph.TotalNodeCount);
        }

        [TestMethod]
        [TestCategory("DirectedGraph")]
        public void SimpleAcyclic_DefinedNodes()
        {
            // Simple acyclic graph.
            var connList = new List<DirectedConnection>
            {
                new DirectedConnection(10, 13),
                new DirectedConnection(11, 13),
                new DirectedConnection(12, 13),
                new DirectedConnection(12, 14)
            };

            // Create graph.
            var digraph = DirectedGraphBuilder.Create(connList, 0, 10);

            // The graph should be unchanged from the input connections.
            CompareConnectionLists(connList, digraph.ConnectionIdArrays);

            // Check the node count.
            Assert.AreEqual(15, digraph.TotalNodeCount);
        }

        [TestMethod]
        [TestCategory("DirectedGraph")]
        public void SimpleAcyclic_DefinedNodes_NodeIdGap()
        {
            // Simple acyclic graph.
            var connList = new List<DirectedConnection>
            {
                new DirectedConnection(100, 103),
                new DirectedConnection(101, 103),
                new DirectedConnection(102, 103),
                new DirectedConnection(102, 104)
            };

            // Create graph.
            var digraph = DirectedGraphBuilder.Create(connList, 0, 10);

            // The gaps in the node IDs should be removed such that node IDs form a contiguous span starting from zero.
            var connListExpected = new List<DirectedConnection>
            {
                new DirectedConnection(10, 13),
                new DirectedConnection(11, 13),
                new DirectedConnection(12, 13),
                new DirectedConnection(12, 14)
            };

            CompareConnectionLists(connListExpected, digraph.ConnectionIdArrays);

            // Check the node count.
            Assert.AreEqual(15, digraph.TotalNodeCount);
        }

        #endregion

        #region Private Static Methods

        private static void CompareConnectionLists(IList<DirectedConnection> x, in ConnectionIdArrays connIdArrays)
        {
            int[] srcIdArr = connIdArrays._sourceIdArr;
            int[] tgtIdArr = connIdArrays._targetIdArr;

            Assert.AreEqual(x.Count, srcIdArr.Length);
            Assert.AreEqual(x.Count, tgtIdArr.Length);

            for(int i=0; i<x.Count; i++) 
            {
                Assert.AreEqual(x[i].SourceId, srcIdArr[i]);
                Assert.AreEqual(x[i].TargetId, tgtIdArr[i]);
            }
        }

        #endregion
    }
}
