using SharpNeat.Neat.Genome;
using Xunit;

namespace SharpNeat.Graphs.Tests
{
    public static class ConnectionCompareUtils
    {
        #region Public Static Methods [Compare - No Weights]

        public static void CompareConnectionLists(
            DirectedConnection[] xConnArr,
            in ConnectionIds yIds)
        {
            int xlen = xConnArr.Length;
            Assert.Equal(xlen, yIds.GetSourceIdSpan().Length);
            Assert.Equal(xlen, yIds.GetTargetIdSpan().Length);

            for(int i=0; i < xlen; i++)  {
                Assert.True(Equal(xConnArr, yIds, i, i));
            }
        }

        public static void CompareConnectionLists<T>(
            ConnectionGenes<T> x,
            in ConnectionIds yIds)
            where T : struct
        {
            Assert.Equal(x.Length, yIds.GetSourceIdSpan().Length);
            Assert.Equal(x.Length, yIds.GetTargetIdSpan().Length);

            for(int i=0; i < x.Length; i++)  {
                Assert.True(Equal(x, yIds, i, i));
            }
        }

        public static void CompareConnectionLists<T>(
            ConnectionGenes<T> x,
            in ConnectionIds yIds,
            int[] connectionIndexMap)
            where T : struct
        {
            Assert.Equal(x.Length, yIds.GetSourceIdSpan().Length);
            Assert.Equal(x.Length, yIds.GetTargetIdSpan().Length);

            for(int i=0; i < x.Length; i++)  {
                Assert.True(Equal(x, yIds, i, connectionIndexMap[i]));
            }
        }

        #endregion

        #region Public Static Methods [Compare - With Weights]

        public static void CompareConnectionLists<T>(
            DirectedConnection[] xConnArr, T[] xWeightArr,
            in ConnectionIds yIds, T[] yWeightArr)
            where T : struct
        {
            int xlen = xConnArr.Length;
            Assert.Equal(xlen, xWeightArr.Length);
            Assert.Equal(xlen, yIds.GetSourceIdSpan().Length);
            Assert.Equal(xlen, yIds.GetTargetIdSpan().Length);
            Assert.Equal(xlen, yWeightArr.Length);

            for(int i=0; i < xlen; i++)  {
                Assert.True(Equal(xConnArr, xWeightArr, yIds, yWeightArr, i, i));
            }
        }

        public static void CompareConnectionLists<T>(
            ConnectionGenes<T> x,
            in ConnectionIds yIds,
            T[] yWeightArr)
            where T : struct
        {
            Assert.Equal(x.Length, yIds.GetSourceIdSpan().Length);
            Assert.Equal(x.Length, yIds.GetTargetIdSpan().Length);
            Assert.Equal(x.Length, yWeightArr.Length);

            for(int i=0; i < x.Length; i++) {
                Assert.True(Equal(x, yIds, yWeightArr, i, i));
            }
        }

        #endregion

        #region Private Static Methods

        private static bool Equal<T>(
            ConnectionGenes<T> x,
            in ConnectionIds yIds,
            int xIdx, int yIdx)
            where T : struct
        {
            return Equal(
                x._connArr,
                yIds,
                xIdx, yIdx);
        }

        private static bool Equal(
            DirectedConnection[] xConnArr,
            in ConnectionIds yIds,
            int xIdx, int yIdx)
        {
            return xConnArr[xIdx].SourceId == yIds.GetSourceId(yIdx)
               &&  xConnArr[xIdx].TargetId == yIds.GetTargetId(yIdx);
        }

        private static bool Equal<T>(
            ConnectionGenes<T> x,
            in ConnectionIds yIds, T[] yWeightArr,
            int xIdx, int yIdx)
            where T : struct
        {
            return Equal(
                x._connArr, x._weightArr,
                yIds, yWeightArr,
                xIdx, yIdx);
        }

        private static bool Equal<T>(
            DirectedConnection[] xConnArr, T[] xWeightArr,
            in ConnectionIds yIds, T[] yWeightArr,
            int xIdx, int yIdx)
            where T : struct
        {
            return xConnArr[xIdx].SourceId == yIds.GetSourceId(yIdx)
               &&  xConnArr[xIdx].TargetId == yIds.GetTargetId(yIdx)
               &&  xWeightArr[xIdx].Equals(yWeightArr[yIdx]);
        }

        #endregion
    }
}
