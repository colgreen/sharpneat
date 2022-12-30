// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
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
    public required GenerativeFnRegressionCustomConfig CustomEvaluationSchemeConfig { get; init; }
}
