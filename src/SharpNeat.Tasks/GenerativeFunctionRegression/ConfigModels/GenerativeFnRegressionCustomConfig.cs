// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Tasks.GenerativeFunctionRegression.ConfigModels;

/// <summary>
/// Model type for generative function regression custom config section.
/// </summary>
public sealed record GenerativeFnRegressionCustomConfig
{
    /// <summary>
    /// Function ID. E.g. "Sin", "Cos", etc.
    /// </summary>
    public required string FunctionId { get; init; }

    /// <summary>
    /// Sample interval minimum.
    /// </summary>
    public required double SampleIntervalMin { get; init; }

    /// <summary>
    /// Sample interval maximum.
    /// </summary>
    public required double SampleIntervalMax { get; init; }

    /// <summary>
    /// Sampling resolution, within the defined min-max interval.
    /// </summary>
    public required int SampleResolution { get; init; }

    /// <summary>
    /// The fitness weighting to assign to the gradient mean squared error (MSE) score.
    /// </summary>
    public required double GradientMseWeight { get; init; }
}
