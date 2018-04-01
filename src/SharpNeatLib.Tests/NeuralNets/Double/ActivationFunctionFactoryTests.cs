using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.NeuralNet;

namespace SharpNeatLib.Tests.NeuralNets.Double
{
    [TestClass]
    public class ActivationFunctionFactoryTests
    {
        [TestMethod]
        [TestCategory("ActivationFunctionFactory")]
        public void Test()
        {
            var fact = new DefaultActivationFunctionFactory<double>(true);
            var actFn = fact.GetActivationFunction("ReLU");

            Assert.IsNotNull(actFn);
            Assert.AreEqual("SharpNeat.NeuralNets.Double.ActivationFunctions.ReLU", actFn.GetType().FullName);

            // Requesting the same activation function should yield the same instance.
            var actFn2 = fact.GetActivationFunction("ReLU");
            Assert.AreSame(actFn, actFn2);
        }

        [TestMethod]
        [TestCategory("ActivationFunctionFactory")]
        public void TestHardwareAccelerated()
        {
            if(!Vector.IsHardwareAccelerated) {
                Assert.Inconclusive("Hardware accelerations not available. Hardware acceleration is available on supporting CPUs only, and only for x64 builds with optimization enabled (i.e. release builds).");
            }

            var fact = new DefaultActivationFunctionFactory<double>();
            var actFn = fact.GetActivationFunction("ReLU");

            Assert.IsNotNull(actFn);
            Assert.AreEqual("SharpNeat.NeuralNets.Double.ActivationFunctions.Vectorized.ReLU", actFn.GetType().FullName);

            // Requesting the same activation function should yield the same instance.
            var actFn2 = fact.GetActivationFunction("ReLU");
            Assert.AreSame(actFn, actFn2);
        }


        [TestMethod]
        [TestCategory("ActivationFunctionFactory")]
        public void TestCppn()
        {
            var fact = new DefaultActivationFunctionFactory<double>(true);
            var actFn = fact.GetActivationFunction("Gaussian");

            Assert.IsNotNull(actFn);
            Assert.AreEqual("SharpNeat.NeuralNets.Double.ActivationFunctions.Cppn.Gaussian", actFn.GetType().FullName);

            // Requesting the same activation function should yield the same instance.
            var actFn2 = fact.GetActivationFunction("Gaussian");
            Assert.AreSame(actFn, actFn2);
        }
    }
}
