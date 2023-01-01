// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Tasks.PreyCapture.ConfigModels;

/// <summary>
/// Model type for prey capture custom config section.
/// </summary>
public sealed record PreyCaptureCustomConfig
{
    /// <summary>
    /// Prey initial moves. The number of moves the prey is allowed to move before the agent can move.
    /// </summary>
    public required int PreyInitMoves { get; init; }

    /// <summary>
    /// Prey speed; in the interval [0, 1].
    /// </summary>
    public required float PreySpeed { get; init; }

    /// <summary>
    /// The sensor range of the agent.
    /// </summary>
    public required float SensorRange { get; init; }

    /// <summary>
    /// The maximum number of simulation timesteps to run without the agent capturing the prey.
    /// </summary>
    public required int MaxTimesteps { get; init; }

    /// <summary>
    /// The number of prey capture trials to run per evaluation.
    /// </summary>
    public required int TrialsPerEvaluation { get; init; }
}
