// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Text.Json;

namespace SharpNeat.Experiments.Windows;

/// <summary>
/// Represents a factory of <see cref="IExperimentUI"/>.
/// </summary>
public interface IExperimentUIFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IExperimentUI"/> using the experiment
    /// configuration settings from the provided json object model.
    /// </summary>
    /// <param name="configElem">Experiment config in presented as json object model.</param>
    /// <returns>A new instance of <see cref="IExperimentUI{T}"/>.</returns>
    IExperimentUI CreateExperimentUI(JsonElement configElem);
}
