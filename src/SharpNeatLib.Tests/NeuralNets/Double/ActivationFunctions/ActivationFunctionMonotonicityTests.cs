using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.NeuralNet;
using SharpNeat.NeuralNet.Double.ActivationFunctions;
using Vectorized = SharpNeat.NeuralNet.Double.ActivationFunctions.Vectorized;

namespace SharpNeatLib.Tests.NeuralNets.Double.ActivationFunctions
{
    [TestClass]
    public class ActivationFunctionMonotonicityTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("ActivationFunctions-Monotonicity")]
        public void TestMonotonicity()
        {
            AssertMonotonic(new ArcSinH(), true);
            AssertMonotonic(new ArcTan(), true);
            AssertMonotonic(new LeakyReLU(), true);
            AssertMonotonic(new LeakyReLUShifted(), true);
            AssertMonotonic(new LogisticApproximantSteep(), false);
            AssertMonotonic(new LogisticFunction(), true);
            AssertMonotonic(new LogisticFunctionSteep(), true);
            AssertMonotonic(new MaxMinusOne(), false);
            AssertMonotonic(new PolynomialApproximantSteep(), true);
            AssertMonotonic(new QuadraticSigmoid(), false);
            AssertMonotonic(new ReLU(), false);
            AssertMonotonic(new ScaledELU(), true);
            AssertMonotonic(new SoftSignSteep(), true);
            AssertMonotonic(new SReLU(), true);
            AssertMonotonic(new SReLUShifted(), true);
            AssertMonotonic(new TanH(), true);

            //AssertMonotonic(new Vectorized.ArcSinH(), true);
            //AssertMonotonic(new Vectorized.ArcTan(), true);
            AssertMonotonic(new Vectorized.LeakyReLU(), true);
            AssertMonotonic(new Vectorized.LeakyReLUShifted(), true);
            //AssertMonotonic(new Vectorized.LogisticApproximantSteep(), false);
            //AssertMonotonic(new Vectorized.LogisticFunction(), true);
            //AssertMonotonic(new Vectorized.LogisticFunctionSteep(), true);
            AssertMonotonic(new Vectorized.MaxMinusOne(), false);
            //AssertMonotonic(new Vectorized.PolynomialApproximantSteep(), true);
            //AssertMonotonic(new Vectorized.QuadraticSigmoid(), false);
            AssertMonotonic(new Vectorized.ReLU(), false);
            //AssertMonotonic(new Vectorized.ScaledELU(), true);
            AssertMonotonic(new Vectorized.SoftSignSteep(), true);
            AssertMonotonic(new Vectorized.SReLU(), true);
            AssertMonotonic(new Vectorized.SReLUShifted(), true);
            //AssertMonotonic(new Vectorized.TanH(), true);
        }

        #endregion

        #region Private Static Methods

        private static void AssertMonotonic(IActivationFunction<double> actFn, bool strict)
        {
            Assert.IsTrue(TestUtils.IsMonotonicIncreasing(actFn.Fn, -6, 6, 0.01, strict));
        }

        #endregion
    }
}
