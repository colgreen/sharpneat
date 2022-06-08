// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Experiments.Windows;

namespace SharpNeat.Windows.Neat;

/// <summary>
/// Default implementation of <see cref="IExperimentUI"/> for NEAT and NEAT genomes."/>.
/// </summary>
public class NeatExperimentUI : IExperimentUI
{
    /// <inheritdoc/>
    public GenomeControl CreateGenomeControl()
    {
        return new NeatGenomeControl();
    }

    /// <inheritdoc/>
    public GenomeControl CreateTaskControl()
    {
        return null;
    }
}
