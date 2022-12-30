// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Windows.Experiments;

namespace SharpNeat.Windows.Neat;

/// <summary>
/// Default implementation of <see cref="IExperimentUi"/> for NEAT and NEAT genomes."/>.
/// </summary>
public class NeatExperimentUi : IExperimentUi
{
    /// <inheritdoc/>
    public virtual GenomeControl CreateGenomeControl()
    {
        return new NeatGenomeControl();
    }

    /// <inheritdoc/>
    public virtual GenomeControl CreateTaskControl(
        bool enableHardwareAcceleratedNeuralNets)
    {
        return null;
    }
}
