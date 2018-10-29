using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeat.Tests.Neat.Reproduction.Asexual.WeightMutation.Selection
{
    [TestClass]
    public class CardinalSubsetSelectionStrategyTests
    {
        [TestMethod]
        [TestCategory("CardinalSubsetSelectionStrategy")]
        public void TestCardinality()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();

            var strategy = new CardinalSubsetSelectionStrategy(30);
            for(int i=0; i < 101; i++)
            {
                int[] idxArr = strategy.SelectSubset(i, rng);
                int expectedCardinality = Math.Min(30, i);
                Assert.AreEqual(expectedCardinality, idxArr.Length);
            }
        }

        [TestMethod]
        [TestCategory("CardinalSubsetSelectionStrategy")]
        public void TestUniqueness()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();

            var strategy = new CardinalSubsetSelectionStrategy(30);
            for(int i=0; i < 20; i++)
            {
                int[] idxArr = strategy.SelectSubset(20, rng);
                HashSet<int> idxSet = new HashSet<int>();

                for(int j=0; j<idxArr.Length; j++)
                {
                    int val = idxArr[j];
                    Assert.IsFalse(idxSet.Contains(val));
                    idxSet.Add(val);
                }
            }
        }
    }
}
