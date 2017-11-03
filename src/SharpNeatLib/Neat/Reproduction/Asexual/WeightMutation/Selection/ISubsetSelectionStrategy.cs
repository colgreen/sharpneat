
namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection
{
    /// <summary>
    /// Strategy for selecting a sub-set of items from a superset.
    /// </summary>
    public interface ISubsetSelectionStrategy
    {
        /// <summary>
        /// Select a subset of items from a superset of a given size.
        /// </summary>
        /// <param name="supersetCount">The size of the superset to select from.</param>
        /// <returns>An array of indexes that are the selected items.</returns>
        int[] SelectSubset(int supersetCount);
    }
}
