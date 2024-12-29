// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using Redzen;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.FunctionRegression;

/// <summary>
/// Evaluator for the Function Regression task.
///
/// One continuous valued input maps to one continuous valued output.
///
/// Evaluation consists of querying the provided black box for a number of distinct values over the range of the
/// continuous valued input, and comparing the black box response with the expected/correct response.
/// </summary>
/// <typeparam name="TScalar">Black box input/output data type.</typeparam>
public sealed class FuncRegressionEvaluator<TScalar> : IPhenomeEvaluator<IBlackBox<TScalar>>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    readonly ParamSamplingInfo<TScalar> _paramSamplingInfo;
    readonly TScalar _gradientMseWeight;
    readonly TScalar _yMseWeight;

    // Expected/correct response arrays.
    readonly TScalar[] _yArrTarget;
    readonly TScalar[] _gradientArrTarget;

    // Actual black box response arrays (reusable evaluator state).
    readonly TScalar[] _yArr;
    readonly TScalar[] _gradientArr;

    readonly IBlackBoxProbe<TScalar> _blackBoxProbe;

    #region Constructors

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="paramSamplingInfo">Parameter sampling info.</param>
    /// <param name="gradientMseWeight">Fitness weighting to apply to the gradient fitness score.</param>
    /// <param name="yArrTarget">Array of target y values (function output values).</param>
    /// <param name="gradientArrTarget">Array of target gradient values.</param>
    /// <param name="blackBoxProbe">Black box probe. For obtaining the y value response array from an instance of <see cref="IBlackBox{T}"/>.</param>
    internal FuncRegressionEvaluator(
        ParamSamplingInfo<TScalar> paramSamplingInfo,
        TScalar gradientMseWeight,
        TScalar[] yArrTarget,
        TScalar[] gradientArrTarget,
        IBlackBoxProbe<TScalar> blackBoxProbe)
    {
        _paramSamplingInfo = paramSamplingInfo;
        _gradientMseWeight = gradientMseWeight;
        _yMseWeight = TScalar.One - gradientMseWeight;

        _yArrTarget = yArrTarget;
        _gradientArrTarget = gradientArrTarget;

        // Alloc working arrays for receiving black box outputs.
        int sampleCount = _paramSamplingInfo.SampleResolution;
        _yArr = new TScalar[sampleCount];
        _gradientArr = new TScalar[sampleCount];

        _blackBoxProbe = blackBoxProbe;
    }

    #endregion

    #region Public Methods

    static readonly TScalar PointZeroTwo = TScalar.CreateChecked(0.02);
    static readonly TScalar Twenty = TScalar.CreateChecked(20.0);

    /// <summary>
    /// Evaluate the provided black box against the function regression task,
    /// and return its fitness score.
    /// </summary>
    /// <param name="box">The black box to evaluate.</param>
    /// <returns>A new instance of <see cref="FitnessInfo"/>.</returns>
    public FitnessInfo Evaluate(IBlackBox<TScalar> box)
    {
        // Probe the black box over the full range of the input parameter.
        _blackBoxProbe.Probe(box, _yArr);

        // Return a zero fitness if there are any NaN or Infinity values in the response array.
        // Notes.
        // Certain activation functions can result in 'run away' values in a cyclic neural net that is run for a large
        // number of cycles, e.g. ReLU doesn't have the natural limiting to the upper bound that the logistic function
        // S-curve has. Arguably such networks should use such an S-curve like activation function rather than 'fixing'
        // the problem here.
        // TODO: Optimisation candidate. Vectorize the search for the non-finite bit pattern.
        if(Array.Exists(_yArr, x => !TScalar.IsFinite(x)))
            return FitnessInfo.DefaultFitnessInfo;

        // Calc gradients.
        FuncRegressionUtils<TScalar>.CalcGradients(_paramSamplingInfo, _yArr, _gradientArr);

        // Calc y position mean squared error (MSE), and apply weighting.
        TScalar yMse = MathSpan.MeanSquaredDelta<TScalar>(_yArr, _yArrTarget);
        yMse *= _yMseWeight;

        // Calc gradient mean squared error.
        TScalar gradientMse = MathSpan.MeanSquaredDelta<TScalar>(_gradientArr, _gradientArrTarget);
        gradientMse *= _gradientMseWeight;

        // Calc fitness as the inverse of MSE (higher value is fitter).
        // Add a constant to avoid divide by zero, and to constrain the fitness range between bad and good solutions;
        // this allows the selection strategy to select solutions that are mediocre and therefore helps preserve diversity.
        TScalar fitness = Twenty / (yMse + gradientMse + PointZeroTwo);

        return new FitnessInfo(double.CreateSaturating(fitness));
    }

    #endregion
}
