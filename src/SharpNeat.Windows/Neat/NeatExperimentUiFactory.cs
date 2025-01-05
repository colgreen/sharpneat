// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Experiments;
using SharpNeat.Windows.Experiments;

namespace SharpNeat.Windows.Neat;

/// <summary>
/// Default implementation of <see cref="IExperimentUiFactory"/> for NEAT and NEAT genomes."/>.
/// </summary>
public sealed class NeatExperimentUiFactory : IExperimentUiFactory
{
    /// <inheritdoc/>
    public IExperimentUi CreateExperimentUi(
        INeatExperiment<float> neatExperiment,
        Stream jsonConfigStream)
    {
        return new NeatExperimentUi();
    }
}
