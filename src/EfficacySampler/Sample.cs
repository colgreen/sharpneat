
namespace EfficacySampler;

/// <summary>
/// A single efficacy sampler sample; this conveys statistics recorded from an evolution algorithm instance.
/// </summary>
public class Sample
{
    /// <summary>
    /// Elapsed time in seconds.
    /// </summary>
    public double ElapsedTimeSecs;
    /// <summary>
    /// Generation count.
    /// </summary>
    public int GenerationCount;
    /// <summary>
    /// Best fitness.
    /// </summary>
    public double BestFitness;
    /// <summary>
    /// Mean fitness.
    /// </summary>
    public double MeanFitness;
    /// <summary>
    /// Maximum complexity.
    /// </summary>
    public double MaxComplexity;
    /// <summary>
    /// Mean complexity.
    /// </summary>
    public double MeanComplexity;
    /// <summary>
    /// Evaluation count.
    /// </summary>
    public ulong EvaluationCount;
}
