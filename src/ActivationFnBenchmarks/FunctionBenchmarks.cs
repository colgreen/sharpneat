using BenchmarkDotNet.Attributes;
using Redzen.Numerics;

namespace ActivationFnBenchmarks
{
    public class FunctionBenchmarks
    {
        const int __loops = 1000000;
        double[] _x = new double[1000];

        public FunctionBenchmarks()
        {
            // Create some random Gaussian values as the inputs to the activation functions.
            ZigguratGaussianSampler gaussian = new ZigguratGaussianSampler(0);
            for(int i=0; i<_x.Length; i++) {
                _x[i] = gaussian.NextDouble(0, 2.0);
            }
        }

        [Benchmark]
        public void LogisticFunctionSteep1M()
        {
            for(int i=0; i<__loops; i++) {
                Functions.LogisticFunctionSteep(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void LogisticApproximantSteep1M()
        {
            for (int i = 0; i < __loops; i++)
            {
                Functions.LogisticApproximantSteep(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void SoftSign1M()
        {
            for (int i = 0; i < __loops; i++)
            {
                Functions.SoftSign(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void PolynomialApproximant1M()
        {
            for(int i=0; i<__loops; i++) {
                Functions.PolynomialApproximant(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void QuadraticSigmoid1M()
        {
            for(int i=0; i<__loops; i++) {
                Functions.QuadraticSigmoid(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void LeakyReLU1M()
        {
            for (int i = 0; i < __loops; i++)
            {
                Functions.LeakyReLU(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void LeakyReLUShifted1M()
        {
            for (int i = 0; i < __loops; i++)
            {
                Functions.LeakyReLUShifted(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void SReLU1M()
        {
            for(int i=0; i<__loops; i++) {
                Functions.SReLU(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void SReLUShifted1M()
        {
            for(int i=0; i<__loops; i++) {
                Functions.SReLUShifted(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void ArcTan1M()
        {
            for (int i = 0; i < __loops; i++)
            {
                Functions.ArcTan(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void TanH1M()
        {
            for (int i = 0; i < __loops; i++)
            {
                Functions.TanH(_x[i % _x.Length]);
            }
        }
    }
}
