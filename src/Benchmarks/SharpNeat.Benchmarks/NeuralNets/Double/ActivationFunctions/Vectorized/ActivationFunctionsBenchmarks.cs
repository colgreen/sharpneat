using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Redzen.Numerics.Distributions.Double;

namespace SharpNeat.NeuralNets.ActivationFunctions.Vectorized;

/// <summary>
/// Double precision vectorized activation function benchmarks.
/// </summary>
public class ActivationFunctionsBenchmarks
{
    static readonly IActivationFunction<double> __LeakyReLU = new LeakyReLU<double>();
    static readonly IActivationFunction<double> __LeakyReLUShifted = new LeakyReLUShifted<double>();
    static readonly IActivationFunction<double> __MaxMinusOne = new MaxMinusOne<double>();
    static readonly IActivationFunction<double> __PolynomialApproximantSteep = new PolynomialApproximantSteep<double>();
    static readonly IActivationFunction<double> __QuadraticSigmoid = new QuadraticSigmoid<double>();
    static readonly IActivationFunction<double> __ReLU = new ReLU<double>();
    static readonly IActivationFunction<double> __SoftSignSteep = new SoftSignSteep<double>();
    static readonly IActivationFunction<double> __SReLU = new SReLU<double>();
    static readonly IActivationFunction<double> __SReLUShifted = new SReLUShifted<double>();

    const int __loops = 1000;
    readonly double[] _x = new double[1003];
    readonly double[] _w = new double[1003];

    public ActivationFunctionsBenchmarks()
    {
        // Create some random Gaussian values as the inputs to the activation functions.
        var gaussian = new ZigguratGaussianSampler(0.0, 2.0, 0);
        for(int i=0; i < _x.Length; i++)
            _x[i] = gaussian.Sample();
    }

    [Benchmark]
    public void LeakyReLU()
    {
        RunBenchmark(__LeakyReLU);
    }

    [Benchmark]
    public void LeakyReLUShifted()
    {
        RunBenchmark(__LeakyReLUShifted);
    }

    [Benchmark]
    public void MaxMinusOne()
    {
        RunBenchmark(__MaxMinusOne);
    }

    [Benchmark]
    public void PolynomialApproximantSteep()
    {
        RunBenchmark(__PolynomialApproximantSteep);
    }

    [Benchmark]
    public void QuadraticSigmoid()
    {
        RunBenchmark(__QuadraticSigmoid);
    }

    [Benchmark]
    public void ReLU()
    {
        RunBenchmark(__ReLU);
    }

    [Benchmark]
    public void SoftSignSteep()
    {
        RunBenchmark(__SoftSignSteep);
    }

    [Benchmark]
    public void SReLU()
    {
        RunBenchmark(__SReLU);
    }

    [Benchmark]
    public void SReLUShifted()
    {
        RunBenchmark(__SReLUShifted);
    }

    private void RunBenchmark(IActivationFunction<double> actFn)
    {
        ref double xref = ref MemoryMarshal.GetReference(_x.AsSpan());
        ref double wref = ref MemoryMarshal.GetReference(_w.AsSpan());
        int len = _x.Length;

        for(int i=0; i < __loops; i++)
            actFn.Fn(ref xref, ref wref, len);
    }
}
