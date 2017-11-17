using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeatLib.Tests.Neat.Network
{
    public static class ConnectionGeneCompareUtils
    {
        #region Public Static Methods

        public static void CompareConnectionLists(ConnectionGene<double>[] x,
                                                  ConnectionIdArrays connIdArrays, double[] yWeightArr)
        {
            int[] srcIdArr = connIdArrays._sourceIdArr;
            int[] tgtIdArr = connIdArrays._targetIdArr;

            Assert.AreEqual(x.Length, srcIdArr.Length);
            Assert.AreEqual(x.Length, tgtIdArr.Length);

            for(int i=0; i<x.Length; i++) {
                CompareConnections(x[i], srcIdArr[i], tgtIdArr[i], yWeightArr[i]);
            }
        }

        public static void CompareConnections(ConnectionGene<double> x,
                                              int ySrcId, int yTgtId, double yWeight)
        {
            Assert.AreEqual(x.SourceId, ySrcId);
            Assert.AreEqual(x.TargetId, yTgtId);
            Assert.AreEqual(x.Weight, yWeight);
        }

        #endregion
    }
}
