using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public interface IAsexualReproductionStrategy<T> where T : struct
    {
        /// <summary>
        /// Create a new child genome from a given parent genome.
        /// </summary>
        /// <param name="parent">The parent genome.</param>
        /// <returns>A new child genome.</returns>
        NeatGenome<T> CreateChildGenome(NeatGenome<T> parent);
    }
}
