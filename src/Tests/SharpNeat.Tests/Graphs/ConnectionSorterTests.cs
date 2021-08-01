using System;
using Redzen.Random;
using Xunit;

namespace SharpNeat.Graphs.Tests
{
    public class ConnectionSorterTests
    {
        #region Test Methods

        [Theory]
        [InlineData(99)]
        [InlineData(100)]
        [InlineData(101)]
        [InlineData(999)]
        [InlineData(1000)]
        [InlineData(1001)]
        [InlineData(1002)]
        [InlineData(1003)]
        public void TestConnectionSorter(int len)
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource((uint)len);

            // Create random connection ID arrays.
            ConnectionIdArrays connIdArrays = new(len);
            var srcIds = connIdArrays.GetSourceIdSpan();
            var tgtIds = connIdArrays.GetTargetIdSpan();
            InitRandomValues(connIdArrays.GetSourceIdSpan(), rng);
            InitRandomValues(connIdArrays.GetTargetIdSpan(), rng);

            // Assign each connection's weight to be the sum of the source and target IDs.
            // This allows us to check that the weights are sorted correctly, i.e. remain aligned with the correct source and target IDs.
            double[] weightArr = new double[len];
            for(int i=0; i < len; i++) {
                weightArr[i] = srcIds[i] + tgtIds[i];
            }

            // Sort the connections.
            ConnectionSorter<double>.Sort(connIdArrays, weightArr);

            // Test connections are correctly ordered.
            int srcIdPrev = srcIds[0];
            int tgtIdPrev = tgtIds[0];

            Assert.Equal(weightArr[0], (double)(srcIds[0] + tgtIds[0]));

            for(int i=1; i < len; i++)
            {
                Assert.True(Compare(srcIdPrev, tgtIdPrev, srcIds[i], tgtIds[i]) <= 0);
                Assert.Equal(weightArr[i], (double)(srcIds[i] + tgtIds[i]));

                srcIdPrev = srcIds[i];
                tgtIdPrev = tgtIds[i];
            }
        }

        #endregion

        #region Private Static Methods

        private void InitRandomValues(Span<int> span, IRandomSource rng)
        {
            for(int i=0; i < span.Length; i++) {
                span[i] = rng.Next();
            }
        }

        private static int Compare(int srcIdA, int tgtIdA, int srcIdB, int tgtIdB)
        {
            if(srcIdA < srcIdB) {
                return -1;
            }
            if(srcIdA > srcIdB) {
                return 1;
            }

            if(tgtIdA < tgtIdB) {
                return -1;
            }
            if(tgtIdA > tgtIdB) {
                return 1;
            }
            return 0;
        }

        #endregion
    }
}
