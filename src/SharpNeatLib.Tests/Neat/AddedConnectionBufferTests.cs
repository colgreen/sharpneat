using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat;
using SharpNeat.Network;

namespace SharpNeatLib.Tests.Neat
{
    [TestClass]
    
    public class AddedConnectionBufferTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("AddedConnectionBuffer")]
        public void TestInputOutputConnectionLookup()
        {
            AddedConnectionBuffer buff = new AddedConnectionBuffer(10, 3, 2);

            // Test lookup of input-output connections.
            TestLookupSuccess(buff, 0, 3, 5);
            TestLookupSuccess(buff, 0, 4, 6);
            TestLookupSuccess(buff, 1, 3, 7);
            TestLookupSuccess(buff, 1, 4, 8);
            TestLookupSuccess(buff, 2, 3, 9);
            TestLookupSuccess(buff, 2, 4, 10);

            // Confirm that non input-output connections fail; no other connections have been registered.
            TestLookupFail(buff, 0, 20);
            TestLookupFail(buff, 20, 3);
            TestLookupFail(buff, 21, 22);
        }

        [TestMethod]
        [TestCategory("AddedConnectionBuffer")]
        public void TestConnectionLookup()
        {
            AddedConnectionBuffer buff = new AddedConnectionBuffer(10, 3, 2);

            // Register some connections and associated innovation IDs.
            buff.Register(new DirectedConnection(0, 5), 100);
            buff.Register(new DirectedConnection(3, 20), 101);
            buff.Register(new DirectedConnection(20, 21), 102);
            buff.Register(new DirectedConnection(21, 20), 103);

            // Test lookups.
            TestLookupSuccess(buff, 0, 5, 100);
            TestLookupSuccess(buff, 3, 20, 101);
            TestLookupSuccess(buff, 20, 21, 102);
            TestLookupSuccess(buff, 21, 20, 103);

            // Test lookup failure.
            TestLookupFail(buff, 0, 6);
            TestLookupFail(buff, 4, 20);
            TestLookupFail(buff, 20, 22);
            TestLookupFail(buff, 22, 20);
        }

        #endregion

        #region Private Static Methods

        private static void TestLookupSuccess(AddedConnectionBuffer buff, int srcId, int tgtId, uint expectedConnectionId)
        {
            uint connectionId;
            Assert.AreEqual(true, buff.TryLookup(new DirectedConnection(srcId, tgtId), out connectionId));
            Assert.AreEqual(expectedConnectionId, connectionId);
        }

        private static void TestLookupFail(AddedConnectionBuffer buff, int srcId, int tgtId)
        {
            uint connectionId;
            Assert.AreEqual(false, buff.TryLookup(new DirectedConnection(srcId, tgtId), out connectionId));
        }

        #endregion
    }
}
