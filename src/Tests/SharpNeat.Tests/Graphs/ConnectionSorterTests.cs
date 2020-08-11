using Redzen.Random;
using Xunit;

namespace SharpNeat.Graphs.Tests
{
    public class ConnectionSorterTests
    {
        #region Test Methods

        [Fact]
        public void TestConnectionSorter()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);
            int len = 1000;

            // Create random connection ID arrays.
            int[] srcIdArr = CreateRandomConnectionIdArray(len, rng);
            int[] tgtIdArr = CreateRandomConnectionIdArray(len, rng);

            // Assign each connection's weight to be the sum of the source and target IDs.
            // This allows us to check that the weights are sorted correctly, i.e. remain aligned with the correct source and target IDs.
            double[] weightArr = new double[len];
            for(int i=0; i < len; i++) {
                weightArr[i] = srcIdArr[i] + tgtIdArr[i];
            }
            
            // Package up the source and target ID arrays.
            ConnectionIdArrays connIdArrays = new ConnectionIdArrays(srcIdArr, tgtIdArr);

            // Sort the connections.
            ConnectionSorter<double>.Sort(connIdArrays, weightArr);

            // Test connections are correctly ordered.
            int srcIdPrev = srcIdArr[0];
            int tgtIdPrev = tgtIdArr[0];

            Assert.Equal(weightArr[0], (double)(srcIdArr[0] + tgtIdArr[0]));

            for(int i=0; i < len; i++)
            {
                Assert.True(Compare(srcIdPrev, tgtIdPrev, srcIdArr[i], tgtIdArr[i]) <= 0);
                Assert.Equal(weightArr[i], (double)(srcIdArr[i] + tgtIdArr[i]));

                srcIdPrev = srcIdArr[i];
                tgtIdPrev = tgtIdArr[i];
            }
        }

        #endregion

        #region Private Static Methods

        private static int[] CreateRandomConnectionIdArray(int length, IRandomSource rng)
        {
            int[] arr = new int[length];
            for(int i=0; i < length; i++) {
                arr[i] = rng.Next(length);
            }
            return arr;
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
