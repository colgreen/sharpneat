using BenchmarkDotNet.Attributes;

namespace SharpNeat.Tasks.FunctionRegression;

public class FuncRegressionUtilsBenchmarks
{
    #region Instance Fields

    const int __sampleCount = 100;
    readonly ParamSamplingInfo<double> _paramSamplingInfo = new(0, 2 * Math.PI, __sampleCount);
    readonly double[] _yArr = new double[__sampleCount];
    readonly double[] _gradientArr = new double[__sampleCount];

    #endregion

    #region Constructor

    public FuncRegressionUtilsBenchmarks()
    {
        var psi = new ParamSamplingInfo<double>(0, 2 * MathF.PI, __sampleCount);
        FuncRegressionUtils<double>.Probe((x) => Math.Sin(x), psi, _yArr);
    }

    #endregion

    #region Public Methods

    [Benchmark]
    public void CalcGradients()
    {
        FuncRegressionUtils<double>.CalcGradients(_paramSamplingInfo, _yArr, _gradientArr);
    }

    #endregion
}
