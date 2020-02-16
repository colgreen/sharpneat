/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Text.Json;
using SharpNeat.Experiments;
using SharpNeat.IO;
using SharpNeat.NeuralNet;
using SharpNeat.Tasks.FunctionRegression;

namespace SharpNeat.Tasks.GenerativeFunctionRegression
{
    /// <summary>
    /// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the generative sinewave task.
    /// </summary>
    public class GenerativeFnRegressionExperimentFactory : INeatExperimentFactory<double>
    {
        #region Public Methods

        /// <summary>
        /// Create a new instance of <see cref="INeatExperiment{T}"/>.
        /// </summary>
        /// <param name="configElem">Experiment config in json form.</param>
        /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
        public INeatExperiment<double> CreateExperiment(JsonElement configElem)
        {
            // Read the customEvaluationSchemeConfig section.
            ReadEvaluationSchemeConfig(
                configElem,
                out Func<double,double> fn,
                out ParamSamplingInfo paramSamplingInfo,
                out double gradientMseWeight);

            // Create an evaluation scheme object for the generative sinewave task; using the evaluation scheme
            // config read from json.
            var evalScheme = new GenerativeFnRegressionEvaluationScheme(fn, paramSamplingInfo, gradientMseWeight);

            // Create a NeatExperiment object with the evaluation scheme,
            // and assign some default settings (these can be overridden by config).
            var experiment = new NeatExperiment<double>("Generative Function Regression", evalScheme)
            {
                IsAcyclic = false,
                CyclesPerActivation = 1,
                ActivationFnName = ActivationFunctionId.LeakyReLU.ToString()
            };

            // Read standard neat experiment json config and use it configure the experiment.
            NeatExperimentJsonReader<double>.Read(experiment, configElem);

            return experiment;
        }

        #endregion

        #region Private static Methods

        private static void ReadEvaluationSchemeConfig(
            JsonElement configElem,
            out Func<double,double> fn,
            out ParamSamplingInfo paramSamplingInfo,
            out double gradientMseWeight)
        {
            // Get the customEvaluationSchemeConfig section.
            if(!configElem.TryGetProperty("customEvaluationSchemeConfig", out JsonElement evalSchemeElem)) {
                throw new Exception("customEvaluationSchemeConfig not defined.");
            }

            // Read function ID.
            string functionIdStr = JsonReadMandatoryUtils.ReadStringMandatory(evalSchemeElem, "functionId");
            FunctionId functionId = (FunctionId)Enum.Parse(typeof(FunctionId), functionIdStr);
            fn = FunctionFactory.GetFunction(functionId);

            // Read sample interval min and max, and sample resolution.
            double sampleIntervalMin = JsonReadMandatoryUtils.ReadDoubleMandatory(evalSchemeElem, "sampleIntervalMin");
            double sampleIntervalMax = JsonReadMandatoryUtils.ReadDoubleMandatory(evalSchemeElem, "sampleIntervalMax");
            int sampleResolution = JsonReadMandatoryUtils.ReadIntMandatory(evalSchemeElem, "sampleResolution");
            paramSamplingInfo = new ParamSamplingInfo(sampleIntervalMin, sampleIntervalMax, sampleResolution);

            // Read the weight to apply to the gradientMse readings in the final fitness score.
            // 0 means don't use the gradient measurements, 1 means give them equal weight to the y position readings at each x sample point.
            gradientMseWeight = JsonReadMandatoryUtils.ReadDoubleMandatory(evalSchemeElem, "gradientMseWeight");
        }

        #endregion
    }
}
