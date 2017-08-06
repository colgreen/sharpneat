using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.NeuralNets;
using SharpNeat.NeuralNets.Double.ActivationFunctions;

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
