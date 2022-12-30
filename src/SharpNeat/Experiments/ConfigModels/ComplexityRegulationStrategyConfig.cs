// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Experiments.ConfigModels;

/// <summary>
/// Model type for NEAT complexity regulation strategy configuration.
/// </summary>
public sealed record ComplexityRegulationStrategyConfig
{
    /// <summary>
    /// Regulation strategy name.
    /// </summary>
    public required string StrategyName { get; init; }

    /// <summary>
    /// The minimum number of generations we stay within simplification mode.
    /// </summary>
    public int? MinSimplifcationGenerations { get; init; }

    /// <summary>
    /// The relative complexity ceiling.
    /// </summary>
    public int? RelativeComplexityCeiling { get; set; }

    /// <summary>
    /// The fixed/absolute complexity ceiling.
    /// </summary>
    public int? ComplexityCeiling { get; set; }
}
