using SharpNeat.NeuralNets.ActivationFunctions;
using Xunit;

namespace SharpNeat.NeuralNets.Double;

public class ActivationFunctionFactoryTests
{
    [Fact]
    public void GetActivationFunction()
    {
        var fact = new DefaultActivationFunctionFactory<double>(false);
        var actFn = fact.GetActivationFunction("ReLU");

        Assert.NotNull(actFn);
        Assert.Equal("SharpNeat.NeuralNets.ActivationFunctions.ReLU", GetNonGenericTypeName(actFn.GetType()));

        // Requesting the same activation function should yield the same instance.
        var actFn2 = fact.GetActivationFunction("ReLU");
        Assert.Same(actFn, actFn2);
    }

    [Fact]
    public void GetActivationFunction_Vectorized()
    {
        var fact = new DefaultActivationFunctionFactory<double>(true);
        var actFn = fact.GetActivationFunction("ReLU");

        Assert.NotNull(actFn);
        Assert.Equal("SharpNeat.NeuralNets.ActivationFunctions.Vectorized.ReLU", GetNonGenericTypeName(actFn.GetType()));

        // Requesting the same activation function should yield the same instance.
        var actFn2 = fact.GetActivationFunction("ReLU");
        Assert.Same(actFn, actFn2);
    }

    [Fact]
    public void GetActivationFunction_Cppn()
    {
        var fact = new DefaultActivationFunctionFactory<double>(false);
        var actFn = fact.GetActivationFunction("Gaussian");

        Assert.NotNull(actFn);
        Assert.Equal("SharpNeat.NeuralNets.ActivationFunctions.Cppn.Gaussian", GetNonGenericTypeName(actFn.GetType()));

        // Requesting the same activation function should yield the same instance.
        var actFn2 = fact.GetActivationFunction("Gaussian");
        Assert.Same(actFn, actFn2);
    }

    private static string GetNonGenericTypeName(Type type)
    {
        string name = type.FullName;
        int backtickIndex = name.IndexOf('`');
        if(backtickIndex > 0)
        {
            name = name.Substring(0, backtickIndex);
        }
        return name;
    }
}
