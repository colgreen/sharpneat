// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Experiments;

/// <summary>
/// <see cref="INeatExperimentFactory"/> extension methods.
/// </summary>
public static class NeatExperimentFactoryExtensions
{
    /// <summary>
    /// Creates a new instance of <see cref="INeatExperiment{T}"/> using the provided NEAT experiment
    /// configuration.
    /// </summary>
    /// <param name="experimentFactory">The experiment factory instance.</param>
    /// <param name="jsonConfigFilename">The name of a file from which experiment JSON configuration can be read.</param>
    /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
    public static INeatExperiment<double> CreateExperiment(
        this INeatExperimentFactory experimentFactory,
        string jsonConfigFilename)
    {
        using FileStream fs = File.OpenRead(jsonConfigFilename);
        return experimentFactory.CreateExperiment(fs);
    }
}
