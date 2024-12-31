using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Redzen.Numerics.Distributions.Float;

namespace SharpNeat.NeuralNets.ActivationFunctions;

/// <summary>
/// Double precision activation function benchmarks.
/// </summary>
public class ActivationFunctionsBenchmarks
{
    static readonly IActivationFunction<double> __ArcSinH = new ArcSinH<double>();
    static readonly IActivationFunction<double> __ArcTan = new ArcTan<double>();
    static readonly IActivationFunction<double> __LeakyReLU = new LeakyReLU<double>();
    static readonly IActivationFunction<double> __LeakyReLUShifted = new LeakyReLUShifted<double>();
    static readonly IActivationFunction<double> __Logistic = new Logistic<double>();
    static readonly IActivationFunction<double> __LogisticSteep = new LogisticSteep<double>();
    static readonly IActivationFunction<double> __MaxMinusOne = new MaxMinusOne<double>();
    static readonly IActivationFunction<double> __NullFn = new NullFn<double>();
    static readonly IActivationFunction<double> __PolynomialApproximantSteep = new PolynomialApproximantSteep<double>();
    static readonly IActivationFunction<double> __QuadraticSigmoid = new QuadraticSigmoid<double>();
    static readonly IActivationFunction<double> __ReLU = new ReLU<double>();
    static readonly IActivationFunction<double> __ScaledELU = new ScaledELU<double>();
    static readonly IActivationFunction<double> __SoftSignSteep = new SoftSignSteep<double>();
    static readonly IActivationFunction<double> __SReLU = new SReLU<double>();
    static readonly IActivationFunction<double> __SReLUShifted = new SReLUShifted<double>();
    static readonly IActivationFunction<double> __TanH = new TanH<double>();

    const int __loops = 1000;
    readonly double[] _x = new double[1003];
    readonly double[] _w = new double[1003];

    public ActivationFunctionsBenchmarks()
    {
        // Create some random Gaussian values as the inputs to the activation functions.
        var gaussian = new ZigguratGaussianSampler(0.0f, 2.0f, 0);
        for(int i=0; i < _x.Length; i++)
            _x[i] = gaussian.Sample();
    }

    [Benchmark]
    public void ArcSinH()
    {
        RunBenchmark(__ArcSinH);
    }

    [Benchmark]
    public void ArcTan()
    {
        RunBenchmark(__ArcTan);
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
    public void Logistic()
    {
        RunBenchmark(__Logistic);
    }

    [Benchmark]
    public void LogisticSteep()
    {
        RunBenchmark(__LogisticSteep);
    }

    [Benchmark]
    public void MaxMinusOne()
    {
        RunBenchmark(__MaxMinusOne);
    }

    [Benchmark]
    public void NullFn()
    {
        RunBenchmark(__NullFn);
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
    public void ScaledELU()
    {
        RunBenchmark(__ScaledELU);
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

    [Benchmark]
    public void TanH()
    {
        RunBenchmark(__TanH);
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
