﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Experiments;
using SharpNeat.Experiments.ConfigModels;
using SharpNeat.IO;
using SharpNeat.NeuralNets.ActivationFunctions;

namespace SharpNeat.Tasks.CartPole.SinglePole;

/// <summary>
/// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the cart and single pole balancing task.
/// </summary>
public sealed class CartSinglePoleExperimentFactory : INeatExperimentFactory
{
    /// <inheritdoc/>
    public string Id => "cartpole-singlepole";

    /// <inheritdoc/>
    public INeatExperiment<TScalar> CreateExperiment<TScalar>(Stream jsonConfigStream)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Load experiment JSON config.
        ExperimentConfig experimentConfig = JsonUtils.Deserialize<ExperimentConfig>(jsonConfigStream);

        // Create an evaluation scheme object for the Single Pole Balancing task.
        var evalScheme = new CartSinglePoleEvaluationScheme<TScalar>();

        // Create a NeatExperiment object with the evaluation scheme,
        // and assign some default settings (these can be overridden by config).
        var experiment = new NeatExperiment<TScalar>(evalScheme, Id)
        {
            IsAcyclic = true,
            ActivationFnName = ActivationFunctionId.LogisticSteep.ToString()
        };

        // Apply configuration to the experiment instance.
        experiment.Configure(experimentConfig);
        return experiment;
    }
}
