// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Experiments;
using SharpNeat.IO;
using SharpNeat.NeuralNets.ActivationFunctions;
using SharpNeat.Tasks.FunctionRegression;
using SharpNeat.Tasks.GenerativeFunctionRegression.ConfigModels;

namespace SharpNeat.Tasks.GenerativeFunctionRegression;

/// <summary>
/// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the generative sinewave task.
/// </summary>
public sealed class GenerativeFnRegressionExperimentFactory : INeatExperimentFactory
{
    /// <inheritdoc/>
    public string Id => "generative-sinewave";

    /// <inheritdoc/>
    public INeatExperiment<TScalar> CreateExperiment<TScalar>(Stream jsonConfigStream)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Load experiment JSON config.
        GenerativeFnRegressionExperimentConfig experimentConfig =
            JsonUtils.Deserialize<GenerativeFnRegressionExperimentConfig>(
                jsonConfigStream);

        // Read custom evaluation scheme config.
        ReadEvaluationSchemeConfig(
            experimentConfig,
            out Func<TScalar, TScalar> fn,
            out ParamSamplingInfo<TScalar> paramSamplingInfo,
            out TScalar gradientMseWeight);

        // Create an evaluation scheme object for the generative sinewave task; using the evaluation scheme
        // config read from json.
        var evalScheme = new GenerativeFnRegressionEvaluationScheme<TScalar>(fn, paramSamplingInfo, gradientMseWeight);

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

    #region Private Static Methods

    private static void ReadEvaluationSchemeConfig<TScalar>(
        GenerativeFnRegressionExperimentConfig experimentConfig,
        out Func<TScalar, TScalar> fn,
        out ParamSamplingInfo<TScalar> paramSamplingInfo,
        out TScalar gradientMseWeight)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Get the customEvaluationSchemeConfig section.
        GenerativeFnRegressionCustomConfig customConfig = experimentConfig.CustomEvaluationSchemeConfig;

        // Read function ID.
        FunctionId functionId = Enum.Parse<FunctionId>(customConfig.FunctionId);

        fn = FunctionFactory.GetFunction<TScalar>(functionId);

        // Read sample interval min and max, and sample resolution.
        paramSamplingInfo = new ParamSamplingInfo<TScalar>(
            TScalar.CreateChecked(customConfig.SampleIntervalMin),
            TScalar.CreateChecked(customConfig.SampleIntervalMax),
            customConfig.SampleResolution);

        // Read the weight to apply to the gradientMse readings in the final fitness score.
        // 0 means don't use the gradient measurements, 1 means give them equal weight to the y position readings at each x sample point.
        gradientMseWeight = TScalar.CreateChecked(customConfig.GradientMseWeight);
    }

    #endregion
}
