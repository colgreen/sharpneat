using System;
using Redzen.Collections;
using SharpNeat.Graphs.Acyclic;
using Xunit;

namespace SharpNeat.Graphs
{
    public class CyclicGraphDepthAnalysisTests
    {
        [Fact]
        public void SimpleAcyclic_A1()
        {
            // Simple acyclic graph.
            var connList = new LightweightList<DirectedConnection>
            {
                new DirectedConnection(0, 3),
                new DirectedConnection(1, 3),
                new DirectedConnection(2, 3),
                new DirectedConnection(2, 4),
                new DirectedConnection(4, 3)
            };

            // Create graph.
            var connSpan = connList.AsSpan();
            connSpan.Sort();
            var digraph = DirectedGraphBuilder.Create(connSpan, 3, 2);

            // Perform depth analysis.
            GraphDepthInfo depthInfo = new CyclicGraphDepthAnalysis().CalculateNodeDepths(digraph);

            // Assertions.
            Assert.Equal(3, depthInfo._graphDepth);
            Assert.Equal(5, depthInfo._nodeDepthArr.Length);

            // Node depths.
            Assert.Equal(0, depthInfo._nodeDepthArr[0]);
            Assert.Equal(0, depthInfo._nodeDepthArr[1]);
            Assert.Equal(0, depthInfo._nodeDepthArr[2]);
            Assert.Equal(2, depthInfo._nodeDepthArr[3]);
            Assert.Equal(1, depthInfo._nodeDepthArr[4]);
        }

        [Fact]
        public void SimpleAcyclic_A2()
        {
            // Simple acyclic graph.
            var connList = new LightweightList<DirectedConnection>
            {
                new DirectedConnection(0, 3),
                new DirectedConnection(1, 3),
                new DirectedConnection(2, 3),
                new DirectedConnection(2, 4),
                new DirectedConnection(3, 4)
            };

            // Create graph.
            var connSpan = connList.AsSpan();
            connSpan.Sort();
            var digraph = DirectedGraphBuilder.Create(connSpan, 3, 2);

            // Perform depth analysis.
            GraphDepthInfo depthInfo = new CyclicGraphDepthAnalysis().CalculateNodeDepths(digraph);

            // Assertions.
            Assert.Equal(3, depthInfo._graphDepth);
            Assert.Equal(5, depthInfo._nodeDepthArr.Length);

            // Node depths.
            Assert.Equal(0, depthInfo._nodeDepthArr[0]);
            Assert.Equal(0, depthInfo._nodeDepthArr[1]);
            Assert.Equal(0, depthInfo._nodeDepthArr[2]);
            Assert.Equal(1, depthInfo._nodeDepthArr[3]);
            Assert.Equal(2, depthInfo._nodeDepthArr[4]);
        }

        [Fact]
        public void SimpleAcyclic_B1()
        {
            // Simple acyclic graph.
            var connList = new LightweightList<DirectedConnection>
            {
                new DirectedConnection(0, 2),
                new DirectedConnection(2, 3),
                new DirectedConnection(2, 4),
                new DirectedConnection(4, 3),
                new DirectedConnection(3, 1)
            };

            // Create graph.
            var connSpan = connList.AsSpan();
            connSpan.Sort();
            var digraph = DirectedGraphBuilder.Create(connSpan, 1, 1);

            // Perform depth analysis.
            GraphDepthInfo depthInfo = new CyclicGraphDepthAnalysis().CalculateNodeDepths(digraph);

            // Assertions.
            Assert.Equal(5, depthInfo._graphDepth);
            Assert.Equal(5, depthInfo._nodeDepthArr.Length);

            // Node depths.
            Assert.Equal(0, depthInfo._nodeDepthArr[0]);
            Assert.Equal(4, depthInfo._nodeDepthArr[1]);
            Assert.Equal(1, depthInfo._nodeDepthArr[2]);
            Assert.Equal(3, depthInfo._nodeDepthArr[3]);
            Assert.Equal(2, depthInfo._nodeDepthArr[4]);
        }

