using SharpNeat.Experiments.ConfigModels;

namespace SharpNeat.Tasks.PreyCapture.ConfigModels;

/// <summary>
/// Model type for prey capture custom config section.
/// </summary>
public class PreyCaptureCustomConfig
{
    /// <summary>
    /// Prey initial moves. The number of moves the prey is allowed to move before the agent can move.
    /// </summary>
    public int? PreyInitMoves { get; set; }

    /// <summary>
    /// Prey speed; in the interval [0, 1].
    /// </summary>
    public float? PreySpeed { get; set; }

    /// <summary>
    /// The sensor range of the agent.
    /// </summary>
    public float? SensorRange { get; set; }

    /// <summary>
    /// The maximum number of simulation timesteps to run without the agent capturing the prey.
    /// </summary>
    public int? MaxTimesteps { get; set; }

    /// <summary>
    /// The number of prey capture trials to run per evaluation.
    /// </summary>
    public int? TrialsPerEvaluation { get; set; }
}
