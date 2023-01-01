using Redzen.Random;
using SharpNeat.Neat.DistanceMetrics.Double;
using Xunit;
using static SharpNeat.Neat.Speciation.Tests.SpeciationStrategyTestUtils;

namespace SharpNeat.Neat.Speciation.GeneticKMeans.Tests;

public class GeneticKMeansSpeciationStrategyTests
{
    [Fact]
    public void SpeciateAll_Manhattan()
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource(0);
        var distanceMetric = new ManhattanDistanceMetric();
        var speciationStrategy = new GeneticKMeansSpeciationStrategy<double>(distanceMetric, 50);

        TestSpeciateAll(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng);
        TestSpeciateAll(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng);
        TestSpeciateAll(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng);
    }

    [Fact]
    public void SpeciateAll_Euclidean()
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource(1);
        var distanceMetric = new EuclideanDistanceMetric();
        var speciationStrategy = new GeneticKMeansSpeciationStrategy<double>(distanceMetric, 50);

        TestSpeciateAll(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng);
        TestSpeciateAll(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng);
        TestSpeciateAll(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng);
    }

    [Fact]
    public void SpeciateAdd_Manhattan()
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource(2);
        var distanceMetric = new ManhattanDistanceMetric();
        var speciationStrategy = new GeneticKMeansSpeciationStrategy<double>(distanceMetric, 50);

        TestSpeciateAdd(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng);
        TestSpeciateAdd(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng);
        TestSpeciateAdd(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng);
    }

    [Fact]
    public void SpeciateAdd_Euclidean()
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource(3);
        var distanceMetric = new EuclideanDistanceMetric();
        var speciationStrategy = new GeneticKMeansSpeciationStrategy<double>(distanceMetric, 50);

        TestSpeciateAdd(100, 3, 2, 0.5, distanceMetric, speciationStrategy, rng);
        TestSpeciateAdd(100, 10, 10, 0.2, distanceMetric, speciationStrategy, rng);
        TestSpeciateAdd(100, 30, 10, 0.1, distanceMetric, speciationStrategy, rng);
    }
}
