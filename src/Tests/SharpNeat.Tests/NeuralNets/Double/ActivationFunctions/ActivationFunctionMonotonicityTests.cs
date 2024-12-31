using SharpNeat.Tests;
using Xunit;

namespace SharpNeat.NeuralNets.ActivationFunctions;

public class ActivationFunctionMonotonicityTests
{
    [Fact]
    public void TestMonotonicity()
    {
        AssertMonotonic(new ArcSinH<double>(), true);
        AssertMonotonic(new ArcTan<double>(), true);
        AssertMonotonic(new LeakyReLU<double>(), true);
        AssertMonotonic(new LeakyReLUShifted<double>(), true);
        AssertMonotonic(new Logistic<double>(), true);
        AssertMonotonic(new LogisticSteep<double>(), true);
        AssertMonotonic(new MaxMinusOne<double>(), false);
        AssertMonotonic(new PolynomialApproximantSteep<double>(), true);
        AssertMonotonic(new QuadraticSigmoid<double>(), false);
        AssertMonotonic(new ReLU<double>(), false);
        AssertMonotonic(new ScaledELU<double>(), true);
        AssertMonotonic(new SoftSignSteep<double>(), true);
        AssertMonotonic(new SReLU<double>(), true);
        AssertMonotonic(new SReLUShifted<double>(), true);
        AssertMonotonic(new TanH<double>(), true);

        AssertMonotonic(new Vectorized.LeakyReLU<double>(), true);
        AssertMonotonic(new Vectorized.LeakyReLUShifted<double>(), true);
        AssertMonotonic(new Vectorized.MaxMinusOne<double>(), false);
        AssertMonotonic(new Vectorized.PolynomialApproximantSteep<double>(), true);
        AssertMonotonic(new Vectorized.QuadraticSigmoid<double>(), false);
        AssertMonotonic(new Vectorized.ReLU<double>(), false);
        AssertMonotonic(new Vectorized.SoftSignSteep<double>(), true);
        AssertMonotonic(new Vectorized.SReLU<double>(), true);
        AssertMonotonic(new Vectorized.SReLUShifted<double>(), true);
    }

    #region Private Static Methods

    private static void AssertMonotonic(IActivationFunction<double> actFn, bool strict)
    {
        double fn(double x)
        {
            actFn.Fn(ref x);
            return x;
        }

        Assert.True(FuncTestUtils.IsMonotonicIncreasing(fn, -6, 6, 0.01, strict));
    }

    #endregion
}
