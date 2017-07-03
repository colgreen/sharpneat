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
        public double LogisticFunctionSteep1M()
        {
            double a = 0.0;
            for(int i=0; i<__loops; i++) {
                a = Functions.LogisticFunctionSteep(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double LogisticApproximantSteep1M()
        {
            double a = 0.0;
            for (int i = 0; i < __loops; i++) {
                a = Functions.LogisticApproximantSteep(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double SoftSign1M()
        {
            double a = 0.0;
            for (int i = 0; i < __loops; i++) {
                a = Functions.SoftSign(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double PolynomialApproximant1M()
        {
            double a = 0.0;
            for (int i=0; i<__loops; i++) {
                a = Functions.PolynomialApproximant(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double QuadraticSigmoid1M()
        {
            double a = 0.0;
            for (int i=0; i<__loops; i++) {
                a = Functions.QuadraticSigmoid(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double ReLU1M()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.ReLU(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double LeakyReLU1M()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.LeakyReLU(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double LeakyReLUShifted1M()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.LeakyReLUShifted(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double SReLU1M()
        {
            double a = 0.0;
            for(int i=0; i<__loops; i++) {
                a = Functions.SReLU(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double SReLUShifted1M()
        {
            double a = 0.0;
            for (int i=0; i<__loops; i++) {
                a = Functions.SReLUShifted(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double ArcTan1M()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.ArcTan(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double TanH1M()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.TanH(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double ArcSinH1M()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.ArcSinH(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double ScaledELU1M()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.ScaledELU(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double MaxMinusOnne1M()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.MaxMinusOne(_x[i % _x.Length]);
            }
            return a;
        }
    }
}
