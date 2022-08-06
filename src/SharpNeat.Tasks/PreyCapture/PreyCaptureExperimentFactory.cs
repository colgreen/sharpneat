// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Experiments;
using SharpNeat.IO;
using SharpNeat.NeuralNets;
using SharpNeat.Tasks.PreyCapture.ConfigModels;
using static SharpNeat.Experiments.ModelUtils;

namespace SharpNeat.Tasks.PreyCapture;

/// <summary>
/// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the prey capture task.
/// </summary>
public sealed class PreyCaptureExperimentFactory : INeatExperimentFactory
{
    /// <inheritdoc/>
    public string Id => "prey-capture";

    /// <inheritdoc/>
    public INeatExperiment<double> CreateExperiment(Stream jsonConfigStream)
    {
        // Load experiment JSON config.
        PreyCaptureExperimentConfig experimentConfig = JsonUtils.Deserialize<PreyCaptureExperimentConfig>(jsonConfigStream);

        // Read custom evaluation scheme config.
        ReadEvaluationSchemeConfig(
            experimentConfig,
            out int preyInitMoves,
            out float preySpeed,
            out float sensorRange,
            out int maxTimesteps,
            out int trialsPerEvaluation);

        // Create an evaluation scheme object for the prey capture task.
        var evalScheme = new PreyCaptureEvaluationScheme(
            preyInitMoves, preySpeed,
            sensorRange, maxTimesteps, trialsPerEvaluation);

        // Create a NeatExperiment object with the evaluation scheme,
        // and assign some default settings (these can be overridden by config).
        var experiment = new NeatExperiment<double>(evalScheme, this.Id)
        {
            IsAcyclic = false,
            CyclesPerActivation = 1,
            ActivationFnName = ActivationFunctionId.LeakyReLU.ToString()
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

    #region Private Static Methods

    private static void ReadEvaluationSchemeConfig(
        PreyCaptureExperimentConfig experimentConfig,
        out int preyInitMoves,
        out float preySpeed,
        out float sensorRange,
        out int maxTimesteps,
        out int trialsPerEvaluation)
    {
        // Get the customEvaluationSchemeConfig section.
        if(experimentConfig.CustomEvaluationSchemeConfig is null)
            throw new ConfigurationException("customEvaluationSchemeConfig not defined.");

        PreyCaptureCustomConfig customConfig = experimentConfig.CustomEvaluationSchemeConfig;
        preyInitMoves = GetMandatoryProperty(() => customConfig.PreyInitMoves);
        preySpeed = GetMandatoryProperty(() => customConfig.PreySpeed);
        sensorRange = GetMandatoryProperty(() => customConfig.SensorRange);
        maxTimesteps = GetMandatoryProperty(() => customConfig.MaxTimesteps);
        trialsPerEvaluation = GetMandatoryProperty(() => customConfig.TrialsPerEvaluation);
    }

    #endregion
}
