using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat;
using SharpNeat.Network;

namespace SharpNeatLib.Tests.Neat
{
    [TestClass]
    public class AddedNodeBufferTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("AddedNodeBuffer")]
        public void TestLookup()
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

        private static void TestLookupSuccess(AddedNodeBuffer buff, DirectedConnection connection, int expectedAddedNodeId)
        {
            Assert.IsTrue(buff.TryLookup(connection, out int addedNodeId));
            Assert.AreEqual(expectedAddedNodeId, addedNodeId);
        }

        private static void TestLookupFail(AddedNodeBuffer buff, DirectedConnection connection)
        {
            Assert.IsFalse(buff.TryLookup(connection, out int addedNodeId));
        }

        #endregion
    }
}
