﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Experiments;
using SharpNeat.IO;
using SharpNeat.Tasks.FunctionRegression;
using SharpNeat.Tasks.GenerativeFunctionRegression.ConfigModels;
using SharpNeat.Windows.Experiments;

namespace SharpNeat.Tasks.Windows.GenerativeFunctionRegression;

/// <summary>
/// An <see cref="IExperimentUiFactory"/> for the Generative Function Regression task.
/// </summary>
public sealed class GenerativeFnRegressionUiFactory : IExperimentUiFactory
{
    /// <inheritdoc/>
    public IExperimentUi CreateExperimentUi(
        INeatExperiment<float> neatExperiment,
        Stream jsonConfigStream)
    {
        // Load experiment JSON config.
        GenerativeFnRegressionExperimentConfig experimentConfig =
            JsonUtils.Deserialize<GenerativeFnRegressionExperimentConfig>(
                jsonConfigStream);

        // Read custom evaluation scheme config.
        ReadEvaluationSchemeConfig(
            experimentConfig.CustomEvaluationSchemeConfig,
            out Func<float, float> fn,
            out ParamSamplingInfo<float> paramSamplingInfo);

        return new GenerativeFnRegressionUi(
            neatExperiment, fn, paramSamplingInfo);
    }

    private static void ReadEvaluationSchemeConfig(
        GenerativeFnRegressionCustomConfig customConfig,
        out Func<float, float> fn,
        out ParamSamplingInfo<float> paramSamplingInfo)
    {
        // Read function ID.
        FunctionId functionId = Enum.Parse<FunctionId>(customConfig.FunctionId);

        fn = FunctionFactory.GetFunction<float>(functionId);

        // Read sample interval min and max, and sample resolution.
        paramSamplingInfo = new ParamSamplingInfo<float>(
            (float)customConfig.SampleIntervalMin,
            (float)customConfig.SampleIntervalMax,
            customConfig.SampleResolution);
    }
}
