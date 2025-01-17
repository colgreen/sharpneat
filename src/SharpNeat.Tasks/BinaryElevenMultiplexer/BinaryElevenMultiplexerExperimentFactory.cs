﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Experiments;
using SharpNeat.Experiments.ConfigModels;
using SharpNeat.IO;
using SharpNeat.NeuralNets.ActivationFunctions;

namespace SharpNeat.Tasks.BinaryElevenMultiplexer;

/// <summary>
/// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the Binary 11-multiplexer task.
/// </summary>
public sealed class BinaryElevenMultiplexerExperimentFactory : INeatExperimentFactory
{
    /// <inheritdoc/>
    public string Id => "binary-11-multiplexer";

    /// <inheritdoc/>
    public INeatExperiment<TScalar> CreateExperiment<TScalar>(Stream jsonConfigStream)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Load experiment JSON config.
        ExperimentConfig experimentConfig = JsonUtils.Deserialize<ExperimentConfig>(jsonConfigStream);

        // Create an evaluation scheme object for the binary 11-multiplexer task.
        var evalScheme = new BinaryElevenMultiplexerEvaluationScheme<TScalar>();

        // Create a NeatExperiment object with the evaluation scheme,
        // and assign some default settings (these can be overridden by config).
        var experiment = new NeatExperiment<TScalar>(evalScheme, Id)
        {
            IsAcyclic = true,
            ActivationFnName = ActivationFunctionId.LeakyReLU.ToString()
        };

        // Apply configuration to the experiment instance.
        experiment.Configure(experimentConfig);
        return experiment;
    }
}
