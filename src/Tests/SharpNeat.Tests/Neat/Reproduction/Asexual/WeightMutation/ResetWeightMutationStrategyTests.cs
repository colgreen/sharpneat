using Redzen.Numerics;
using Redzen.Random;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection.Tests;
using Xunit;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Tests;

public class ResetWeightMutationStrategyTests
{
    #region Test Methods

    [Fact]
    public void UniformReset()
    {
        double weightScale = 5.0;
        var strategy = ResetWeightMutationStrategy<double>.CreateUniformResetStrategy(
            new SelectAllStrategy(),
            weightScale);

        IRandomSource rng = RandomDefaults.CreateRandomSource(0);

        int iters = 10_000;
        double[] weightArr = new double[iters];
        for(int i=0; i < iters; i++)
            weightArr[i] = 123.0;

        strategy.Invoke(weightArr, rng);

        // Construct a histogram on the array of weights.
        HistogramData hist = NumericsUtils.BuildHistogramData(weightArr, 8);

        // We expect samples to be approximately evenly distributed over the histogram buckets.
        for(int i=0; i < hist.FrequencyArray.Length; i++)
        {
            Assert.True(hist.FrequencyArray[i] > (iters / 8) * 0.8);
        }

        // We expect min and max to be close to -weightScale and +weightScale respectively.
        double delta = weightScale - hist.Max;
        Assert.True(delta >= 0.0 && delta < 0.1);

        delta = weightScale + hist.Min;
        Assert.True(delta >= 0.0 && delta < 0.1);

        // Mean should be near to zero.
        TestMean(weightArr);
    }

    [Fact]
    public void GaussianReset()
    {
        var strategy = ResetWeightMutationStrategy<double>.CreateGaussianResetStrategy(
            new SelectAllStrategy(),
            1.0);

        IRandomSource rng = RandomDefaults.CreateRandomSource(0);

        int iters = 100_000;
        double[] weightArr = new double[iters];
        for(int i=0; i < iters; i++)
            weightArr[i] = 123.0;

        strategy.Invoke(weightArr, rng);

        // Construct a histogram on the array of weights.
        HistogramData hist = NumericsUtils.BuildHistogramData(weightArr, 8);

        // We expect min and max to be close to be about -4.5 and +4.5 respectively
        // (but they could be higher in magnitude, with no bound).
        Assert.True(hist.Max >= 3.8);
        Assert.True(hist.Min <= -3.8);

        TestMean(weightArr);
        TestStandardDeviation(weightArr);
    }

    #endregion

    #region Private Static Methods

    private static void TestMean(double[] sampleArr)
    {
        double mean = sampleArr.Average();
        Assert.True(Math.Abs(mean) < 0.1);
    }

    private static void TestStandardDeviation(double[] sampleArr)
    {
        double sqrSum = 0.0;
        for(int i=0; i < sampleArr.Length; i++)
        {
            double x = sampleArr[i];
            sqrSum += x*x;
        }

        double var = sqrSum / sampleArr.Length;
        double stdDev = Math.Sqrt(var);
        Assert.True(Math.Abs(stdDev-1.0) < 0.1);
    }

    #endregion
}
