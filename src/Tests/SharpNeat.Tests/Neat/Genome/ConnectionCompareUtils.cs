using SharpNeat.Neat.Genome;
using Xunit;

namespace SharpNeat.Graphs.Tests
{
    public static class ConnectionCompareUtils
    {
        #region Public Static Methods [Compare - No Weights]

        public static void CompareConnectionLists(
            DirectedConnection[] xConnArr,
            in ConnectionIdArrays yIdArrays)
        {
            int xlen = xConnArr.Length;
            Assert.Equal(xlen, yIdArrays._sourceIdArr.Length);
            Assert.Equal(xlen, yIdArrays._targetIdArr.Length);

            for(int i=0; i < xlen; i++)  {
                Assert.True(Equal(xConnArr, yIdArrays, i, i));
            }
        }

        public static void CompareConnectionLists<T>(
            ConnectionGenes<T> x,
            in ConnectionIdArrays yIdArrays)
            where T : struct
        {
            Assert.Equal(x.Length, yIdArrays._sourceIdArr.Length);
            Assert.Equal(x.Length, yIdArrays._targetIdArr.Length);

            for(int i=0; i < x.Length; i++)  {
                Assert.True(Equal(x, yIdArrays, i, i));
            }
        }

        public static void CompareConnectionLists<T>(
            ConnectionGenes<T> x,
            in ConnectionIdArrays yIdArrays,
            int[] connectionIndexMap)
            where T : struct
        {
            Assert.Equal(x.Length, yIdArrays._sourceIdArr.Length);
            Assert.Equal(x.Length, yIdArrays._targetIdArr.Length);

            for(int i=0; i < x.Length; i++)  {
                Assert.True(Equal(x, yIdArrays, i, connectionIndexMap[i]));
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
            Assert.Equal(xlen, xWeightArr.Length);
            Assert.Equal(xlen, yIdArrays._sourceIdArr.Length);
            Assert.Equal(xlen, yIdArrays._targetIdArr.Length);
            Assert.Equal(xlen, yWeightArr.Length);

            for(int i=0; i < xlen; i++)  {
                Assert.True(Equal(xConnArr, xWeightArr, yIdArrays, yWeightArr, i, i));
            }
        }

        public static void CompareConnectionLists<T>(
            ConnectionGenes<T> x,
            in ConnectionIdArrays yIdArrays,
            T[] yWeightArr)
            where T : struct
        {
            Assert.Equal(x.Length, yIdArrays._sourceIdArr.Length);
            Assert.Equal(x.Length, yIdArrays._targetIdArr.Length);
            Assert.Equal(x.Length, yWeightArr.Length);

            for(int i=0; i < x.Length; i++) {
                Assert.True(Equal(x, yIdArrays, yWeightArr, i, i));
            }
        }

        #endregion

        #region Private Static Methods

        private static bool Equal<T>(
            ConnectionGenes<T> x,
            in ConnectionIdArrays yIdArrays,
            int xIdx, int yIdx)
            where T : struct
        {
            return Equal(
                x._connArr,
                yIdArrays,
                xIdx, yIdx);
        }

        private static bool Equal(
            DirectedConnection[] xConnArr,
            in ConnectionIdArrays yIdArrays,
            int xIdx, int yIdx)
        {
            return xConnArr[xIdx].SourceId == yIdArrays._sourceIdArr[yIdx] 
               &&  xConnArr[xIdx].TargetId == yIdArrays._targetIdArr[yIdx];
        }

        private static bool Equal<T>(
            ConnectionGenes<T> x,
            in ConnectionIdArrays yIdArrays, T[] yWeightArr,
            int xIdx, int yIdx)
            where T : struct
        {
            return Equal(
                x._connArr, x._weightArr,
                yIdArrays, yWeightArr,
                xIdx, yIdx);
        }

        private static bool Equal<T>(
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
