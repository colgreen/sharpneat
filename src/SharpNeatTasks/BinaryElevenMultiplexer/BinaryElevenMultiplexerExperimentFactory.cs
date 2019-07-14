/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using Newtonsoft.Json.Linq;
using SharpNeat.Experiments;

namespace SharpNeat.Tasks.BinaryElevenMultiplexer
{
    /// <summary>
    /// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the Binary 11-multiplexer task.
    /// </summary>
    public class BinaryElevenMultiplexerExperimentFactory : INeatExperimentFactory<double>
    {
        // TODO: Consider defining a central list of standard activation function names.
        const string __DefaultActivationFunctionName = "LeakyReLU";

        /// <summary>
        /// Create a new instance of <see cref="INeatExperiment{T}"/>.
        /// </summary>
        /// <param name="jsonConfig">Experiment config in json format.</param>
        /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
        public INeatExperiment<double> CreateExperiment(string jsonConfig)
        {
            // Parse the json config string.
            JObject configJobj = JObject.Parse(jsonConfig);

            // Create an evaluation scheme object for the binary 11-multiplexer task.
            var evalScheme = new BinaryElevenMultiplexerEvaluationScheme();

            // Create a NeatExperiment object with the evaluation scheme.
            var experiment = NeatExperiment<double>.CreateAcyclic(
                "Binary 11-multiplexer",
                evalScheme,
                __DefaultActivationFunctionName);

            // Read standard neat experiment json config and use it configure the experiment.
            NeatExperimentJsonReader<double>.Read(experiment, configJobj);

            return experiment;
        }
    }
}
