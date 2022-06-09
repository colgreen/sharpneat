// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Text.Json;

namespace SharpNeat.Experiments;

/// <summary>
/// Represents a factory of <see cref="INeatExperiment{T}"/>.
/// </summary>
public interface INeatExperimentFactory
{
    /// <summary>
    /// Gets a unique human-readable ID for the experiment, e.g. 'binary-11-multiplexer'.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Creates a new instance of <see cref="INeatExperiment{T}"/> using the provided NEAT experiment
    /// configuration.
    /// </summary>
    /// <param name="jsonConfigStream">A stream from which experiment JSON configuration can be read.</param>
    /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
    INeatExperiment<double> CreateExperiment(Stream jsonConfigStream);

    /// <summary>
    ///  Creates a new instance of <see cref="INeatExperiment{T}"/> using the provided NEAT experiment
    /// configuration, and using single-precision floating-point number format for the
    /// genome and neural-net connection weights.
    /// </summary>
    /// <param name="jsonConfigStream">A stream from which experiment JSON configuration can be read.</param>
    /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
    INeatExperiment<float> CreateExperimentSinglePrecision(Stream jsonConfigStream);
}
