using FluentAssertions;
using Xunit;

namespace SharpNeat.NeuralNets.ActivationFunctions;

#pragma warning disable xUnit1025 // InlineData should be unique within the Theory it belongs to

public class ReLUTests
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(-0.0)]
    [InlineData(-0.000001)]
    [InlineData(+0.000001)]
    [InlineData(-0.1)]
    [InlineData(0.1)]
    [InlineData(-1.1)]
    [InlineData(1.1)]
    [InlineData(-1_000_000.0)]
    [InlineData(1_000_000.0)]
    [InlineData(double.Epsilon)]
    [InlineData(-double.Epsilon)]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void BitwiseReLUGivesCorrectResponses(double x)
    {
        // Arrange.
        var relu = new ReLU();

        // Act.
        double actual = x;
        relu.Fn(ref actual);

        // Assert.
        double expected = x < 0.0 ? 0.0 : x;
        actual.Should().Be(expected);
    }
}
