// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Windows.Experiments;

/// <summary>
/// Represents a factory of <see cref="IExperimentUI"/>.
/// </summary>
public interface IExperimentUIFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IExperimentUI"/> using the experiment
    /// configuration settings from the provided json object model.
    /// </summary>
    /// <param name="jsonConfigStream">A stream from which experiment JSON configuration can be read.</param>
    /// <returns>A new instance of <see cref="IExperimentUI{T}"/>.</returns>
    IExperimentUI CreateExperimentUI(Stream jsonConfigStream);
}
