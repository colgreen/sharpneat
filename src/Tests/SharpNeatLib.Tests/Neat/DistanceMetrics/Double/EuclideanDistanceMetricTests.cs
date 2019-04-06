using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.DistanceMetrics.Double;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Tests.Neat.DistanceMetrics.Double
{
    [TestClass]
    public class EuclideanDistanceMetricTests
    {
        [TestMethod]
        [TestCategory("DistanceMetrics")]
        public void TestPythagoras()
        {
            var connGenes1 = new ConnectionGenes<double>(2);
            connGenes1[0] = (0, 3, 0.0);
            connGenes1[1] = (0, 4, 0.0);

            var connGenes2 = new ConnectionGenes<double>(2);
            connGenes2[0] = (0, 3, 3.0);
            connGenes2[1] = (0, 4, 4.0);

            var distanceMetric = new EuclideanDistanceMetric();;

            // GetDistance() tests.
            Assert.AreEqual(5.0, distanceMetric.CalcDistance(connGenes1, connGenes2));
            Assert.AreEqual(5.0, distanceMetric.CalcDistance(connGenes2, connGenes1));

            // TestDistance() tests.
            Assert.IsTrue(distanceMetric.TestDistance(connGenes1, connGenes2, 5.01));
            Assert.IsFalse(distanceMetric.TestDistance(connGenes1, connGenes2, 5.0));
            Assert.IsFalse(distanceMetric.TestDistance(connGenes1, connGenes2, 1.0));
            Assert.IsFalse(distanceMetric.TestDistance(connGenes1, connGenes2, 0.0));

            Assert.IsTrue(distanceMetric.TestDistance(connGenes2, connGenes1, 5.01));
            Assert.IsFalse(distanceMetric.TestDistance(connGenes2, connGenes1, 5.0));
            Assert.IsFalse(distanceMetric.TestDistance(connGenes2, connGenes1, 1.0));
            Assert.IsFalse(distanceMetric.TestDistance(connGenes2, connGenes1, 0.0));
        }

        [TestMethod]
        [TestCategory("DistanceMetrics")]
        public void TestMatchingGenomes()
        {
            var connGenes1 = new ConnectionGenes<double>(5);
            connGenes1[0] = (0, 10, 1.0);
            connGenes1[1] = (1, 11, 2.0);
            connGenes1[2] = (2, 12, 3.0);
            connGenes1[3] = (3, 13, 4.0);
            connGenes1[4] = (4, 14, 5.0);

            var connGenes2 = new ConnectionGenes<double>(5);
            connGenes2[0] = (0, 10, 1.0);
            connGenes2[1] = (1, 11, 2.0);
            connGenes2[2] = (2, 12, 3.0);
            connGenes2[3] = (3, 13, 4.0);
            connGenes2[4] = (4, 14, 5.0);

            var distanceMetric = new EuclideanDistanceMetric();;

            // GetDistance() tests.
            Assert.AreEqual(0.0, distanceMetric.CalcDistance(connGenes1, connGenes2));
            Assert.AreEqual(0.0, distanceMetric.CalcDistance(connGenes2, connGenes1));

            // TestDistance() tests.
            Assert.IsTrue(distanceMetric.TestDistance(connGenes1, connGenes2, 1.0));
            Assert.IsTrue(distanceMetric.TestDistance(connGenes1, connGenes2, 0.001));
            Assert.IsFalse(distanceMetric.TestDistance(connGenes1, connGenes2, 0.0));

            Assert.IsTrue(distanceMetric.TestDistance(connGenes2, connGenes1, 5.0));
            Assert.IsTrue(distanceMetric.TestDistance(connGenes2, connGenes1, 0.001));
            Assert.IsFalse(distanceMetric.TestDistance(connGenes2, connGenes1, 0.0));            
        }

        [TestMethod]
        [TestCategory("DistanceMetrics")]
        public void TestPartialMatchGenomes()
        {
            var connGenes1 = new ConnectionGenes<double>(5);
            connGenes1[0] = (0, 10, 1.0);
            connGenes1[1] = (1, 11, 2.0);
            connGenes1[2] = (2, 12, 3.0);
            connGenes1[3] = (3, 13, 4.0);
            connGenes1[4] = (4, 14, 5.0);

            var connGenes2 = new ConnectionGenes<double>(5);
            connGenes2[0] = (0, 10, 1.0);
            connGenes2[1] = (3, 13, 4.0);
            connGenes2[2] = (100, 11, 2.0);
            connGenes2[3] = (200, 12, 3.0);
            connGenes2[4] = (400, 14, 5.0);

            var distanceMetric = new EuclideanDistanceMetric();;

            // GetDistance() tests.
            Assert.AreEqual(Math.Sqrt(76), distanceMetric.CalcDistance(connGenes1, connGenes2));
            Assert.AreEqual(Math.Sqrt(76), distanceMetric.CalcDistance(connGenes2, connGenes1));

            // TestDistance() tests.
            Assert.IsTrue(distanceMetric.TestDistance(connGenes1, connGenes2, Math.Sqrt(76) + 0.001));
            Assert.IsFalse(distanceMetric.TestDistance(connGenes1, connGenes2, Math.Sqrt(76) - 0.001));
        }

        [TestMethod]
        [TestCategory("DistanceMetrics")]
        public void TestMismatchGenomes()
        {
            var connGenes1 = new ConnectionGenes<double>(5);
            connGenes1[0] = (0, 10, 1.0);
            connGenes1[1] = (1, 11, 2.0);
            connGenes1[2] = (2, 12, 3.0);
            connGenes1[3] = (3, 13, 4.0);
            connGenes1[4] = (4, 14, 5.0);

            var connGenes2 = new ConnectionGenes<double>(5);
            connGenes2[0] = (0, 100, 1.0);
            connGenes2[1] = (1, 110, 2.0);
            connGenes2[2] = (2, 120, 3.0);
            connGenes2[3] = (3, 130, 4.0);
            connGenes2[4] = (4, 140, 5.0);

            var distanceMetric = new EuclideanDistanceMetric();;

            // GetDistance() tests.
            Assert.AreEqual(Math.Sqrt(110), distanceMetric.CalcDistance(connGenes1, connGenes2));
            Assert.AreEqual(Math.Sqrt(110), distanceMetric.CalcDistance(connGenes2, connGenes1));

            // TestDistance() tests.
            Assert.IsTrue(distanceMetric.TestDistance(connGenes1, connGenes2, Math.Sqrt(110) + 0.001));
            Assert.IsFalse(distanceMetric.TestDistance(connGenes1, connGenes2, Math.Sqrt(110) - 0.001));
        }

        [TestMethod]
        [TestCategory("DistanceMetrics")]
        public void TestSingleMatchGenomes()
        {
            var connGenes1 = new ConnectionGenes<double>(5);
            connGenes1[0] = (0, 10, 1.0);
            connGenes1[1] = (1, 11, 2.0);
            connGenes1[2] = (2, 12, 3.0);
            connGenes1[3] = (3, 13, 4.0);
            connGenes1[4] = (4, 14, 5.0);

            var connGenes2 = new ConnectionGenes<double>(1);
            connGenes2[0] = (4, 14, 20.0);

            var distanceMetric = new EuclideanDistanceMetric();;

            // GetDistance() tests.
            Assert.AreEqual(Math.Sqrt(255), distanceMetric.CalcDistance(connGenes1, connGenes2));
            Assert.AreEqual(Math.Sqrt(255), distanceMetric.CalcDistance(connGenes2, connGenes1));

            // TestDistance() tests.
            Assert.IsTrue(distanceMetric.TestDistance(connGenes1, connGenes2, Math.Sqrt(255) + 0.001));
            Assert.IsFalse(distanceMetric.TestDistance(connGenes1, connGenes2, Math.Sqrt(255) - 0.001));
        }
    }
}
