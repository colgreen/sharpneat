
namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation
{
    /// <summary>
    /// A connection weight mutation strategy.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public interface IWeightMutationStrategy<T> where T : struct
    {
        /// <summary>
        /// Invoke the strategy.
        /// </summary>
        /// <param name="weightArr">The connection weight array to apply mutations to.</param>
        void Invoke(T[] weightArr);
    }
}
