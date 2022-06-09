using SharpNeat.Experiments.ConfigModels;

namespace SharpNeat.Tasks.GenerativeFunctionRegression.ConfigModels;

/// <summary>
/// Model type for generative function regression experiment configuration.
/// </summary>
public class GenerativeFnRegressionExperimentConfig : ExperimentConfig
{
    /// <summary>
    /// Custom config for the generative function regression experiment.
    /// </summary>
    public GenerativeFnRegressionCustomConfig? CustomEvaluationSchemeConfig {  get; set; }
}
