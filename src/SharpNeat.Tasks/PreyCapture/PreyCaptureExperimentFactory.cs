// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Experiments;
using SharpNeat.IO;
using SharpNeat.NeuralNets.ActivationFunctions;
using SharpNeat.Tasks.PreyCapture.ConfigModels;

namespace SharpNeat.Tasks.PreyCapture;

/// <summary>
/// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the prey capture task.
/// </summary>
public sealed class PreyCaptureExperimentFactory : INeatExperimentFactory
{
    /// <inheritdoc/>
    public string Id => "prey-capture";

    /// <inheritdoc/>
    public INeatExperiment<TScalar> CreateExperiment<TScalar>(Stream jsonConfigStream)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Load experiment JSON config.
        PreyCaptureExperimentConfig experimentConfig = JsonUtils.Deserialize<PreyCaptureExperimentConfig>(jsonConfigStream);

        // Get the customEvaluationSchemeConfig section.
        PreyCaptureCustomConfig customConfig = experimentConfig.CustomEvaluationSchemeConfig;

        // Create an evaluation scheme object for the prey capture task.
        var evalScheme = new PreyCaptureEvaluationScheme<TScalar>(
            customConfig.PreyInitMoves,
            customConfig.PreySpeed,
            customConfig.SensorRange,
            customConfig.MaxTimesteps,
            customConfig.TrialsPerEvaluation);

        // Create a NeatExperiment object with the evaluation scheme,
        // and assign some default settings (these can be overridden by config).
        var experiment = new NeatExperiment<TScalar>(evalScheme, Id)
        {
            IsAcyclic = false,
            CyclesPerActivation = 1,
            ActivationFnName = ActivationFunctionId.LeakyReLU.ToString()
        };

        // Apply configuration to the experiment instance.
        experiment.Configure(experimentConfig);
        return experiment;
    }
}
