// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Text.Json;
using SharpNeat.Experiments.Windows;

namespace SharpNeat.Windows.Neat;

/// <summary>
/// Default implementation of <see cref="IExperimentUIFactory"/> for NEAT and NEAT genomes."/>.
/// </summary>
public class NeatExperimentUIFactory : IExperimentUIFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="NeatExperimentUI"/> using the experiment
    /// configuration settings from the provided json object model.
    /// </summary>
    /// <param name="configElem">Experiment config in presented as json object model.</param>
    /// <returns>A new instance of <see cref="IExperimentUI{T}"/>.</returns>
    public IExperimentUI CreateExperimentUI(JsonElement configElem)
    {
        return new NeatExperimentUI();
    }
}
