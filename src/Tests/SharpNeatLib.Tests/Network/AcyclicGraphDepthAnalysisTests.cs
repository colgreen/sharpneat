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
            var digraph = DirectedGraphBuilder.Create(connList, 3, 2);

            // Assert is acyclic.
            var cyclicGraphAnalysis = new CyclicGraphAnalysis();
            Assert.IsTrue(!cyclicGraphAnalysis.IsCyclic(digraph));

            // Depth analysis.
            GraphDepthInfo depthInfo = new AcyclicGraphDepthAnalysis().CalculateNodeDepths(digraph);

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
            var digraph = DirectedGraphBuilder.Create(connList, 2, 2);

            // Assert is acyclic.
            var cyclicGraphAnalysis = new CyclicGraphAnalysis();
            Assert.IsTrue(!cyclicGraphAnalysis.IsCyclic(digraph));

            // Depth analysis.
            GraphDepthInfo depthInfo = new AcyclicGraphDepthAnalysis().CalculateNodeDepths(digraph);

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

        [TestMethod]
        [TestCategory("AcyclicGraphDepthAnalysis")]
        public void Random1()
        {
            // Simple acyclic graph.
            var connList = new List<DirectedConnection>
            {
                new DirectedConnection(0, 2),
                new DirectedConnection(0, 3),
                new DirectedConnection(0, 4),
                new DirectedConnection(2, 5),
                new DirectedConnection(2, 10),
                new DirectedConnection(3, 1),
                new DirectedConnection(3, 4),
                new DirectedConnection(3, 6),
                new DirectedConnection(3, 7),
                new DirectedConnection(4, 1),
                new DirectedConnection(4, 6),
                new DirectedConnection(4, 7),
                new DirectedConnection(4, 10),
                new DirectedConnection(5, 4),
                new DirectedConnection(5, 8),
                new DirectedConnection(6, 9),
                new DirectedConnection(7, 9),
                new DirectedConnection(7, 10),
                new DirectedConnection(8, 1),
                new DirectedConnection(8, 3),
                new DirectedConnection(8, 9),
                new DirectedConnection(9, 1),
                new DirectedConnection(10, 1),
            };

            // Create graph.
            connList.Sort();
            var digraph = DirectedGraphBuilder.Create(connList, 1, 1);

            // Depth analysis.
            GraphDepthInfo depthInfo = new AcyclicGraphDepthAnalysis().CalculateNodeDepths(digraph);

            // Assertions.
            Assert.AreEqual(9, depthInfo._networkDepth);
            Assert.AreEqual(11, depthInfo._nodeDepthArr.Length);

            // Node depths.
            Assert.AreEqual(0, depthInfo._nodeDepthArr[0]);
            Assert.AreEqual(8, depthInfo._nodeDepthArr[1]);
            Assert.AreEqual(1, depthInfo._nodeDepthArr[2]);
            Assert.AreEqual(4, depthInfo._nodeDepthArr[3]);
            Assert.AreEqual(5, depthInfo._nodeDepthArr[4]);
            Assert.AreEqual(2, depthInfo._nodeDepthArr[5]);
            Assert.AreEqual(6, depthInfo._nodeDepthArr[6]);
            Assert.AreEqual(6, depthInfo._nodeDepthArr[7]);
            Assert.AreEqual(3, depthInfo._nodeDepthArr[8]);
            Assert.AreEqual(7, depthInfo._nodeDepthArr[9]);
            Assert.AreEqual(7, depthInfo._nodeDepthArr[10]);
        }

        #endregion
    }
}
