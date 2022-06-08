// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Text.Json;
using SharpNeat.Experiments;
using SharpNeat.NeuralNets;

namespace SharpNeat.Tasks.CartPole.SinglePole;

/// <summary>
/// A factory for creating instances of <see cref="INeatExperiment{T}"/> for the cart and single pole balancing task.
/// </summary>
public sealed class CartSinglePoleExperimentFactory : INeatExperimentFactory
{
    /// <inheritdoc/>
    public string Id => "cartpole-singlepole";

    /// <inheritdoc/>
    public INeatExperiment<double> CreateExperiment(JsonElement configElem)
    {
        // Create an evaluation scheme object for the Single Pole Balancing task.
        var evalScheme = new CartSinglePoleEvaluationScheme();

        // Create a NeatExperiment object with the evaluation scheme,
        // and assign some default settings (these can be overridden by config).
        var experiment = new NeatExperiment<double>(evalScheme, this.Id)
        {
            IsAcyclic = true,
            ActivationFnName = ActivationFunctionId.LogisticSteep.ToString()
        };

        // Read standard neat experiment json config and use it configure the experiment.
        NeatExperimentJsonReader<double>.Read(experiment, configElem);
        return experiment;
    }

    /// <inheritdoc/>
    public INeatExperiment<float> CreateExperimentSinglePrecision(JsonElement configElem)
    {
        throw new NotImplementedException();
    }
}
