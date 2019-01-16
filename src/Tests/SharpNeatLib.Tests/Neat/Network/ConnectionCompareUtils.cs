using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeat.Tests.Neat.Network
{
    public static class ConnectionCompareUtils
    {
        #region Public Static Methods [Compare - No Weights]

        public static void CompareConnectionLists(
            DirectedConnection[] xConnArr,
            in ConnectionIdArrays yIdArrays)
        {
            int xlen = xConnArr.Length;
            Assert.AreEqual(xlen, yIdArrays._sourceIdArr.Length);
            Assert.AreEqual(xlen, yIdArrays._targetIdArr.Length);

            for(int i=0; i < xlen; i++)  {
                Assert.IsTrue(AreEqual(xConnArr, yIdArrays, i, i));
            }
        }

        public static void CompareConnectionLists<T>(
            ConnectionGenes<T> x,
            in ConnectionIdArrays yIdArrays)
            where T : struct
        {
            Assert.AreEqual(x.Length, yIdArrays._sourceIdArr.Length);
            Assert.AreEqual(x.Length, yIdArrays._targetIdArr.Length);

            for(int i=0; i < x.Length; i++)  {
                Assert.IsTrue(AreEqual(x, yIdArrays, i, i));
            }
        }

        public static void CompareConnectionLists<T>(
            ConnectionGenes<T> x,
            in ConnectionIdArrays yIdArrays,
            int[] connectionIndexMap)
            where T : struct
        {
            Assert.AreEqual(x.Length, yIdArrays._sourceIdArr.Length);
            Assert.AreEqual(x.Length, yIdArrays._targetIdArr.Length);

            for(int i=0; i < x.Length; i++)  {
                Assert.IsTrue(AreEqual(x, yIdArrays, i, connectionIndexMap[i]));
            }
        }

        #endregion

        #region Public Static Methods [Compare - With Weights]

        public static void CompareConnectionLists<T>(
            DirectedConnection[] xConnArr, T[] xWeightArr,
            in ConnectionIdArrays yIdArrays, T[] yWeightArr)
            where T : struct
        {
            int xlen = xConnArr.Length;
            Assert.AreEqual(xlen, xWeightArr.Length);
            Assert.AreEqual(xlen, yIdArrays._sourceIdArr.Length);
            Assert.AreEqual(xlen, yIdArrays._targetIdArr.Length);
            Assert.AreEqual(xlen, yWeightArr.Length);

            for(int i=0; i < xlen; i++)  {
                Assert.IsTrue(AreEqual(xConnArr, xWeightArr, yIdArrays, yWeightArr, i, i));
            }
        }

        public static void CompareConnectionLists<T>(
            ConnectionGenes<T> x,
            in ConnectionIdArrays yIdArrays,
            T[] yWeightArr)
            where T : struct
        {
            Assert.AreEqual(x.Length, yIdArrays._sourceIdArr.Length);
            Assert.AreEqual(x.Length, yIdArrays._targetIdArr.Length);
            Assert.AreEqual(x.Length, yWeightArr.Length);

            for(int i=0; i < x.Length; i++) {
                Assert.IsTrue(AreEqual(x, yIdArrays, yWeightArr, i, i));
            }
        }

        #endregion

        #region Private Static Methods

        private static bool AreEqual<T>(
            ConnectionGenes<T> x,
            in ConnectionIdArrays yIdArrays,
            int xIdx, int yIdx)
            where T : struct
        {
            return AreEqual(
                x._connArr,
                yIdArrays,
                xIdx, yIdx);
        }

        private static bool AreEqual(
            DirectedConnection[] xConnArr,
            in ConnectionIdArrays yIdArrays,
            int xIdx, int yIdx)
        {
            return xConnArr[xIdx].SourceId == yIdArrays._sourceIdArr[yIdx] 
               &&  xConnArr[xIdx].TargetId == yIdArrays._targetIdArr[yIdx];
        }

        private static bool AreEqual<T>(
            ConnectionGenes<T> x,
            in ConnectionIdArrays yIdArrays, T[] yWeightArr,
            int xIdx, int yIdx)
            where T : struct
        {
            return AreEqual(
                x._connArr, x._weightArr,
                yIdArrays, yWeightArr,
                xIdx, yIdx);
        }

        private static bool AreEqual<T>(
            DirectedConnection[] xConnArr, T[] xWeightArr,
            in ConnectionIdArrays yIdArrays, T[] yWeightArr,
            int xIdx, int yIdx)
            where T : struct
        {
            return xConnArr[xIdx].SourceId == yIdArrays._sourceIdArr[yIdx] 
               &&  xConnArr[xIdx].TargetId == yIdArrays._targetIdArr[yIdx] 
               &&  xWeightArr[xIdx].Equals(yWeightArr[yIdx]);
        }

        #endregion
    }
}
