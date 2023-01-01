// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.PreyCapture;

/// <summary>
/// Evaluation scheme for the prey capture task.
/// </summary>
public sealed class PreyCaptureEvaluationScheme : IBlackBoxEvaluationScheme<double>
{
    readonly int _preyInitMoves;
    readonly float _preySpeed;
    readonly float _sensorRange;
    readonly int _maxTimesteps;
    readonly int _trialsPerEvaluation;

    #region Properties

    /// <inheritdoc/>
    public int InputCount => 14;

    /// <inheritdoc/>
    public int OutputCount => 4;

    /// <inheritdoc/>
    public bool IsDeterministic => false;

    /// <inheritdoc/>
    public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

    /// <inheritdoc/>
    public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

    /// <inheritdoc/>
    public bool EvaluatorsHaveState => true;

    #endregion

    #region Constructor

    /// <summary>
    /// Construct with the provided task and evaluator parameters.
    /// </summary>
    /// <param name="preyInitMoves">Prey initial moves. The number of moves the prey is allowed to move before the agent can move.</param>
    /// <param name="preySpeed">Prey speed; in the interval [0, 1].</param>
    /// <param name="sensorRange">The sensor range of the agent.</param>
    /// <param name="maxTimesteps">The maximum number of simulation timesteps to run without the agent capturing the prey.</param>
    /// <param name="trialsPerEvaluation">The number of prey capture trials to run per evaluation.</param>
    public PreyCaptureEvaluationScheme(
        int preyInitMoves,
        float preySpeed,
        float sensorRange,
        int maxTimesteps,
        int trialsPerEvaluation)
    {
        _preyInitMoves = preyInitMoves;
        _preySpeed = preySpeed;
        _sensorRange = sensorRange;
        _maxTimesteps = maxTimesteps;
        _trialsPerEvaluation = trialsPerEvaluation;
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public IPhenomeEvaluator<IBlackBox<double>> CreateEvaluator()
    {
        return new PreyCaptureEvaluator(
            _preyInitMoves,
            _preySpeed,
            _sensorRange,
            _maxTimesteps,
            _trialsPerEvaluation);
    }

    /// <inheritdoc/>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return (fitnessInfo.PrimaryFitness >= _trialsPerEvaluation);
    }

    #endregion
}
