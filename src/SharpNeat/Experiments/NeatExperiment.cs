// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Recombination;
using SharpNeat.NeuralNets.ActivationFunctions;

namespace SharpNeat.Experiments;

/// <summary>
/// An aggregation of settings objects that make up a given experiment.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public class NeatExperiment<TScalar> : INeatExperiment<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <inheritdoc/>
    public string FactoryId { get; }

    /// <inheritdoc/>
    public string Id { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string Description { get; set; } = string.Empty;

    /// <inheritdoc/>
    public bool IsAcyclic { get; set; }

    /// <inheritdoc/>
    public int CyclesPerActivation { get; set; } = 1;

    /// <inheritdoc/>
    public string ActivationFnName { get; set; } = ActivationFunctionId.LeakyReLU.ToString();

    /// <inheritdoc/>
    public int PopulationSize { get; set; } = 400;

    /// <inheritdoc/>
    public double InitialInterconnectionsProportion { get; set; } = 0.05;

    /// <inheritdoc/>
    public double ConnectionWeightScale { get; set; } = 5.0;

    /// <inheritdoc/>
    public int DegreeOfParallelism { get; set; } = -1;

    /// <inheritdoc/>
    public bool EnableHardwareAcceleratedNeuralNets { get; set; }

    /// <inheritdoc/>
    public bool EnableHardwareAcceleratedActivationFunctions { get; set; }

    /// <inheritdoc/>
    public IBlackBoxEvaluationScheme<TScalar> EvaluationScheme { get; }

    /// <inheritdoc/>
    public NeatEvolutionAlgorithmSettings EvolutionAlgorithmSettings { get; set; } = new NeatEvolutionAlgorithmSettings();

    /// <inheritdoc/>
    public NeatAsexualReproductionSettings AsexualReproductionSettings { get; set; } = new NeatAsexualReproductionSettings();

    /// <inheritdoc/>
    public NeatRecombinationSettings RecombinationSettings { get; set; } = new NeatRecombinationSettings();

    /// <inheritdoc/>
    public IComplexityRegulationStrategy ComplexityRegulationStrategy { get; set; }

    /// <summary>
    /// Constructs with the provided name and evaluation scheme, and default settings.
    /// </summary>
    /// <param name="evalScheme">Experiment evaluation scheme object.</param>
    /// <param name="factoryId">Experiment Factory ID.</param>
    /// <param name="id">Experiment ID (optional).</param>
    public NeatExperiment(
        IBlackBoxEvaluationScheme<TScalar> evalScheme,
        string factoryId,
        string? id = null)
    {
        EvaluationScheme = evalScheme ?? throw new ArgumentNullException(nameof(evalScheme));
        FactoryId = factoryId ?? throw new ArgumentNullException(nameof(factoryId));
        Id = id ?? factoryId;

        // Use the id as a default name; however this can be overwritten/set after construction.
        Name = id ?? Id;

        // Assign a default complexity regulation strategy.
        ComplexityRegulationStrategy = new NullComplexityRegulationStrategy();
    }
}
