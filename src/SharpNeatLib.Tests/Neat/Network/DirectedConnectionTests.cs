using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Network;

namespace SharpNeatLib.Tests.Neat.Network
{
    [TestClass]
    public class DirectedConnectionTests
    {

        [TestMethod]
        [TestCategory("DirectedConnection")]
        public void TestDirectedConnection_Equals()
        {
            Assert.IsTrue(new DirectedConnection(10,20).Equals(new DirectedConnection(10,20)));
            Assert.IsTrue(new DirectedConnection(10,20) ==  new DirectedConnection(10,20));

            Assert.IsFalse(!new DirectedConnection(10,20).Equals(new DirectedConnection(10,20)));
            Assert.IsFalse(new DirectedConnection(10,20) !=  new DirectedConnection(10,20));

            Assert.IsFalse(new DirectedConnection(10,20).Equals(new DirectedConnection(10,21)));
            Assert.IsFalse(new DirectedConnection(10,20) ==  new DirectedConnection(10,21));

            Assert.IsFalse(new DirectedConnection(10,20).Equals(new DirectedConnection(11,20)));
            Assert.IsFalse(new DirectedConnection(10,20) ==  new DirectedConnection(11,20));
        }

        [TestMethod]
        [TestCategory("DirectedConnection")]
        public void TestDirectedConnection_LessThan()
        {
            Assert.IsTrue(new DirectedConnection(10,20) < (new DirectedConnection(10,21)));
            Assert.IsTrue(new DirectedConnection(10,20) < (new DirectedConnection(11,20)));

            Assert.IsFalse(new DirectedConnection(10,20) < (new DirectedConnection(10,20)));
            Assert.IsFalse(new DirectedConnection(10,20) < (new DirectedConnection(9,20)));
            Assert.IsFalse(new DirectedConnection(10,20) < (new DirectedConnection(10,19)));
            Assert.IsFalse(new DirectedConnection(10,20) < (new DirectedConnection(9,19)));
        }

        [TestMethod]
        [TestCategory("DirectedConnection")]
        public void TestDirectedConnection_GreaterThan()
        {
            Assert.IsTrue(new DirectedConnection(10,21) > (new DirectedConnection(10,20)));
            Assert.IsTrue(new DirectedConnection(11,20) > (new DirectedConnection(10,20)));

            Assert.IsFalse(new DirectedConnection(10,20) > (new DirectedConnection(10,20)));
            Assert.IsFalse(new DirectedConnection(9,20) > (new DirectedConnection(10,20)));
            Assert.IsFalse(new DirectedConnection(10,19) > (new DirectedConnection(10,20)));
            Assert.IsFalse(new DirectedConnection(9,19) > (new DirectedConnection(10,20)));
        }

        [TestMethod]
        [TestCategory("DirectedConnection")]
        public void TestDirectedConnection_CompareTo()
        {
            Assert.AreEqual(0, new DirectedConnection(10,20).CompareTo(new DirectedConnection(10,20)));

            Assert.AreEqual(1, new DirectedConnection(10,21).CompareTo(new DirectedConnection(10,20)));
            Assert.AreEqual(1, new DirectedConnection(11,20).CompareTo(new DirectedConnection(10,20)));
            Assert.AreEqual(1, new DirectedConnection(11,21).CompareTo(new DirectedConnection(10,20)));

            Assert.AreEqual(-1, new DirectedConnection(10,20).CompareTo(new DirectedConnection(10,21)));
            Assert.AreEqual(-1, new DirectedConnection(10,20).CompareTo(new DirectedConnection(11,20)));
            Assert.AreEqual(-1, new DirectedConnection(10,20).CompareTo(new DirectedConnection(11,21)));
        }
    }
}
