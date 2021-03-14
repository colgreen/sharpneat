using System;
using SharpNeat.Tasks.FunctionRegression;
using Xunit;

namespace SharpNeat.Tasks.Tests.FunctionRegression
{
    public class FuncRegressionUtilsTests
    {
        #region Test Methods

        [Fact]
        public void CalcGradients()
        {
            const int sampleCount = 100;
            ParamSamplingInfo psi = new(0, 2 * Math.PI, sampleCount);
            double[] yArr = new double[sampleCount];
            FuncRegressionUtils.Probe((x) => Math.Sin(x), psi, yArr);

            // Calc gradients.
            double[] gradientArr = new double[sampleCount];
            FuncRegressionUtils.CalcGradients(psi, yArr, gradientArr);

            // Calc expected gradients (using simple non-vectorized logic).
            double[] gradientArrExpected = new double[sampleCount];
            CalcGradients_IndependentImpl(psi, yArr, gradientArrExpected);

            // Compare results.
            Assert.Equal(gradientArrExpected, gradientArr);
        }

        #endregion

        #region Private Static Methods

        private static void CalcGradients_IndependentImpl(
            ParamSamplingInfo paramSamplingInfo,
            double[] yArr,
            double[] gradientArr)
        {
            // Handle the end points as special cases.
            // First point.
            double[] xArr = paramSamplingInfo.XArr;
            gradientArr[0] = CalcGradient(xArr[0], yArr[0], xArr[1], yArr[1]);

            // Intermediate points.
            int i=1;
            for(; i < xArr.Length - 1; i++) {
                gradientArr[i] = CalcGradient(xArr[i - 1], yArr[i - 1], xArr[i + 1], yArr[i + 1]);
            }

            // Last point.
            gradientArr[i] = CalcGradient(xArr[i - 1], yArr[i - 1], xArr[i], yArr[i]);
        }

        private static double CalcGradient(double x1, double y1, double x2, double y2)
        {
            return (y2 - y1) / (x2 - x1);
        }

        #endregion
    }
}
