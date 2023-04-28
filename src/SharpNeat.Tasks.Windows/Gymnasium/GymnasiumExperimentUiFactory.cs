using SharpNeat.Experiments;
using SharpNeat.Experiments.ConfigModels;
using SharpNeat.IO;
using SharpNeat.Windows.Experiments;

namespace SharpNeat.Tasks.Windows.Gymnasium;

public sealed class GymnasiumExperimentUiFactory : IExperimentUiFactory
{
    /// <inheritdoc/>
    public IExperimentUi CreateExperimentUi(
        INeatExperiment<double> neatExperiment,
        Stream jsonConfigStream)
    {
        // Load experiment JSON config.
        ExperimentConfig experimentConfig = JsonUtils.Deserialize<ExperimentConfig>(jsonConfigStream);

        return new GymnasiumExperimentUi(neatExperiment);
    }
}
