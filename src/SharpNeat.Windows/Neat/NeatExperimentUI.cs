// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Experiments.Windows;

namespace SharpNeat.Windows.Neat;

/// <summary>
/// Default implementation of <see cref="IExperimentUI"/> for NEAT and NEAT genomes."/>.
/// </summary>
public class NeatExperimentUI : IExperimentUI
{
    /// <summary>
    /// Create a new Windows.Forms UI control for direct visualisation of the NEAT genomes, i.e. the showing the
    /// neural net nodes and connections.
    /// </summary>
    /// <returns>A new instance of <see cref="GenomeControl"/>; or null if the experiment does not provide a genome control.</returns>
    public GenomeControl CreateGenomeControl()
    {
        return new NeatGenomeControl();
    }

    /// <summary>
    /// Create a new Windows.Forms UI control for visualisation of tasks. E.g. for a prey capture task we might
    /// have a UI control that shows the prey capture world, and how the current best genome performs in that world.
    /// </summary>
    /// <returns>A new instance of <see cref="GenomeControl"/>; or null if the experiment does not provide a task control.</returns>
    public GenomeControl CreateTaskControl()
    {
        return null;
    }
}
