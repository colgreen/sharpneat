using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Redzen.Numerics.Distributions.Float;

namespace SharpNeat.NeuralNets.ActivationFunctions;

/// <summary>
/// Double precision activation function benchmarks.
/// </summary>
public class ActivationFunctionsBenchmarks
{
    static readonly IActivationFunction<float> __ArcSinH = new ArcSinH<float>();
    static readonly IActivationFunction<float> __ArcTan = new ArcTan<float>();
    static readonly IActivationFunction<float> __LeakyReLU = new LeakyReLU<float>();
    static readonly IActivationFunction<float> __LeakyReLUShifted = new LeakyReLUShifted<float>();
    static readonly IActivationFunction<float> __Logistic = new Logistic<float>();
    static readonly IActivationFunction<float> __LogisticSteep = new LogisticSteep<float>();
    static readonly IActivationFunction<float> __MaxMinusOne = new MaxMinusOne<float>();
    static readonly IActivationFunction<float> __NullFn = new NullFn<float>();
    static readonly IActivationFunction<float> __PolynomialApproximantSteep = new PolynomialApproximantSteep<float>();
    static readonly IActivationFunction<float> __QuadraticSigmoid = new QuadraticSigmoid<float>();
    static readonly IActivationFunction<float> __ReLU = new ReLU<float>();
    static readonly IActivationFunction<float> __ScaledELU = new ScaledELU<float>();
    static readonly IActivationFunction<float> __SoftSignSteep = new SoftSignSteep<float>();
    static readonly IActivationFunction<float> __SReLU = new SReLU<float>();
    static readonly IActivationFunction<float> __SReLUShifted = new SReLUShifted<float>();
    static readonly IActivationFunction<float> __TanH = new TanH<float>();

    const int __loops = 1000;
    readonly float[] _x = new float[1003];
    readonly float[] _w = new float[1003];

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

    private void RunBenchmark(IActivationFunction<float> actFn)
    {
        ref float xref = ref MemoryMarshal.GetReference(_x.AsSpan());
        ref float wref = ref MemoryMarshal.GetReference(_w.AsSpan());
        int len = _x.Length;

        for(int i=0; i < __loops; i++)
            actFn.Fn(ref xref, ref wref, len);
    }
}
