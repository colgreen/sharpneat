using System;
using System.Collections.Generic;
using Redzen.Random;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;
using Xunit;

namespace SharpNeat.Tests.Neat.Reproduction.Asexual.WeightMutation.Selection
{
    public class CardinalSubsetSelectionStrategyTests
    {
        [Fact]
        public void SelectSubset_Cardinality()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();

            var strategy = new CardinalSubsetSelectionStrategy(30);
            for(int i=0; i < 101; i++)
            {
                int[] idxArr = strategy.SelectSubset(i, rng);
                int expectedCardinality = Math.Min(30, i);
                Assert.Equal(expectedCardinality, idxArr.Length);
            }
        }

        [Fact]
        public void SelectSubset_Uniqueness()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();

            var strategy = new CardinalSubsetSelectionStrategy(30);
            for(int i=0; i < 20; i++)
            {
                int[] idxArr = strategy.SelectSubset(20, rng);
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
