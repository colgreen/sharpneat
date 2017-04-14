
namespace SharpNeat.Core
{
    public interface IPhenomeEvaluator<TPhenome>
    {
        /// <summary>
        /// Evaluate the provided phenome and return its fitness score.
        /// </summary>
        /// <returns>A fitness score or scores for the phenome.</returns>/returns>
        FitnessInfo Evaluate(TPhenome phenome);

        /// <summary>
        /// Gets the fitness of a perfect/optimal phenome. This can be used to test when to stop the evolution algorithm.
        /// Return FitnessInfo.Empty if this is not required or useful for a given IPhenomeEvaluator.
        /// </summary>
        FitnessInfo OptimalFitness { get; }
    }
}
