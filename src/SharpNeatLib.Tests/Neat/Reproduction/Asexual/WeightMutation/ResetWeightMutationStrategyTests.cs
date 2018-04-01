using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Numerics;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;
using SharpNeat.Tests.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeat.Tests.Neat.Reproduction.Asexual.WeightMutation
{
    [TestClass]
    public class ResetWeightMutationStrategyTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("ResetWeightMutationStrategy")]
        public void TestUniformReset()
        {
            double weightScale = 5.0;
            var strategy = ResetWeightMutationStrategy<double>.CreateUniformResetStrategy(
                new SelectAllStrategy(),
                weightScale);

            int iters = 10000;
            double[] weightArr = new double[iters];
            for(int i=0; i<iters; i++) {
                weightArr[i] = 123.0;
            }
            
            strategy.Invoke(weightArr);

            // Construct a histogram on the array of weights.
            HistogramData hist = NumericsUtils.BuildHistogramData(weightArr, 8);

            // We expect samples to be approximately evenly distributed over the histogram buckets.
            for(int i=0; i < hist.FrequencyArray.Length; i++) {
                Assert.IsTrue(hist.FrequencyArray[i] > (iters / 8) * 0.8);
            }            

            // We expect min and max to be close to -weightScale and +weightScale respectively.
            double delta = weightScale - hist.Max;
            Assert.IsTrue(delta >= 0.0 && delta < 0.1);

            delta = weightScale + hist.Min;
            Assert.IsTrue(delta >= 0.0 && delta < 0.1);

            // Mean should be near to zero.
            TestMean(weightArr);
        }

        [TestMethod]
        [TestCategory("ResetWeightMutationStrategy")]
        public void TestGaussianReset()
        {
            var strategy = ResetWeightMutationStrategy<double>.CreateGaussianResetStrategy(
                new SelectAllStrategy(),
                1.0);

            int iters = 100000;
            double[] weightArr = new double[iters];
            for(int i=0; i<iters; i++) {
                weightArr[i] = 123.0;
            }
            
            strategy.Invoke(weightArr);

            // Construct a histogram on the array of weights.
            HistogramData hist = NumericsUtils.BuildHistogramData(weightArr, 8);

            // We expect min and max to be close to be about -4.5 and +4.5 respectively 
            // (but they could be higher in magnitude, with no bound).
            Assert.IsTrue(hist.Max >= 3.8);
            Assert.IsTrue(hist.Min <= -3.8);

            TestMean(weightArr);
            TestStandardDeviation(weightArr);
        }

        #endregion

        #region Private Static Methods

        private static void TestMean(double[] sampleArr)
        {
            double mean = sampleArr.Average();
            Assert.IsTrue(Math.Abs(mean) < 0.1);
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
            Assert.IsTrue(Math.Abs(stdDev-1.0) < 0.1);
        }

        #endregion
    }
}
