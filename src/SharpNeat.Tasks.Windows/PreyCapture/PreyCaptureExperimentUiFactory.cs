// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Experiments;
using SharpNeat.IO;
using SharpNeat.Tasks.PreyCapture.ConfigModels;
using SharpNeat.Windows.Experiments;

namespace SharpNeat.Tasks.Windows.PreyCapture;

/// <summary>
/// An <see cref="IExperimentUiFactory"/> for the Prey Capture task.
/// </summary>
public sealed class PreyCaptureExperimentUiFactory : IExperimentUiFactory
{
    /// <inheritdoc/>
    public IExperimentUi CreateExperimentUi(
        INeatExperiment<float> neatExperiment,
        Stream jsonConfigStream)
    {
        // Load experiment JSON config.
        PreyCaptureExperimentConfig experimentConfig =
            JsonUtils.Deserialize<PreyCaptureExperimentConfig>(
                jsonConfigStream);

        return new PreyCaptureExperimentUi(
            neatExperiment,
            experimentConfig.CustomEvaluationSchemeConfig);
    }
}
