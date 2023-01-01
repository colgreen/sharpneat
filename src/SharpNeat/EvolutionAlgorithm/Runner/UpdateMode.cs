// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.EvolutionAlgorithm.Runner;

/// <summary>
/// Evolution algorithm update event modes.
/// </summary>
public enum UpdateMode
{
    /// <summary>
    /// Do not generate any update events.
    /// </summary>
    None,

    /// <summary>
    /// Generate an update event at regular time intervals.
    /// </summary>
    Timespan,

    /// <summary>
    /// Generate an update event at regular generation intervals. (Every N generations).
    /// </summary>
    Generational
}
