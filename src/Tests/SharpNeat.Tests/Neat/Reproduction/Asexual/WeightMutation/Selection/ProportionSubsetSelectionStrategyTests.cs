using System;
using System.Collections.Generic;
using Redzen.Random;
using Xunit;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection.Tests
{
    public class ProportionSubsetSelectionStrategyTests
    {
        [Fact]
        public void SelectSubset_Cardinality()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();

            var strategy = new ProportionSubsetSelectionStrategy(0.1);
            for(int i=0; i < 100; i++)
            {
                int[] idxArr = strategy.SelectSubset(i, rng);
                int expectedCardinalityMin = (int)Math.Floor(i * 0.1);
                int expectedCardinalityMax = (int)Math.Ceiling(i * 0.1);
                Assert.True(idxArr.Length >= expectedCardinalityMin && idxArr.Length <= expectedCardinalityMax);
            }
        }

        [Fact]
        public void SelectSubset_Uniqueness()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();

            var strategy = new ProportionSubsetSelectionStrategy(0.66);
            for(int i=0; i < 20; i++)
            {
                int[] idxArr = strategy.SelectSubset(50, rng);
                HashSet<int> idxSet = new HashSet<int>();

                for(int j=0; j < idxArr.Length; j++)
                {
                    int val = idxArr[j];
                    Assert.DoesNotContain(val, idxSet);
                    idxSet.Add(val);
                }
            }
        }
    }
}
