using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation
{
    /// <summary>
    /// A strategy for mutating the weights on an array of connection genes.
    /// </summary>
    /// <typeparam name="T">Connection weight type.</typeparam>
    public interface IConnectionArrayMutationStrategy<T>
        where T : struct
    {
        /// <summary>
        /// Invoke the strategy.
        /// </summary>
        /// <param name="connArr">Connection gene array.</param>
        void Invoke(ConnectionGene<T>[] connArr);
    }
}
