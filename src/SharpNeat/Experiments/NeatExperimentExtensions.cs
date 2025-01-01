﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Experiments.ConfigModels;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.NeuralNets.ActivationFunctions;
using static SharpNeat.Experiments.ModelUtils;

namespace SharpNeat.Experiments;

/// <summary>
/// <see cref="INeatExperiment{T}"/> extension methods.
/// </summary>
public static class NeatExperimentExtensions
{
    /// <summary>
    /// Apply configuration to a given <see cref="INeatExperiment{T}"/> instance.
    /// </summary>
    /// <param name="experiment">The NEAT experiment to configure.</param>
    /// <param name="experimentConfig">The configuration to apply.</param>
    /// <typeparam name="TScalar">Experiment black box input/output data type.</typeparam>
    public static void Configure<TScalar>(
        this INeatExperiment<TScalar> experiment,
        ExperimentConfig experimentConfig)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        experiment.Id = experimentConfig.Id ?? experiment.Id;
        experiment.Name = experimentConfig.Name ?? experiment.Name;
        experiment.Description = experimentConfig.Description ?? experiment.Description;
        experiment.IsAcyclic = experimentConfig.IsAcyclic ?? experiment.IsAcyclic;
        experiment.CyclesPerActivation = experimentConfig.CyclesPerActivation ?? experiment.CyclesPerActivation;
        experiment.ActivationFnName = experimentConfig.ActivationFnName ?? experiment.ActivationFnName;
        experiment.PopulationSize = experimentConfig.PopulationSize ?? experiment.PopulationSize;
        experiment.InitialInterconnectionsProportion = experimentConfig.InitialInterconnectionsProportion ?? experiment.InitialInterconnectionsProportion;
        experiment.ConnectionWeightScale = experimentConfig.ConnectionWeightScale ?? experiment.ConnectionWeightScale;
        experiment.DegreeOfParallelism = experimentConfig.DegreeOfParallelism ?? experiment.DegreeOfParallelism;
        experiment.EnableHardwareAcceleratedNeuralNets = experimentConfig.EnableHardwareAcceleratedNeuralNets ?? experiment.EnableHardwareAcceleratedNeuralNets;
        experiment.EnableHardwareAcceleratedActivationFunctions = experimentConfig.EnableHardwareAcceleratedActivationFunctions ?? experiment.EnableHardwareAcceleratedActivationFunctions;

        experiment.EvolutionAlgorithmSettings = experimentConfig.EvolutionAlgorithm ?? experiment.EvolutionAlgorithmSettings;
        experiment.AsexualReproductionSettings = experimentConfig.AsexualReproduction ?? experiment.AsexualReproductionSettings;
        experiment.RecombinationSettings = experimentConfig.Recombination ?? experiment.RecombinationSettings;

        ApplyConfiguration(experiment, experimentConfig.ComplexityRegulationStrategy);
    }

    /// <summary>
    /// Create a <see cref="MetaNeatGenome{T}"/> based on the parameters supplied by an
    /// <see cref="INeatExperiment{T}"/>.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="neatExperiment">The neat experiment.</param>
    /// <returns>A new instance of <see cref="MetaNeatGenome{T}"/>.</returns>
    public static MetaNeatGenome<TScalar> CreateMetaNeatGenome<TScalar>(
        this INeatExperiment<TScalar> neatExperiment)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Resolve the configured activation function name to an activation function instance.
        var actFnFactory = new DefaultActivationFunctionFactory<TScalar>(
            neatExperiment.EnableHardwareAcceleratedActivationFunctions);

        var activationFn = actFnFactory.GetActivationFunction(
            neatExperiment.ActivationFnName);

        var metaNeatGenome = new MetaNeatGenome<TScalar>(
            inputNodeCount: neatExperiment.EvaluationScheme.InputCount,
            outputNodeCount: neatExperiment.EvaluationScheme.OutputCount,
            isAcyclic: neatExperiment.IsAcyclic,
            cyclesPerActivation: neatExperiment.CyclesPerActivation,
            activationFn: activationFn,
            connectionWeightScale: neatExperiment.ConnectionWeightScale);

        return metaNeatGenome;
    }

    #region Private Static Methods

    private static void ApplyConfiguration<TScalar>(
        INeatExperiment<TScalar> experiment,
        ComplexityRegulationStrategyConfig? config)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        if(config is null)
            return;

        string? strategyName = config.StrategyName;
        IComplexityRegulationStrategy strategy =
            strategyName switch
            {
                // The 'null' strategy has been explicitly defined.
                "null" => new NullComplexityRegulationStrategy(),
                "absolute" => ReadAbsoluteComplexityRegulationStrategy(config),
                "relative" => ReadRelativeComplexityRegulationStrategy(config),
                null => throw new ConfigurationException($"strategyName not defined."),
                _ => throw new ConfigurationException($"Unsupported complexity regulation strategyName [{strategyName}]"),
            };

        experiment.ComplexityRegulationStrategy = strategy;
    }

    private static AbsoluteComplexityRegulationStrategy ReadAbsoluteComplexityRegulationStrategy(
        ComplexityRegulationStrategyConfig config)
    {
        int complexityCeiling = GetMandatoryProperty(
            config,
            x => x.ComplexityCeiling);

        int minSimplifcationGenerations = GetMandatoryProperty(
            config,
            x => x.MinSimplifcationGenerations);

        return new AbsoluteComplexityRegulationStrategy(minSimplifcationGenerations, complexityCeiling);
    }

    private static RelativeComplexityRegulationStrategy ReadRelativeComplexityRegulationStrategy(
        ComplexityRegulationStrategyConfig config)
    {
        int relativeComplexityCeiling = GetMandatoryProperty(
            config,
            x => x.RelativeComplexityCeiling);

        int minSimplifcationGenerations = GetMandatoryProperty(
            config,
            x => x.MinSimplifcationGenerations);

        return new RelativeComplexityRegulationStrategy(minSimplifcationGenerations, relativeComplexityCeiling);
    }

    #endregion
}
