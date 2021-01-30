using Redzen;
using SharpNeat.Neat.Genome;
using SharpNeat.Graphs;
using Xunit;
using SharpNeat.Tests;

namespace SharpNeat.Neat.DistanceMetrics.Double.Tests
{
    public class DistanceMetricsUtilsTests
    {
        [Fact]
        public void CalculateEuclideanCentroid()
        {
            // Init input gene arrays.
            var connGenes1 = new ConnectionGenes<double>(6);
            connGenes1[0] = (0, 1, 1.0);
            connGenes1[1] = (0, 2, 2.0);
            connGenes1[2] = (2, 2, 3.0);
            connGenes1[3] = (2, 4, 4.0);
            connGenes1[4] = (2, 5, 5.0);
            connGenes1[5] = (3, 0, 6.0);

            var connGenes2 = new ConnectionGenes<double>(8);
            connGenes2[0] = (0, 1, 10.0);
            connGenes2[1] = (0, 3, 20.0);
            connGenes2[2] = (2, 2, 30.0);
            connGenes2[3] = (2, 3, 40.0);
            connGenes2[4] = (2, 5, 50.0);
            connGenes2[5] = (2, 6, 60.0);
            connGenes2[6] = (3, 0, 70.0);
            connGenes2[7] = (4, 5, 80.0);

            var connGenes3 = new ConnectionGenes<double>(2);
            connGenes3[0] = (2, 5, 100.0);
            connGenes3[1] = (10, 20, 200.0);

            var arr = new ConnectionGenes<double>[] { connGenes1, connGenes2, connGenes3 };

            // Calc centroid.
            ConnectionGenes<double> centroid =  DistanceMetricUtils.CalculateEuclideanCentroid(arr);

            // Expected centroid.
            var expected = new ConnectionGenes<double>(11);
            expected[0] = (0, 1, 11 / 3.0);
            expected[1] = (0, 2, 2 / 3.0);
            expected[2] = (0, 3, 20 / 3.0);
            expected[3] = (2, 2, 33 / 3.0);
            expected[4] = (2, 3, 40 / 3.0);
            expected[5] = (2, 4, 4 / 3.0);
            expected[6] = (2, 5, 155 / 3.0);
            expected[7] = (2, 6, 60 / 3.0);
            expected[8] = (3, 0, 76 / 3.0);
            expected[9] = (4, 5, 80 / 3.0);
            expected[10] = (10, 20, 200 / 3.0);

            Assert.True(SpanUtils.Equal<DirectedConnection>(expected._connArr, centroid._connArr));
            Assert.True(ArrayTestUtils.ConponentwiseEqual(expected._weightArr, centroid._weightArr, 1e-6));
        }
    }
}
