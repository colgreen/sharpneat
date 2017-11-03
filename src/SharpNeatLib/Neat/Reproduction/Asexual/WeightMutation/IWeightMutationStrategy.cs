using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation
{
    /// <summary>
    /// A strategy for mutating single connection weighs.
    /// </summary>
    /// <typeparam name="T">Connection weight type.</typeparam>
    public interface IWeightMutationStrategy<T> where T : struct
    {
        /// <summary>
        /// Invoke the strategy.
        /// </summary>
        /// <param name="connArr">Connection gene.</param>
        void Invoke(ConnectionGene<T> connGene);
    }
}
