// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.PreyCapture;

/// <summary>
/// Evaluator for the prey capture task.
/// </summary>
public sealed class PreyCaptureEvaluator : IPhenomeEvaluator<IBlackBox<double>>
{
    readonly PreyCaptureWorld _world;
    readonly int _trialsPerEvaluation;

    #region Construction

    /// <summary>
    /// Construct with the provided task and evaluator parameters.
    /// </summary>
    /// <param name="preyInitMoves">Prey initial moves. The number of moves the prey is allowed to move before the agent can move.</param>
    /// <param name="preySpeed">Prey speed; in the interval [0, 1].</param>
    /// <param name="sensorRange">The sensor range of the agent.</param>
    /// <param name="maxTimesteps">The maximum number of simulation timesteps to run without the agent capturing the prey.</param>
    /// <param name="trialsPerEvaluation">The number of prey capture trials to run per evaluation.</param>
    public PreyCaptureEvaluator(
        int preyInitMoves,
        float preySpeed,
        float sensorRange,
        int maxTimesteps,
        int trialsPerEvaluation)
    {
        // Construct a re-usable instance of the prey capture world.
        _world = new PreyCaptureWorld(preyInitMoves, preySpeed, sensorRange, maxTimesteps);
        _trialsPerEvaluation = trialsPerEvaluation;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Evaluate the provided black box against the prey capture task,
    /// and return its fitness score.
    /// </summary>
    /// <param name="box">The black box to evaluate.</param>
    /// <returns>A new instance of <see cref="FitnessInfo"/>.</returns>
    public FitnessInfo Evaluate(IBlackBox<double> box)
    {
        // Perform multiple independent trials.
        int fitness = 0;
        for(int i=0; i < _trialsPerEvaluation; i++)
        {
            // TODO: Change RunTrial() to return 0 or 1, so that we can sum the result without performing a conditional branch.
            // Run a single trial, and record if the prey was captured.
            if(_world.RunTrial(box))
                fitness++;
        }

        // Fitness is given by the number of trials in which the agent caught the prey.
        return new FitnessInfo(fitness);
    }

    #endregion
}
