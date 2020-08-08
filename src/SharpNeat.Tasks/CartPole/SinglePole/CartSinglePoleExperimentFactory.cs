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
using System.Text.Json;
using SharpNeat.Experiments;
using SharpNeat.NeuralNet;

namespace SharpNeat.Tasks.CartPole.SinglePole
{
    /// <summary>
    /// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the cart and single pole balancing task.
    /// </summary>
    public class CartSinglePoleExperimentFactory : INeatExperimentFactory
    {
        /// <summary>
        /// Gets a unique human-readable ID for the experiment.
        /// </summary>
        public string Id => "cartpole-singlepole";

        /// <summary>
        /// Create a new instance of <see cref="INeatExperiment{T}"/>.
        /// </summary>
        /// <param name="configElem">Experiment config in json form.</param>
        /// <returns>A new instance of <see cref="INeatExperiment{T}"/>.</returns>
        public INeatExperiment<double> CreateExperiment(JsonElement configElem)
        {
            // Create an evaluation scheme object for the Single Pole Balancing task.
            var evalScheme = new CartSinglePoleEvaluationScheme();

            // Create a NeatExperiment object with the evaluation scheme,
            // and assign some default settings (these can be overridden by config).
            var experiment = new NeatExperiment<double>("Cart and Pole Balancing (Single Pole)", evalScheme)
            {
                IsAcyclic = true,
                ActivationFnName = ActivationFunctionId.LogisticSteep.ToString()
            };

            // Read standard neat experiment json config and use it configure the experiment.
            NeatExperimentJsonReader<double>.Read(experiment, configElem);
            return experiment;
        }
    }
}