        [Fact]
        public void SimpleAcyclic_B2()
        {
            // Simple acyclic graph.
            var connList = new LightweightList<DirectedConnection>
            {
                new DirectedConnection(0, 2),
                new DirectedConnection(2, 3),
                new DirectedConnection(2, 4),
                new DirectedConnection(3, 4),
                new DirectedConnection(4, 1)
            };

            // Create graph.
            var connSpan = connList.AsSpan();
            connSpan.Sort();
            var digraph = DirectedGraphBuilder.Create(connSpan, 1, 1);

            // Perform depth analysis.
            GraphDepthInfo depthInfo = new CyclicGraphDepthAnalysis().CalculateNodeDepths(digraph);

            // Assertions.
            Assert.Equal(5, depthInfo._graphDepth);
            Assert.Equal(5, depthInfo._nodeDepthArr.Length);

            // Node depths.
            Assert.Equal(0, depthInfo._nodeDepthArr[0]);
            Assert.Equal(4, depthInfo._nodeDepthArr[1]);
            Assert.Equal(1, depthInfo._nodeDepthArr[2]);
            Assert.Equal(2, depthInfo._nodeDepthArr[3]);
            Assert.Equal(3, depthInfo._nodeDepthArr[4]);
        }

        [Fact]
        public void SimpleCyclic_A1()
        {
            // Simple acyclic graph.
            var connList = new LightweightList<DirectedConnection>
            {
                new DirectedConnection(0, 2),
                new DirectedConnection(2, 3),
                new DirectedConnection(3, 1),
                new DirectedConnection(1, 2)
            };

            // Create graph.
            var connSpan = connList.AsSpan();
            connSpan.Sort();
            var digraph = DirectedGraphBuilder.Create(connSpan, 1, 1);

            // Perform depth analysis.
            GraphDepthInfo depthInfo = new CyclicGraphDepthAnalysis().CalculateNodeDepths(digraph);

            // Assertions.
            Assert.Equal(4, depthInfo._graphDepth);
            Assert.Equal(4, depthInfo._nodeDepthArr.Length);

            // Node depths.
            Assert.Equal(0, depthInfo._nodeDepthArr[0]);
            Assert.Equal(3, depthInfo._nodeDepthArr[1]);
            Assert.Equal(1, depthInfo._nodeDepthArr[2]);
            Assert.Equal(2, depthInfo._nodeDepthArr[3]);
        }

        [Fact]
        public void SimpleCyclic_B1()
        {
            // Simple acyclic graph.
            var connList = new LightweightList<DirectedConnection>
            {
                new DirectedConnection(0, 4),
                new DirectedConnection(1, 5),
                new DirectedConnection(4, 4),
                new DirectedConnection(4, 6),
                new DirectedConnection(5, 5),
                new DirectedConnection(5, 7),
                new DirectedConnection(6, 2),
                new DirectedConnection(6, 5),
                new DirectedConnection(7, 3),
                new DirectedConnection(7, 4)
            };

            // Create graph.
            var connSpan = connList.AsSpan();
            connSpan.Sort();
            var digraph = DirectedGraphBuilder.Create(connSpan, 2, 2);

            // Perform depth analysis.
            GraphDepthInfo depthInfo = new CyclicGraphDepthAnalysis().CalculateNodeDepths(digraph);

            // Assertions.
            Assert.Equal(4, depthInfo._graphDepth);
            Assert.Equal(8, depthInfo._nodeDepthArr.Length);

            // Node depths.
            Assert.Equal(0, depthInfo._nodeDepthArr[0]);
            Assert.Equal(0, depthInfo._nodeDepthArr[1]);

            Assert.Equal(1, depthInfo._nodeDepthArr[4]);
            Assert.Equal(1, depthInfo._nodeDepthArr[5]);

            Assert.Equal(2, depthInfo._nodeDepthArr[6]);
            Assert.Equal(2, depthInfo._nodeDepthArr[7]);

            Assert.Equal(3, depthInfo._nodeDepthArr[2]);
            Assert.Equal(3, depthInfo._nodeDepthArr[3]);
        }
    }
}
