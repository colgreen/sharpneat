using BenchmarkDotNet.Attributes;
using Redzen.Numerics;
using SharpNeat.NeuralNets;

namespace SharpNeatLib.Benchmarks
{
    /// <summary>
    /// Double precision activation function benchmarks.
    /// </summary>
    public class BenchmarksDouble
    {
        #region Instance Fields

        const int __loops = 1000;
        double[] _x = new double[1000];
        double[] _w = new double[1000];

        #endregion

        #region Constructor

        public BenchmarksDouble()
        {
            // Create some random Gaussian values as the inputs to the activation functions.
            ZigguratGaussianSampler gaussian = new ZigguratGaussianSampler(0);
            for(int i=0; i<_x.Length; i++) {
                _x[i] = gaussian.NextDouble(0, 2.0);
            }
        }

        #endregion

        #region Public Methods

        [Benchmark]
        public void ArcSinH() {
            RunBenchmark(ActivationFunctionsDouble.ArcSinH);
        }

        [Benchmark]
        public void ArcTan() {
            RunBenchmark(ActivationFunctionsDouble.ArcTan);
        }

        [Benchmark]
        public void LeakyReLU() {
            RunBenchmark(ActivationFunctionsDouble.LeakyReLU);
        }

        [Benchmark]
        public void LeakyReLUShifted() {
            RunBenchmark(ActivationFunctionsDouble.LeakyReLUShifted);
        }

        [Benchmark]
        public void LogisticApproximantSteep() {
            RunBenchmark(ActivationFunctionsDouble.LogisticApproximantSteep);
        }

        [Benchmark]
        public void LogisticFunction() {
            RunBenchmark(ActivationFunctionsDouble.LogisticFunction);
        }

        [Benchmark]
        public void LogisticFunctionSteep() {
            RunBenchmark(ActivationFunctionsDouble.LogisticFunctionSteep);
        }

        [Benchmark]
        public void MaxMinusOne() {
            RunBenchmark(ActivationFunctionsDouble.MaxMinusOne);
        }

        [Benchmark]
        public void NullFn() {
            RunBenchmark(ActivationFunctionsDouble.NullFn);
        }

        [Benchmark]
        public void PolynomialApproximantSteep() {
            RunBenchmark(ActivationFunctionsDouble.PolynomialApproximantSteep);
        }

        [Benchmark]
        public void QuadraticSigmoid() {
            RunBenchmark(ActivationFunctionsDouble.QuadraticSigmoid);
        }

        [Benchmark]
        public void ReLU() {
            RunBenchmark(ActivationFunctionsDouble.ReLU);
        }

        [Benchmark]
        public void ScaledELU() {
            RunBenchmark(ActivationFunctionsDouble.ScaledELU);
        }

        [Benchmark]
        public void SoftSignSteep() {
            RunBenchmark(ActivationFunctionsDouble.SoftSignSteep);
        }

        [Benchmark]
        public void SReLU() {
            RunBenchmark(ActivationFunctionsDouble.SReLU);
        }

        [Benchmark]
        public void SReLUShifted() {
            RunBenchmark(ActivationFunctionsDouble.SReLUShifted);
        }

        [Benchmark]
        public void TanH() {
            RunBenchmark(ActivationFunctionsDouble.TanH);
        }

        #endregion

        #region Private Methods

        private void RunBenchmark(IActivationFunction<double> actFn)
        {
            VecFnSegment2<double> fn = actFn.Fn;
            double y = 0.0;
            for(int i=0; i<__loops; i++) 
            {
                fn(_x, _w, 0, _x.Length);
            }
        }

        #endregion
    }
}
