namespace SharpNeat.Tasks.GenerativeFunctionRegression.ConfigModels;

/// <summary>
/// Model type for generative function regression custom config section.
/// </summary>
public class GenerativeFnRegressionCustomConfig
{
    /// <summary>
    /// Function ID. E.g. "Sin", "Cos", etc.
    /// </summary>
    public string? FunctionId { get; set; }

    /// <summary>
    /// Sample interval minimum.
    /// </summary>
    public double? SampleIntervalMin { get; set; }

    /// <summary>
    /// Sample interval maximum.
    /// </summary>
    public double? SampleIntervalMax { get; set; }

    /// <summary>
    /// Sampling resolution, within the defined min-max interval.
    /// </summary>
    public int? SampleResolution { get; set; }

    /// <summary>
    /// The fitness weighting to assign to the gradient mean squared error (MSE) score.
    /// </summary>
    public double? GradientMseWeight { get; set; }
}
