using BenchmarkDotNet.Attributes;
using Redzen.Random.Double;

namespace ActivationFnBenchmarks
{
    public class FunctionFloatBenchmarks
    {
        const int __loops = 1000000;
        float[] _x = new float[1000];

        public FunctionFloatBenchmarks()
        {
            // Create some random Gaussian values as the inputs to the activation functions.
            ZigguratGaussianDistribution gaussian = new ZigguratGaussianDistribution(0);
            for(int i=0; i<_x.Length; i++) {
                _x[i] = (float)gaussian.Sample(0, 2.0);
            }
        }

        [Benchmark]
        public void LogisticFunctionSteep()
        {
            for(int i=0; i<__loops; i++) {
                FunctionsFloat.LogisticFunctionSteep(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void LogisticApproximantSteep()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.LogisticApproximantSteep(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void SoftSign()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.SoftSign(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void PolynomialApproximant()
        {
            for(int i=0; i<__loops; i++) {
                FunctionsFloat.PolynomialApproximant(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void QuadraticSigmoid()
        {
            for(int i=0; i<__loops; i++) {
                FunctionsFloat.QuadraticSigmoid(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void SReLU()
        {
            for(int i=0; i<__loops; i++) {
                FunctionsFloat.SReLU(_x[i % _x.Length]);
            }
        }

        [Benchmark]
        public void SReLUShifted()
        {
            for(int i=0; i<__loops; i++) {
                FunctionsFloat.SReLUShifted(_x[i % _x.Length]);
            }
        }
    }
}
