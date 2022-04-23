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
    /// Creates a new instance of <see cref="INeatExperiment{T}"/> using experiment configuration settings
    /// from the provided json object model.
    /// </summary>
    /// <param name="configElem">Experiment config in json form.</param>
    /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
    INeatExperiment<double> CreateExperiment(JsonElement configElem);

    /// <summary>
    /// Creates a new instance of <see cref="INeatExperiment{T}"/> using experiment configuration settings
    /// from the provided json object model, and using single-precision floating-point number format for the
    /// genome and neural-net connection weights.
    /// </summary>
    /// <param name="configElem">Experiment config in json form.</param>
    /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
    INeatExperiment<float> CreateExperimentSinglePrecision(JsonElement configElem);
}
