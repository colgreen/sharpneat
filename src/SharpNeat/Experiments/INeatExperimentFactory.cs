/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Text.Json;

namespace SharpNeat.Experiments
{
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
}
