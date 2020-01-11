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
using Newtonsoft.Json.Linq;
using SharpNeat.Experiments;
using SharpNeat.IO;
using SharpNeat.NeuralNet;

namespace SharpNeat.Tasks.CartPole.SinglePole
{
    /// <summary>
    /// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the cart and single pole balancing task.
    /// </summary>
    public class CartSinglePoleExperimentFactory : INeatExperimentFactory<double>
    {
        const ActivationFunctionName __DefaultActivationFunctionName = ActivationFunctionName.LogisticSteep;

        /// <summary>
        /// Create a new instance of <see cref="INeatExperiment{T}"/>.
        /// </summary>
        /// <param name="jsonConfig">Experiment config in json format.</param>
        /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
        public INeatExperiment<double> CreateExperiment(string jsonConfig)
        {
            // Parse the json config string.
            JObject configJobj = JsonUtils.Parse(jsonConfig);

            // Create an evaluation scheme object for the Single Pole Balancing task.
            var evalScheme = new CartSinglePoleEvaluationScheme();

            // Create a NeatExperiment object with the evaluation scheme.
            var experiment = NeatExperiment<double>.CreateAcyclic(
                "Cart and Pole Balancing (Single Pole)",
                evalScheme,
                __DefaultActivationFunctionName.ToString());

            // Read standard neat experiment json config and use it configure the experiment.
            if(configJobj != null) { 
                NeatExperimentJsonReader<double>.Read(experiment, configJobj);
            }

            return experiment;
        }
    }
}
