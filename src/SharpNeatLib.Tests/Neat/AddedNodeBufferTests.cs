using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat;

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
            buff.Register(100, new AddedNodeInfo(101, 102, 103));
            buff.Register(102, new AddedNodeInfo(104, 105, 106));

            // Test lookups.
            TestLookupSuccess(buff, 100, new AddedNodeInfo(101, 102, 103));
            TestLookupSuccess(buff, 102, new AddedNodeInfo(104, 105, 106));

            // Test lookup failure.
            TestLookupFail(buff, 0);
            TestLookupFail(buff, 101);
            TestLookupFail(buff, 103);
            TestLookupFail(buff, 104);
            TestLookupFail(buff, 105);
            TestLookupFail(buff, 106);
        }

        #endregion

        #region Private Static Methods

        private static void TestLookupSuccess(AddedNodeBuffer buff, int connectionId, AddedNodeInfo expectedAddedNodeInfo)
        {
            AddedNodeInfo addedNodeInfo;
            Assert.AreEqual(true, buff.TryLookup(connectionId, out addedNodeInfo));
            Assert.IsTrue(AreEqual(expectedAddedNodeInfo, addedNodeInfo));
        }

        private static void TestLookupFail(AddedNodeBuffer buff, int connectionId)
        {
            AddedNodeInfo addedNodeInfo;
            Assert.AreEqual(false, buff.TryLookup(connectionId, out addedNodeInfo));
        }

        private static bool AreEqual(AddedNodeInfo x, AddedNodeInfo y)
        {
            return x.AddedNodeId == y.AddedNodeId
                && x.AddedInputConnectionId == y.AddedInputConnectionId
                && x.AddedOutputConnectionId == y.AddedOutputConnectionId;
        }

        #endregion
    }
}
