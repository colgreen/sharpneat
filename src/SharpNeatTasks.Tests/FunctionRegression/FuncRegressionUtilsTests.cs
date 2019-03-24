using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Tasks.FunctionRegression;
using SharpNeat.Tests;

namespace SharpNeat.Tasks.Tests.FunctionRegression
{

    [TestClass]
    public class FuncRegressionUtilsTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("FuncRegressionUtils")]
        public void TestCalcGradients()
        {
            const int sampleCount = 100;
            ParamSamplingInfo psi = new ParamSamplingInfo(0, 2 * Math.PI, sampleCount);
            double[] yArr = new double[sampleCount];
            FuncRegressionUtils.Probe((x) => Math.Sin(x), psi, yArr);

            // Calc gradients.
            double[] gradientArr = new double[sampleCount];    
            FuncRegressionUtils.CalcGradients(psi, yArr, gradientArr);

            // Calc expected gradients (using simple non-vectorized logic).
            double[] gradientArrExpected = new double[sampleCount]; 
            CalcGradients(psi, yArr, gradientArrExpected);

            // Compare results.
            ArrayTestUtils.Compare(gradientArrExpected, gradientArr);
        }

        #endregion

        #region Private Static Methods

        private static void CalcGradients(
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
