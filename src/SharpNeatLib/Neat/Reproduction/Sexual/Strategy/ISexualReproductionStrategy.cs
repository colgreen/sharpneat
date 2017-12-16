using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy
{
    public interface ISexualReproductionStrategy<T> where T : struct
    {
        /// <summary>
        /// Create a new child genome based on the genetic content of two parent genome.
        /// </summary>
        /// <param name="parent1">Parent 1.</param>
        /// <param name="parent2">Parent 2.</param>
        /// <returns>A new child genome.</returns>
        NeatGenome<T> CreateGenome(NeatGenome<double> parent1, NeatGenome<double> parent2);
    }
}
