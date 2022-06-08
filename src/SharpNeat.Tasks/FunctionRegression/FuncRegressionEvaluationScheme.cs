// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.FunctionRegression;

/// <summary>
/// Evaluation scheme for the function regression task.
/// </summary>
public sealed class FuncRegressionEvaluationScheme : IBlackBoxEvaluationScheme<double>
{
    readonly ParamSamplingInfo _paramSamplingInfo;
    readonly double _gradientMseWeight;

    // Expected/correct response arrays.
    readonly double[] _yArrTarget;
    readonly double[] _gradientArrTarget;

    readonly IBlackBoxProbe _blackBoxProbe;

    #region Auto Properties [IBlackBoxEvaluationScheme]

    /// <inheritdoc/>
    public int InputCount => 2;

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
    public FuncRegressionEvaluationScheme(
        Func<double,double> fn,
        ParamSamplingInfo paramSamplingInfo,
        double gradientMseWeight)
    {
        _paramSamplingInfo = paramSamplingInfo;
        _gradientMseWeight = gradientMseWeight;

        // Alloc arrays.
        int sampleCount = _paramSamplingInfo.SampleResolution;
        _yArrTarget = new double[sampleCount];
        _gradientArrTarget = new double[sampleCount];

        // Predetermine target responses.
        FuncRegressionUtils.Probe(fn, paramSamplingInfo, _yArrTarget);
        FuncRegressionUtils.CalcGradients(paramSamplingInfo, _yArrTarget, _gradientArrTarget);

        // Create blackbox probe.
        _blackBoxProbe = CreateBlackBoxProbe(fn, paramSamplingInfo);
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public IPhenomeEvaluator<IBlackBox<double>> CreateEvaluator()
    {
        return new FuncRegressionEvaluator(_paramSamplingInfo, _gradientMseWeight, _yArrTarget, _gradientArrTarget, _blackBoxProbe);
    }

    /// <inheritdoc/>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return (fitnessInfo.PrimaryFitness >= 100_000.0);
    }

    #endregion

    #region Private Static Methods

    private static BlackBoxProbe CreateBlackBoxProbe(
        Func<double,double> fn,
        ParamSamplingInfo paramSamplingInfo)
    {
        // Determine the mid output value of the function (over the specified sample points) and a scaling factor
        // to apply the to neural network response for it to be able to recreate the function (because the neural net
        // output range is [0,1] when using the logistic function as the neuron activation function).
        FuncRegressionUtils.CalcFunctionMidAndScale(fn, paramSamplingInfo, out double mid, out double scale);

        return new BlackBoxProbe(paramSamplingInfo, mid, scale);
    }

    #endregion
}
