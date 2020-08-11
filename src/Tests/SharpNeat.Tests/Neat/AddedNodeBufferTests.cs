using SharpNeat.Graphs;
using Xunit;

namespace SharpNeat.Neat.Tests
{
    public class AddedNodeBufferTests
    {
        #region Test Methods

        [Fact]
        public void TryLookup()
        {
            AddedNodeBuffer buff = new AddedNodeBuffer(10);

            // Register some added nodes.
            buff.Register(new DirectedConnection(100, 101), 102);
            buff.Register(new DirectedConnection(103, 104), 105);

            // Test lookups.
            TestLookupSuccess(buff, new DirectedConnection(100, 101), 102);
            TestLookupSuccess(buff, new DirectedConnection(103, 104), 105);

            // Test lookup failure.
            TestLookupFail(buff, new DirectedConnection(100, 102));
            TestLookupFail(buff, new DirectedConnection(101, 100));
            TestLookupFail(buff, new DirectedConnection(103, 102));
            TestLookupFail(buff, new DirectedConnection(104, 103));
        }

        #endregion

        #region Private Static Methods

        private static void TestLookupSuccess(AddedNodeBuffer buff, in DirectedConnection connection, int expectedAddedNodeId)
        {
            Assert.True(buff.TryLookup(in connection, out int addedNodeId));
            Assert.Equal(expectedAddedNodeId, addedNodeId);
        }

        private static void TestLookupFail(AddedNodeBuffer buff, in DirectedConnection connection)
        {
            Assert.False(buff.TryLookup(in connection, out _));
        }

        #endregion
    }
}
