// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
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
    public IExperimentUi CreateExperimentUi<TScalar>(
        INeatExperiment<TScalar> neatExperiment,
        Stream jsonConfigStream)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Load experiment JSON config.
        GenerativeFnRegressionExperimentConfig experimentConfig =
            JsonUtils.Deserialize<GenerativeFnRegressionExperimentConfig>(
                jsonConfigStream);

        // Read custom evaluation scheme config.
        ReadEvaluationSchemeConfig(
            experimentConfig.CustomEvaluationSchemeConfig,
            out Func<TScalar, TScalar> fn,
            out ParamSamplingInfo<TScalar> paramSamplingInfo);

        return new GenerativeFnRegressionUi<TScalar>(
            neatExperiment, fn, paramSamplingInfo);
    }

    private static void ReadEvaluationSchemeConfig<TScalar>(
        GenerativeFnRegressionCustomConfig customConfig,
        out Func<TScalar, TScalar> fn,
        out ParamSamplingInfo<TScalar> paramSamplingInfo)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Read function ID.
        FunctionId functionId = Enum.Parse<FunctionId>(customConfig.FunctionId);

        fn = FunctionFactory.GetFunction<TScalar>(functionId);

        // Read sample interval min and max, and sample resolution.
        paramSamplingInfo = new ParamSamplingInfo<TScalar>(
            TScalar.CreateChecked(customConfig.SampleIntervalMin),
            TScalar.CreateChecked(customConfig.SampleIntervalMax),
            customConfig.SampleResolution);
    }
}
