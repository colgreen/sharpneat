// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Evaluation;
using SharpNeat.Tasks.FunctionRegression;

namespace SharpNeat.Tasks.GenerativeFunctionRegression;

/// <summary>
/// Evaluation scheme for the function regression task.
/// </summary>
/// <typeparam name="TScalar">Black box input/output data type.</typeparam>
public sealed class GenerativeFnRegressionEvaluationScheme<TScalar> : IBlackBoxEvaluationScheme<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    readonly ParamSamplingInfo<TScalar> _paramSamplingInfo;
    readonly TScalar _gradientMseWeight;

    // Expected/correct response arrays.
    readonly TScalar[] _yArrTarget;
    readonly TScalar[] _gradientArrTarget;

    readonly IBlackBoxProbe<TScalar> _blackBoxProbe;

    #region Auto Properties [IBlackBoxEvaluationScheme]

    /// <inheritdoc/>
    public int InputCount => 1;

    /// <inheritdoc/>
    public int OutputCount => 1;

    /// <inheritdoc/>
    public bool IsDeterministic => true;

    /// <inheritdoc/>
    public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

    /// <inheritdoc/>
    public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

    /// <inheritdoc/>
    public bool EvaluatorsHaveState => true;

    #endregion

    #region Constructors

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="fn">The target function.</param>
    /// <param name="paramSamplingInfo">Sampling (defines the x range and sampling density).</param>
    /// <param name="gradientMseWeight">The fitness weighting to assign to the gradient mean squared error (MSE) score.</param>
    public GenerativeFnRegressionEvaluationScheme(
        Func<TScalar, TScalar> fn,
        ParamSamplingInfo<TScalar> paramSamplingInfo,
        TScalar gradientMseWeight)
    {
        _paramSamplingInfo = paramSamplingInfo;
        _gradientMseWeight = gradientMseWeight;

        // Alloc arrays.
        int sampleCount = _paramSamplingInfo.SampleResolution;
        _yArrTarget = new TScalar[sampleCount];
        _gradientArrTarget = new TScalar[sampleCount];

        // Calculate the target responses (the expected/correct responses).
        FuncRegressionUtils<TScalar>.Probe(fn, paramSamplingInfo, _yArrTarget);
        FuncRegressionUtils<TScalar>.CalcGradients(paramSamplingInfo, _yArrTarget, _gradientArrTarget);

        // Create blackbox probe.
        _blackBoxProbe = CreateBlackBoxProbe(fn, paramSamplingInfo);
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public IPhenomeEvaluator<IBlackBox<TScalar>> CreateEvaluator()
    {
        return new FuncRegressionEvaluator<TScalar>(
            _paramSamplingInfo,
            _gradientMseWeight,
            _yArrTarget,
            _gradientArrTarget,
            _blackBoxProbe);
    }

    /// <inheritdoc/>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return (fitnessInfo.PrimaryFitness >= 100_000.0);
    }

    #endregion

    #region Private Static Methods

    private static GenerativeBlackBoxProbe<TScalar> CreateBlackBoxProbe(
        Func<TScalar, TScalar> fn,
        ParamSamplingInfo<TScalar> paramSamplingInfo)
    {
        // Determine the mid output value of the function (over the specified sample points) and a scaling factor
        // to apply the to neural network response for it to be able to recreate the function (because the neural net
        // output range is [0,1] when using the logistic function as the neuron activation function).
        FuncRegressionUtils<TScalar>.CalcFunctionMidAndScale(
            fn, paramSamplingInfo,
            out TScalar mid,
            out TScalar scale);

        return new GenerativeBlackBoxProbe<TScalar>(
            paramSamplingInfo.SampleResolution,
            mid, scale);
    }

    #endregion
}
