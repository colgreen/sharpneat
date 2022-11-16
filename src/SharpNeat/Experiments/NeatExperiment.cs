// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Sexual;
using SharpNeat.NeuralNets;

namespace SharpNeat.Experiments;

/// <summary>
/// An aggregation of settings objects that make up a given experiment.
/// </summary>
/// <typeparam name="T">Black box numeric data type.</typeparam>
public class NeatExperiment<T> : INeatExperiment<T>
    where T : struct
{
    #region Construction

    /// <summary>
    /// Constructs with the provided name and evaluation scheme, and default settings.
    /// </summary>
    /// <param name="evalScheme">Experiment evaluation scheme object.</param>
    /// <param name="factoryId">Experiment Factory ID.</param>
    /// <param name="id">Experiment ID (optional).</param>
    public NeatExperiment(
        IBlackBoxEvaluationScheme<T> evalScheme,
        string factoryId,
        string? id = null)
    {
        EvaluationScheme = evalScheme ?? throw new ArgumentNullException(nameof(evalScheme));
        FactoryId = factoryId ?? throw new ArgumentNullException(nameof(factoryId));
        Id = id ?? factoryId;

        // Use the id as a default name; however this can be overwritten/set after construction.
        Name = id ?? Id;

        // Assign a set of default settings.
        EvolutionAlgorithmSettings = new NeatEvolutionAlgorithmSettings();
        ReproductionAsexualSettings = new NeatReproductionAsexualSettings();
        ReproductionSexualSettings = new NeatReproductionSexualSettings();
        PopulationSize = 400;
        InitialInterconnectionsProportion = 0.05;
        ConnectionWeightScale = 5.0;

        // Assign a default complexity regulation strategy.
        ComplexityRegulationStrategy = new NullComplexityRegulationStrategy();
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public string FactoryId { get; }

    /// <inheritdoc/>
    public string Id { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string Description { get; set; } = string.Empty;

    /// <inheritdoc/>
    public IBlackBoxEvaluationScheme<T> EvaluationScheme { get; }

    /// <inheritdoc/>
    public bool IsAcyclic { get; set; } = false;

    /// <inheritdoc/>
    public int CyclesPerActivation { get; set; } = 1;

    /// <inheritdoc/>
    public string ActivationFnName { get; set; } = ActivationFunctionId.LeakyReLU.ToString();

    /// <inheritdoc/>
    public NeatEvolutionAlgorithmSettings EvolutionAlgorithmSettings { get; }

    /// <inheritdoc/>
    public NeatReproductionAsexualSettings ReproductionAsexualSettings { get; }

    /// <inheritdoc/>
    public NeatReproductionSexualSettings ReproductionSexualSettings { get; }

    /// <inheritdoc/>
    public int PopulationSize { get; set; }

    /// <inheritdoc/>
    public double InitialInterconnectionsProportion { get; set; }

    /// <inheritdoc/>
    public double ConnectionWeightScale { get; set; }

    /// <inheritdoc/>
    public IComplexityRegulationStrategy ComplexityRegulationStrategy { get; set; }

    /// <inheritdoc/>
    public int DegreeOfParallelism { get; set; } = -1;

    /// <inheritdoc/>
    public bool EnableHardwareAcceleratedNeuralNets { get; set; } = false;

    /// <inheritdoc/>
    public bool EnableHardwareAcceleratedActivationFunctions { get; set; } = false;

    #endregion
}
