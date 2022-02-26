/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2022 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Text.Json;
using SharpNeat.Experiments;
using SharpNeat.NeuralNets;

namespace SharpNeat.Tasks.Xor;

/// <summary>
/// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the XOR task.
/// </summary>
public sealed class XorExperimentFactory : INeatExperimentFactory
{
    /// <summary>
    /// Gets a unique human-readable ID for the experiment.
    /// </summary>
    public string Id => "xor";

    /// <summary>
    /// Creates a new instance of <see cref="INeatExperiment{T}"/> using experiment configuration settings
    /// from the provided json object model.
    /// </summary>
    /// <param name="configElem">Experiment config in json form.</param>
    /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
    public INeatExperiment<double> CreateExperiment(JsonElement configElem)
    {
        // Create an evaluation scheme object for the XOR task.
        var evalScheme = new XorEvaluationScheme();

        // Create a NeatExperiment object with the evaluation scheme,
        // and assign some default settings (these can be overridden by config).
        var experiment = new NeatExperiment<double>(evalScheme, this.Id)
        {
            IsAcyclic = true,
            ActivationFnName = ActivationFunctionId.LeakyReLU.ToString()
        };

        // Read standard neat experiment json config and use it configure the experiment.
        NeatExperimentJsonReader<double>.Read(experiment, configElem);
        return experiment;
    }

    /// <summary>
    /// Creates a new instance of <see cref="INeatExperiment{T}"/> using experiment configuration settings
    /// from the provided json object model, and using single-precision floating-point number format for the
    /// genome and neural-net connection weights.
    /// </summary>
    /// <param name="configElem">Experiment config in json form.</param>
    /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
    public INeatExperiment<float> CreateExperimentSinglePrecision(JsonElement configElem)
    {
        throw new NotImplementedException();
    }
}
