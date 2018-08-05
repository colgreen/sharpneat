using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeat.Tests.Neat.Reproduction.Asexual.WeightMutation.Selection
{
    public class SelectAllStrategy : ISubsetSelectionStrategy
    {
        public int[] SelectSubset(int supersetCount)
        {
            var arr = new int[supersetCount];
            for(int i=0; i< supersetCount; i++) {
                arr[i] = i;
            }
            return arr;
        }
    }
}
