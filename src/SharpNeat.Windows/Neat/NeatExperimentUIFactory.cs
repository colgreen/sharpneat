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
    /// <inheritdoc/>
    public IExperimentUI CreateExperimentUI(JsonElement configElem)
    {
        return new NeatExperimentUI();
    }
}
