// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
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
    public IExperimentUi CreateExperimentUi<TScalar>(
        INeatExperiment<TScalar> neatExperiment,
        Stream jsonConfigStream)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Load experiment JSON config.
        PreyCaptureExperimentConfig experimentConfig =
            JsonUtils.Deserialize<PreyCaptureExperimentConfig>(
                jsonConfigStream);

        return new PreyCaptureExperimentUi<TScalar>(
            neatExperiment,
            experimentConfig.CustomEvaluationSchemeConfig);
    }
}
