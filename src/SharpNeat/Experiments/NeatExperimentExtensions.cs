// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Experiments.ConfigModels;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Sexual;
using static SharpNeat.Experiments.ModelUtils;

namespace SharpNeat.Experiments;

/// <summary>
/// <see cref="INeatExperiment{T}"/> extension methods.
/// </summary>
public static class NeatExperimentExtensions
{
    #region Public Static Methods

    /// <summary>
    /// Apply configuration to a given <see cref="INeatExperiment{T}"/> instance.
    /// </summary>
    /// <param name="experiment">The NEAT experiment to configure.</param>
    /// <param name="experimentConfig">The configuration to apply.</param>
    /// <typeparam name="T">Experiment black-box numeric data type.</typeparam>
    public static void Configure<T>(
        this INeatExperiment<T> experiment,
        ExperimentConfig experimentConfig)
        where T : struct
    {
        experiment.Id = experimentConfig.Id ?? experiment.Id;
        experiment.Name = experimentConfig.Name ?? experiment.Name;
        experiment.Description = experimentConfig.Description ?? experiment.Description;
        experiment.IsAcyclic = experimentConfig.IsAcyclic ?? experiment.IsAcyclic;
        experiment.CyclesPerActivation = experimentConfig.CyclesPerActivation ?? experiment.CyclesPerActivation;
        experiment.ActivationFnName = experimentConfig.ActivationFnName ?? experiment.ActivationFnName;

        ApplyConfiguration(experiment.EvolutionAlgorithmSettings, experimentConfig.EvolutionAlgorithm);
        ApplyConfiguration(experiment.ReproductionAsexualSettings, experimentConfig.ReproductionAsexual);
        ApplyConfiguration(experiment.ReproductionSexualSettings, experimentConfig.ReproductionSexual);

        experiment.PopulationSize = experimentConfig.PopulationSize ?? experiment.PopulationSize;
        experiment.InitialInterconnectionsProportion = experimentConfig.InitialInterconnectionsProportion ?? experiment.InitialInterconnectionsProportion;
        experiment.ConnectionWeightScale = experimentConfig.ConnectionWeightScale ?? experiment.ConnectionWeightScale;

        ApplyConfiguration(experiment, experimentConfig.ComplexityRegulationStrategy);

        experiment.DegreeOfParallelism = experimentConfig.DegreeOfParallelism ?? experiment.DegreeOfParallelism;
        experiment.EnableHardwareAcceleratedNeuralNets = experimentConfig.EnableHardwareAcceleratedNeuralNets ?? experiment.EnableHardwareAcceleratedNeuralNets;
        experiment.EnableHardwareAcceleratedActivationFunctions = experimentConfig.EnableHardwareAcceleratedActivationFunctions ?? experiment.EnableHardwareAcceleratedActivationFunctions;
    }

    #endregion

    #region Private Static Methods

    private static void ApplyConfiguration(
        NeatEvolutionAlgorithmSettings settings,
        NeatEvolutionAlgorithmConfig? config)
    {
        if(config is null)
            return;

        settings.SpeciesCount = config.SpeciesCount ?? settings.SpeciesCount;
        settings.ElitismProportion = config.ElitismProportion ?? settings.ElitismProportion;
        settings.SelectionProportion = config.SelectionProportion ?? settings.SelectionProportion;
        settings.OffspringAsexualProportion = config.OffspringAsexualProportion ?? settings.OffspringAsexualProportion;
        settings.OffspringSexualProportion = config.OffspringSexualProportion ?? settings.OffspringSexualProportion;
        settings.InterspeciesMatingProportion = config.InterspeciesMatingProportion ?? settings.InterspeciesMatingProportion;
        settings.StatisticsMovingAverageHistoryLength = config.StatisticsMovingAverageHistoryLength ?? settings.StatisticsMovingAverageHistoryLength;
    }

    private static void ApplyConfiguration(
        NeatReproductionAsexualSettings settings,
        NeatReproductionAsexualConfig? config)
    {
        if(config is null)
            return;

        settings.ConnectionWeightMutationProbability = config.ConnectionWeightMutationProbability ?? settings.ConnectionWeightMutationProbability;
        settings.AddNodeMutationProbability = config.AddNodeMutationProbability ?? settings.AddNodeMutationProbability;
        settings.AddConnectionMutationProbability = config.AddConnectionMutationProbability ?? settings.AddConnectionMutationProbability;
        settings.DeleteConnectionMutationProbability = config.DeleteConnectionMutationProbability ?? settings.DeleteConnectionMutationProbability;
    }

    private static void ApplyConfiguration(
        NeatReproductionSexualSettings settings,
        NeatReproductionSexualConfig? config)
    {
        if(config is null)
            return;

        settings.SecondaryParentGeneProbability = config.SecondaryParentGeneProbability ?? settings.SecondaryParentGeneProbability;
    }

    private static void ApplyConfiguration<T>(
        INeatExperiment<T> experiment,
        ComplexityRegulationStrategyConfig? config)
        where T : struct
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
        int complexityCeiling = GetMandatoryProperty(() => config.ComplexityCeiling);
        int minSimplifcationGenerations = GetMandatoryProperty(() => config.MinSimplifcationGenerations);
        return new AbsoluteComplexityRegulationStrategy(minSimplifcationGenerations, complexityCeiling);
    }

    private static RelativeComplexityRegulationStrategy ReadRelativeComplexityRegulationStrategy(
        ComplexityRegulationStrategyConfig config)
    {
        int relativeComplexityCeiling = GetMandatoryProperty(() => config.RelativeComplexityCeiling);
        int minSimplifcationGenerations = GetMandatoryProperty(() => config.MinSimplifcationGenerations);
        return new RelativeComplexityRegulationStrategy(minSimplifcationGenerations, relativeComplexityCeiling);
    }

    #endregion
}
