using BenchmarkDotNet.Attributes;
using Redzen.Numerics;

namespace ActivationFnBenchmarks
{
    public class FunctionBenchmarks
    {
        const int __loops = 1000000;
        double[] _x = new double[1000];
        float[] _f = new float[1000];

        public FunctionBenchmarks()
        {
            // Create some random Gaussian values as the inputs to the activation functions.
            ZigguratGaussianSampler gaussian = new ZigguratGaussianSampler(0);
            for(int i=0; i<_x.Length; i++) 
            {
                _x[i] = gaussian.NextDouble(0, 2.0);
                _f[i] = (float)gaussian.NextDouble(0, 2.0);
            }
        }

        [Benchmark]
        public double LogisticFunctionSteepDouble()
        {
            double a = 0.0;
            for(int i=0; i<__loops; i++) {
                a = Functions.LogisticFunctionSteep(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double LogisticApproximantSteepDouble()
        {
            double a = 0.0;
            for (int i = 0; i < __loops; i++) {
                a = Functions.LogisticApproximantSteep(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double SoftSignDouble()
        {
            double a = 0.0;
            for (int i = 0; i < __loops; i++) {
                a = Functions.SoftSign(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double PolynomialApproximantDouble()
        {
            double a = 0.0;
            for (int i=0; i<__loops; i++) {
                a = Functions.PolynomialApproximant(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double QuadraticSigmoidDouble()
        {
            double a = 0.0;
            for (int i=0; i<__loops; i++) {
                a = Functions.QuadraticSigmoid(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double ReLUDouble()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.ReLU(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double LeakyReLUDouble()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.LeakyReLU(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double LeakyReLUShiftedDouble()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.LeakyReLUShifted(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double SReLUDouble()
        {
            double a = 0.0;
            for(int i=0; i<__loops; i++) {
                a = Functions.SReLU(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double SReLUShiftedDouble()
        {
            double a = 0.0;
            for (int i=0; i<__loops; i++) {
                a = Functions.SReLUShifted(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double ArcTanDouble()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.ArcTan(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double TanHDouble()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.TanH(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double ArcSinHDouble()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.ArcSinH(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double ScaledELUDouble()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.ScaledELU(_x[i % _x.Length]);
            }
            return a;
        }

        [Benchmark]
        public double MaxMinusOneDouble()
        {
            double a = 0.0;
            for (int i=0; i < __loops; i++) {
                a = Functions.MaxMinusOne(_x[i % _x.Length]);
            }
            return a;
        }


        [Benchmark]
        public void LogisticFunctionSteepFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.LogisticFunctionSteep(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void LogisticApproximantSteepFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.LogisticApproximantSteep(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void SoftSignFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.SoftSign(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void PolynomialApproximantFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.PolynomialApproximant(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void QuadraticSigmoidFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.QuadraticSigmoid(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void SReLUFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.SReLU(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void SReLUShiftedFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.SReLUShifted(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void ReLUFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.ReLU(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void LeakyReLUFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.LeakyReLU(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void LeakyReLUShiftedFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.LeakyReLUShifted(_f[i % _x.Length]);
            }
        }


        [Benchmark]
        public void ArcTanFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.ArcTanF(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void TanHFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.TanHF(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void ArcSinHFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.ArcSinHF(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void ScaledELUFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.ScaledELUF(_f[i % _x.Length]);
            }
        }

        [Benchmark]
        public void MaxMinusOneFloat()
        {
            for (int i = 0; i < __loops; i++)
            {
                FunctionsFloat.MaxMinusOneF(_f[i % _x.Length]);
            }
        }



    }
}
