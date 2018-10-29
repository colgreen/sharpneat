using BenchmarkDotNet.Attributes;
using Redzen.Numerics.Distributions.Double;
using SharpNeat.NeuralNet;

namespace SharpNeatLib.Benchmarks
{
    /// <summary>
    /// Double precision vectorized activation function benchmarks.
    /// </summary>
    public class BenchmarksVectorizedDouble
    {
        #region Instance Fields

        const int __loops = 1000;
        double[] _x = new double[1000];
        double[] _w = new double[1000];

        #endregion

        #region Constructor

        public BenchmarksVectorizedDouble()
        {
            // Create some random Gaussian values as the inputs to the activation functions.
            var gaussian = new ZigguratGaussianSampler(0.0, 2.0, 0);
            for(int i=0; i<_x.Length; i++) {
                _x[i] = gaussian.Sample();
            }
        }

        #endregion

        #region Public Methods

        //[Benchmark]
        //public void ArcSinH() {
        //    RunBenchmark(ActivationFunctionsVectorizedDouble.ArcSinH);
        //}

        //[Benchmark]
        //public void ArcTan() {
        //    RunBenchmark(ActivationFunctionsVectorizedDouble.ArcTan);
        //}

        [Benchmark]
        public void LeakyReLU() {
            RunBenchmark(ActivationFunctionsVectorizedDouble.LeakyReLU);
        }

        [Benchmark]
        public void LeakyReLUShifted() {
            RunBenchmark(ActivationFunctionsVectorizedDouble.LeakyReLUShifted);
        }

        //[Benchmark]
        //public void LogisticApproximantSteep() {
        //    RunBenchmark(ActivationFunctionsVectorizedDouble.LogisticApproximantSteep);
        //}

        //[Benchmark]
        //public void Logistic() {
        //    RunBenchmark(ActivationFunctionsVectorizedDouble.Logistic);
        //}

        //[Benchmark]
        //public void LogisticSteep() {
        //    RunBenchmark(ActivationFunctionsVectorizedDouble.LogisticSteep);
        //}

        [Benchmark]
        public void MaxMinusOne() {
            RunBenchmark(ActivationFunctionsVectorizedDouble.MaxMinusOne);
        }

        //[Benchmark]
        //public void NullFn() {
        //    RunBenchmark(ActivationFunctionsVectorizedDouble.NullFn);
        //}

        [Benchmark]
        public void PolynomialApproximantSteep() {
            RunBenchmark(ActivationFunctionsVectorizedDouble.PolynomialApproximantSteep);
        }

        [Benchmark]
        public void QuadraticSigmoid() {
            RunBenchmark(ActivationFunctionsVectorizedDouble.QuadraticSigmoid);
        }

        [Benchmark]
        public void ReLU() {
            RunBenchmark(ActivationFunctionsVectorizedDouble.ReLU);
        }

        //[Benchmark]
        //public void ScaledELU() {
        //    RunBenchmark(ActivationFunctionsVectorizedDouble.ScaledELU);
        //}

        [Benchmark]
        public void SoftSignSteep() {
            RunBenchmark(ActivationFunctionsVectorizedDouble.SoftSignSteep);
        }

        [Benchmark]
        public void SReLU() {
            RunBenchmark(ActivationFunctionsVectorizedDouble.SReLU);
        }

        [Benchmark]
        public void SReLUShifted() {
            RunBenchmark(ActivationFunctionsVectorizedDouble.SReLUShifted);
        }

        //[Benchmark]
        //public void TanH() {
        //    RunBenchmark(ActivationFunctionsVectorizedDouble.TanH);
        //}

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
