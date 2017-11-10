using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;

namespace SharpNeatLib.Tests.Neat.Genome
{
    [TestClass]
    public class ConnectionGeneUtilsTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("ConnectionGeneUtils")]
        public void TestGetConnectionIndexBySourceNodeId()
        {
            var connArr = new ConnectionGene<double>[9];
            connArr[0] = new ConnectionGene<double>(0, 1, 70, 1.0);
            connArr[1] = new ConnectionGene<double>(1, 4, 14, 1.0);
            connArr[2] = new ConnectionGene<double>(2, 6, 23, 1.0);
            connArr[3] = new ConnectionGene<double>(3, 7, 36, 1.0);
            connArr[4] = new ConnectionGene<double>(4, 10, 18, 1.0);
            connArr[5] = new ConnectionGene<double>(5, 10, 31, 1.0);
            connArr[6] = new ConnectionGene<double>(6, 14, 2, 1.0);
            connArr[7] = new ConnectionGene<double>(7, 20, 24, 1.0);
            connArr[8] = new ConnectionGene<double>(8, 25, 63, 1.0);

            int idx = ConnectionGeneUtils.GetConnectionIndexBySourceNodeId(connArr, 10);
            Assert.AreEqual(4, idx);
        }
        
        #endregion
    }
}
