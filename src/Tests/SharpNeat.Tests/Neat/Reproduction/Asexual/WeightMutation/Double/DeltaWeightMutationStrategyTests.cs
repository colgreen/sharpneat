using Redzen.Numerics;
using Redzen.Random;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection.Tests;
using Xunit;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Double.Tests;

public class DeltaWeightMutationStrategyTests
{
    [Fact]
    public void UniformDelta()
    {
        double weightScale = 5.0;
        var strategy = DeltaWeightMutationStrategy.CreateUniformDeltaStrategy(
            new SelectAllStrategy(),
            weightScale);

        IRandomSource rng = RandomDefaults.CreateRandomSource(0);

        int iters = 10_000;
        double[] weightArr = new double[iters];
        for(int i=0; i < iters; i++)
            weightArr[i] = 1000.0;

        strategy.Invoke(weightArr, rng);

        // Construct a histogram on the array of weights.
        HistogramData hist = NumericsUtils.BuildHistogramData(weightArr, 8);

        // We expect samples to be approximately evenly distributed over the histogram buckets.
        for(int i=0; i < hist.FrequencyArray.Length; i++)
        {
            Assert.True(hist.FrequencyArray[i] > (iters / 8) * 0.8);
        }

        // We expect min and max to be close to 1000-weightScale and 1000+weightScale respectively.
        Assert.True(hist.Max <= (1000 + weightScale) && hist.Max > (1000 + weightScale)-0.1);
        Assert.True(hist.Min >= (1000 - weightScale) && hist.Min < (1000 - weightScale)+0.1);
    }

    [Fact]
    public void GaussianDelta()
    {
        var strategy = DeltaWeightMutationStrategy.CreateGaussianDeltaStrategy(
            new SelectAllStrategy(),
            1.0);

        IRandomSource rng = RandomDefaults.CreateRandomSource(0);

        const int iters = 100_000;
        double[] weightArr = new double[iters];
        for(int i=0; i < iters; i++)
            weightArr[i] = 1000.0;

        strategy.Invoke(weightArr, rng);

        // Construct a histogram on the array of weights.
        HistogramData hist = NumericsUtils.BuildHistogramData(weightArr, 8);

        // We expect min and max to be close to be about -995.5 and +1004.5 respectively
        // (but they could be further from the mean of 1000, with no bound).
        Assert.True(hist.Max >= 1002.0);
        Assert.True(hist.Min <= 998.0);

        TestMean(weightArr, 1000.0);
        TestStandardDeviation(weightArr);
    }

    #region Private Static Methods

    private static void TestMean(double[] sampleArr, double expectedMean)
    {
        double mean = sampleArr.Average();
        Assert.True(Math.Abs(mean) - expectedMean < 0.1);
    }

    private static void TestStandardDeviation(double[] sampleArr)
    {
        double mean = sampleArr.Average();

        double sqrSum = 0.0;
        for(int i=0; i < sampleArr.Length; i++)
        {
            double x = sampleArr[i] - mean;
            sqrSum += x*x;
        }

        double var = sqrSum / sampleArr.Length;
        double stdDev = Math.Sqrt(var);
        Assert.True(Math.Abs(stdDev - 1.0) < 0.1);
    }

    #endregion
}
