using Xunit;

namespace SharpNeat.NeuralNets.Double.Tests
{
    public class ActivationFunctionFactoryTests
    {
        [Fact]
        public void GetActivationFunction()
        {
            var fact = new DefaultActivationFunctionFactory<double>(false);
            var actFn = fact.GetActivationFunction("ReLU");

            Assert.NotNull(actFn);
            Assert.Equal("SharpNeat.NeuralNets.Double.ActivationFunctions.ReLU", actFn.GetType().FullName);

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
            Assert.Equal("SharpNeat.NeuralNets.Double.ActivationFunctions.Vectorized.ReLU", actFn.GetType().FullName);

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
            Assert.Equal("SharpNeat.NeuralNets.Double.ActivationFunctions.Cppn.Gaussian", actFn.GetType().FullName);

            // Requesting the same activation function should yield the same instance.
            var actFn2 = fact.GetActivationFunction("Gaussian");
            Assert.Same(actFn, actFn2);
        }
    }
}
