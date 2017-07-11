
namespace SharpNeat.Core
{
    public interface IPhenomeEvaluator<TPhenome>
    {
        /// <summary>
        /// Evaluate the provided phenome and return its fitness score.
        /// </summary>
        /// <returns>A fitness score or scores for the phenome.</returns>/returns>
        double Evaluate(TPhenome phenome);

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the evolutionary algorithm search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        bool StopConditionSatisfied { get; }
    }
}
