// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Experiments;

namespace SharpNeat.Windows.Experiments;

/// <summary>
/// Represents a factory of <see cref="IExperimentUi"/>.
/// </summary>
public interface IExperimentUiFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IExperimentUi"/> using the experiment
    /// configuration settings from the provided json object model.
    /// </summary>
    /// <param name="neatExperiment">A neat experiment instance.</param>
    /// <param name="jsonConfigStream">A stream from which experiment JSON configuration can be read.</param>
    /// <returns>A new instance of <see cref="IExperimentUi"/>.</returns>
    IExperimentUi CreateExperimentUi(
        INeatExperiment<float> neatExperiment,
        Stream jsonConfigStream);
}
