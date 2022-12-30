// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Windows.Experiments;

/// <summary>
/// Represents types that can create new User Interfaces (UIs) for visualisation of genomes and tasks.
/// </summary>
public interface IExperimentUi
{
    /// <summary>
    /// Create a new Windows.Forms UI control for direct visualisation of the genomes, i.e. the showing the
    /// neural net nodes and connections.
    /// </summary>
    /// <returns>
    /// A new instance of <see cref="GenomeControl"/>; or null if the experiment does not provide a genome control.
    /// </returns>
    GenomeControl CreateGenomeControl();

    /// <summary>
    /// Create a new Windows.Forms UI control for visualisation of tasks. E.g. for a prey capture task we might
    /// have a UI control that shows the prey capture world, and how the current best genome performs in that world.
    /// </summary>
    /// <param name="enableHardwareAcceleratedNeuralNets">Enable use of hardware accelerated neural network
    /// implementations, i.e. alternate implementations that use CPU SIMD/vector instructions.</param>
    /// <returns>
    /// A new instance of <see cref="GenomeControl"/>; or null if the experiment does not provide a task control.
    /// </returns>
    GenomeControl CreateTaskControl(
        bool enableHardwareAcceleratedNeuralNets);
}
