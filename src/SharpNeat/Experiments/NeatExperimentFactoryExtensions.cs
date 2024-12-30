// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

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
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="experimentFactory">The experiment factory instance.</param>
    /// <param name="jsonConfigFilename">The name of a file from which experiment JSON configuration can be read.</param>
    /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
    public static INeatExperiment<TScalar> CreateExperiment<TScalar>(
        this INeatExperimentFactory experimentFactory,
        string jsonConfigFilename)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        using FileStream fs = File.OpenRead(jsonConfigFilename);
        return experimentFactory.CreateExperiment<TScalar>(fs);
    }
}
