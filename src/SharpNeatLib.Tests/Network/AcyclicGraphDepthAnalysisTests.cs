using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.Tests.Network
{
    [TestClass]
    public class AcyclicGraphDepthAnalysisTests
    {
        #region Test Methods

        /// <summary>
        /// Input 1 has a connection coming into it; although this is not allowed in NeatGenome, it is 
        /// is allowed by DirectedGraph, so we test it works as expected.
        /// </summary>
        [TestMethod]
        [TestCategory("AcyclicGraphDepthAnalysis")]
        public void ConnectThroughInput()
        {
            // Simple acyclic graph.
            var connList = new List<DirectedConnection>
            {
                new DirectedConnection(0, 3),
                new DirectedConnection(1, 3),
                new DirectedConnection(2, 3),
                new DirectedConnection(2, 4),
                new DirectedConnection(4, 1)
            };

            // Create graph.
            connList.Sort();
            var digraph = DirectedGraphFactory.Create(connList, 3, 2);

            // Assert is acyclic.
            Assert.IsTrue(!CyclicGraphAnalysis.IsCyclicStatic(digraph));

            // Depth analysis.
            GraphDepthInfo depthInfo = AcyclicGraphDepthAnalysis.CalculateNodeDepths(digraph);

            // Assertions.
            Assert.AreEqual(4, depthInfo._networkDepth);
            Assert.AreEqual(5, depthInfo._nodeDepthArr.Length);

            // Node depths.
            Assert.AreEqual(0, depthInfo._nodeDepthArr[0]);
            Assert.AreEqual(2, depthInfo._nodeDepthArr[1]);
            Assert.AreEqual(0, depthInfo._nodeDepthArr[2]);
            Assert.AreEqual(3, depthInfo._nodeDepthArr[3]);
            Assert.AreEqual(1, depthInfo._nodeDepthArr[4]);
        }

        /// <summary>
        /// An output has both a short and a long path connecting to it.
        /// It's depth should be the number of hops in the long path.
        /// A second output has a single connection from the first, so should have 
        /// a depth one higher.
        /// </summary>
        [TestMethod]
        [TestCategory("AcyclicGraphDepthAnalysis")]
        public void ShortAndLongPath()
        {
            // Simple acyclic graph.
            var connList = new List<DirectedConnection>
            {
                new DirectedConnection(0, 4),
                new DirectedConnection(4, 5),
                new DirectedConnection(5, 2),
                new DirectedConnection(1, 2),
                new DirectedConnection(2, 3)
            };

            // Create graph.
            connList.Sort();
            var digraph = DirectedGraphFactory.Create(connList, 2, 2);

            // Assert is acyclic.
            Assert.IsTrue(!CyclicGraphAnalysis.IsCyclicStatic(digraph));

            // Depth analysis.
            GraphDepthInfo depthInfo = AcyclicGraphDepthAnalysis.CalculateNodeDepths(digraph);

            // Assertions.
            Assert.AreEqual(5, depthInfo._networkDepth);
            Assert.AreEqual(6, depthInfo._nodeDepthArr.Length);

            // Node depths.
            Assert.AreEqual(0, depthInfo._nodeDepthArr[0]);
            Assert.AreEqual(0, depthInfo._nodeDepthArr[1]);
            Assert.AreEqual(3, depthInfo._nodeDepthArr[2]);
            Assert.AreEqual(4, depthInfo._nodeDepthArr[3]);
            Assert.AreEqual(1, depthInfo._nodeDepthArr[4]);
            Assert.AreEqual(2, depthInfo._nodeDepthArr[5]);
        }

        #endregion
    }
}
