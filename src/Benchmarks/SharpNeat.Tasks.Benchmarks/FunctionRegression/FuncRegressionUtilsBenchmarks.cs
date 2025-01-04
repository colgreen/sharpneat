using BenchmarkDotNet.Attributes;

namespace SharpNeat.Tasks.FunctionRegression;

public class FuncRegressionUtilsBenchmarks
{
    const int __sampleCount = 100;
    readonly ParamSamplingInfo<float> _paramSamplingInfo = new(0, 2 * MathF.PI, __sampleCount);
    readonly float[] _yArr = new float[__sampleCount];
    readonly float[] _gradientArr = new float[__sampleCount];

    public FuncRegressionUtilsBenchmarks()
    {
        var psi = new ParamSamplingInfo<float>(0, 2 * MathF.PI, __sampleCount);
        FuncRegressionUtils<float>.Probe((x) => MathF.Sin(x), psi, _yArr);
    }

    [Benchmark]
    public void CalcGradients()
    {
        FuncRegressionUtils<float>.CalcGradients(_paramSamplingInfo, _yArr, _gradientArr);
    }
}
