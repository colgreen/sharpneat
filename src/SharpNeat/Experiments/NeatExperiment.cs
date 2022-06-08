// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Sexual;
using SharpNeat.NeuralNets;

namespace SharpNeat.Experiments;

#pragma warning disable CA1805 // Do not initialize unnecessarily

/// <summary>
/// An aggregation of settings objects that make up a given experiment.
/// </summary>
/// <typeparam name="T">Black box numeric data type.</typeparam>
public class NeatExperiment<T> : INeatExperiment<T>
    where T : struct
{
    #region Auto Properties

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
    public NeatEvolutionAlgorithmSettings NeatEvolutionAlgorithmSettings { get; }

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

    #region Constructor

    /// <summary>
    /// Constructs with the provided name and evaluation scheme, and default settings.
    /// </summary>
    /// <param name="evalScheme">Experiment evaluation scheme object.</param>
    /// <param name="factoryId">Experiment Factory ID (optional).</param>
    /// <param name="id">Experiment ID.</param>
    public NeatExperiment(
        IBlackBoxEvaluationScheme<T> evalScheme,
        string factoryId, string id)
    {
        this.EvaluationScheme = evalScheme ?? throw new ArgumentNullException(nameof(evalScheme));
        this.Id = id ?? throw new ArgumentNullException(nameof(id));
        this.FactoryId = factoryId ?? throw new ArgumentNullException(nameof(factoryId));

        // Use the id as a default name; however this can be overwritten/set after construction.
        this.Name = id;

        // Assign a set of default settings.
        this.NeatEvolutionAlgorithmSettings = new NeatEvolutionAlgorithmSettings();
        this.ReproductionAsexualSettings = new NeatReproductionAsexualSettings();
        this.ReproductionSexualSettings = new NeatReproductionSexualSettings();
        this.PopulationSize = 400;
        this.InitialInterconnectionsProportion = 0.05;
        this.ConnectionWeightScale = 5.0;

        // Assign a default complexity regulation strategy.
        this.ComplexityRegulationStrategy = new NullComplexityRegulationStrategy();
    }

    /// <summary>
    /// Constructs with the provided name and evaluation scheme, and default settings.
    /// </summary>
    /// <param name="evalScheme">Experiment evaluation scheme object.</param>
    /// <param name="factoryId">Experiment Factory ID (optional).</param>
    public NeatExperiment(
        IBlackBoxEvaluationScheme<T> evalScheme,
        string factoryId)
        : this(evalScheme, factoryId, factoryId)
    {
    }

    #endregion
}
