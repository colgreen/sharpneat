using Redzen.Random;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeat.Tests.Neat.Reproduction.Asexual.WeightMutation.Selection
{
    public class SelectAllStrategy : ISubsetSelectionStrategy
    {
        /// <summary>
        /// Select a subset of items from a superset of a given size.
        /// </summary>
        /// <param name="supersetCount">The size of the superset to select from.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>An array of indexes that are the selected items.</returns>
        public int[] SelectSubset(int supersetCount, IRandomSource rng)
        {
            var arr = new int[supersetCount];
            for(int i=0; i < supersetCount; i++) {
                arr[i] = i;
            }
            return arr;
        }
    }
}
