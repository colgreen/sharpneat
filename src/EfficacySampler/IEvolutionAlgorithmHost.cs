
namespace EfficacySampler;

/// <summary>
/// Represents a host for an IEvolutionAlgorithm instance, that runs the algorithm until some defined stop condition occurs, and then returns
/// statistics (e.g. max fitness) associated with the evolution algorithm recorded at the stop time.
/// </summary>
public interface IEvolutionAlgorithmHost
{
    /// <summary>
    /// Take one sample, i.e. start one instance of an evolution algorithm, wait for the stop condition, and return evolution algorithm statistics.
    /// </summary>
    /// <returns>A new sample object that conveys the recorded statistics.</returns>
    Sample Sample();
}
