// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Tasks.FunctionRegression;

/// <summary>
/// Enumeration of function IDs for the function regression task.
/// </summary>
public enum FunctionId
{
    /// <summary>
    /// Absolute value function.
    /// </summary>
    Abs,
    /// <summary>
    /// Logarithm function.
    /// </summary>
    Log,
    /// <summary>
    /// Sine function.
    /// </summary>
    Sin,
    /// <summary>
    /// The sum of two sine waves with different periods.
    /// </summary>
    BeatSinewave,
    /// <summary>
    /// Sin(x^2) function.
    /// </summary>
    SinXSquared,
    /// <summary>
    /// Custom waveform #1 function.
    /// </summary>
    Waveform1
}
