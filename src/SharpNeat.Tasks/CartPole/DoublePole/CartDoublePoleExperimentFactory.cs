﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Experiments;
using SharpNeat.Experiments.ConfigModels;
using SharpNeat.IO;
using SharpNeat.NeuralNets;

namespace SharpNeat.Tasks.CartPole.DoublePole;

/// <summary>
/// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the cart and double pole balancing task.
/// </summary>
public sealed class CartDoublePoleExperimentFactory : INeatExperimentFactory
{
    /// <inheritdoc/>
    public string Id => "cartpole-doublepole";

    /// <inheritdoc/>
    public INeatExperiment<double> CreateExperiment(Stream jsonConfigStream)
    {
        // Load experiment JSON config.
        ExperimentConfig experimentConfig = JsonUtils.Deserialize<ExperimentConfig>(jsonConfigStream);

        // Create an evaluation scheme object for the Single Pole Balancing task.
        var evalScheme = new CartDoublePoleEvaluationScheme<double>();

        // Create a NeatExperiment object with the evaluation scheme,
        // and assign some default settings (these can be overridden by config).
        var experiment = new NeatExperiment<double>(evalScheme, Id)
        {
            IsAcyclic = false,
            CyclesPerActivation = 1,
            ActivationFnName = ActivationFunctionId.LogisticSteep.ToString()
        };

        // Apply configuration to the experiment instance.
        experiment.Configure(experimentConfig);
        return experiment;
    }

    /// <inheritdoc/>
    public INeatExperiment<float> CreateExperimentSinglePrecision(Stream jsonConfigStream)
    {
        throw new NotImplementedException();
    }
}
