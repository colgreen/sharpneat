using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using SharpNeat.Neat.DistanceMetrics.Double;
using SharpNeat.Neat.Speciation.GeneticKMeans;
using static SharpNeat.Tests.Neat.Speciation.SpeciationStrategyTestUtils;

namespace SharpNeat.Tests.Neat.Speciation.GeneticKMeans
{
    [TestClass]
    public class RegularizedGeneticKMeansSpeciationStrategyTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAll_Manhattan()
        {
            var rng = new XorShiftRandom(0);
            var distanceMetric = new ManhattanDistanceMetric();
            var speciationStrategy = new RegularizedGeneticKMeansSpeciationStrategy<double>(distanceMetric, 50, 0.1);

            TestSpeciateAll(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng, false);
            TestSpeciateAll(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng, false);
            TestSpeciateAll(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng, false);
        }

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAll_Euclidean()
        {
            var rng = new XorShiftRandom(1);
            var distanceMetric = new EuclideanDistanceMetric();
            var speciationStrategy = new RegularizedGeneticKMeansSpeciationStrategy<double>(distanceMetric, 50, 0.1);

            TestSpeciateAll(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng, false);
            TestSpeciateAll(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng, false);
            TestSpeciateAll(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng, false);
        }

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAdd_Manhattan()
        {
            var rng = new XorShiftRandom(2);
            var distanceMetric = new ManhattanDistanceMetric();
            var speciationStrategy = new RegularizedGeneticKMeansSpeciationStrategy<double>(distanceMetric, 50, 0.1);

            TestSpeciateAdd(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng, false);
            TestSpeciateAdd(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng, false);
            TestSpeciateAdd(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng, false);
        }

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAdd_Euclidean()
        {
            var rng = new XorShiftRandom(3);
            var distanceMetric = new EuclideanDistanceMetric();
            var speciationStrategy = new RegularizedGeneticKMeansSpeciationStrategy<double>(distanceMetric, 50, 0.1);

            TestSpeciateAdd(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng, false);
            TestSpeciateAdd(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng, false);
            TestSpeciateAdd(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng, false);
        }

        #endregion
    }
}
