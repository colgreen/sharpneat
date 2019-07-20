using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using SharpNeat.Neat.DistanceMetrics.Double;
using SharpNeat.Neat.Speciation.GeneticKMeans.Parallelized;
using static SharpNeat.Tests.Neat.Speciation.SpeciationStrategyTestUtils;

namespace SharpNeat.Tests.Neat.Speciation.GeneticKMeans.Parallelized
{
    [TestClass]
    public class GeneticKMeansSpeciationStrategyTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAll_Manhattan()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);
            var distanceMetric = new ManhattanDistanceMetric();
            var speciationStrategy = new GeneticKMeansSpeciationStrategy<double>(distanceMetric, 50, 4);

            TestSpeciateAll(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng);
            TestSpeciateAll(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng);
            TestSpeciateAll(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng);
        }

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAll_Euclidean()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(1);
            var distanceMetric = new EuclideanDistanceMetric();
            var speciationStrategy = new GeneticKMeansSpeciationStrategy<double>(distanceMetric, 50, 4);

            TestSpeciateAll(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng);
            TestSpeciateAll(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng);
            TestSpeciateAll(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng);
        }

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAdd_Manhattan()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(2);
            var distanceMetric = new ManhattanDistanceMetric();
            var speciationStrategy = new GeneticKMeansSpeciationStrategy<double>(distanceMetric, 50, 4);

            TestSpeciateAdd(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng);
            TestSpeciateAdd(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng);
            TestSpeciateAdd(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng);
        }

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAdd_Euclidean()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(3);
            var distanceMetric = new EuclideanDistanceMetric();
            var speciationStrategy = new GeneticKMeansSpeciationStrategy<double>(distanceMetric, 50, 4);

            TestSpeciateAdd(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng);
            TestSpeciateAdd(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng);
            TestSpeciateAdd(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng);
        }

        #endregion
    }
}
