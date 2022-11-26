// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Windows.Experiments;

namespace SharpNeat.Windows.Neat;

/// <summary>
/// Default implementation of <see cref="IExperimentUIFactory"/> for NEAT and NEAT genomes."/>.
/// </summary>
public sealed class NeatExperimentUIFactory : IExperimentUIFactory
{
    /// <inheritdoc/>
    public IExperimentUI CreateExperimentUI(Stream jsonConfigStream)
    {
        return new NeatExperimentUI();
    }
}
