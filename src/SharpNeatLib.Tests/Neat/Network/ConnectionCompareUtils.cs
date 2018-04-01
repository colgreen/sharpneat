using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeat.Tests.Neat.Network
{
    public static class ConnectionCompareUtils
    {
        #region Public Static Methods

        public static void CompareConnectionLists<T>(
            DirectedConnection[] xConnArr, T[] xWeightArr,
            ConnectionIdArrays yIdArrays, T[] yWeightArr)
            where T : struct
        {
            int xlen = xConnArr.Length;
            Assert.AreEqual(xlen, xWeightArr.Length);
            Assert.AreEqual(xlen, yIdArrays._sourceIdArr.Length);
            Assert.AreEqual(xlen, yIdArrays._targetIdArr.Length);
            Assert.AreEqual(xlen, yWeightArr.Length);

            for(int i=0; i < xlen; i++)  {
                Assert.IsTrue(AreEqual(xConnArr, xWeightArr, yIdArrays, yWeightArr, i));
            }
        }

        public static bool AreEqual<T>(
            DirectedConnection[] xConnArr, T[] xWeightArr,
            ConnectionIdArrays yIdArrays, T[] yWeightArr,
            int idx)
            where T : struct
        {
            return xConnArr[idx].SourceId == yIdArrays._sourceIdArr[idx] 
                &&  xConnArr[idx].TargetId == yIdArrays._targetIdArr[idx] 
                &&  xWeightArr[idx].Equals(yWeightArr[idx]);
        }

        public static void CompareConnectionLists<T>(ConnectionGenes<T> x,
                                                     ConnectionIdArrays yIdArrays,
                                                     T[] yWeightArr)
            where T : struct
        {
            Assert.AreEqual(x.Length, yIdArrays._sourceIdArr.Length);
            Assert.AreEqual(x.Length, yIdArrays._targetIdArr.Length);
            Assert.AreEqual(x.Length, yWeightArr.Length);

            for(int i=0; i<x.Length; i++)  {
                Assert.IsTrue(AreEqual(x, yIdArrays, yWeightArr, i));
            }
        }

        public static bool AreEqual<T>(ConnectionGenes<T> x,
                                       ConnectionIdArrays yIdArrays,
                                       T[] yWeightArr, int idx)
            where T : struct
        {
            return x._connArr[idx].SourceId == yIdArrays._sourceIdArr[idx] 
                &&  x._connArr[idx].TargetId == yIdArrays._targetIdArr[idx] 
                &&  x._weightArr[idx].Equals(yWeightArr[idx]);
        }

        #endregion
    }
}
