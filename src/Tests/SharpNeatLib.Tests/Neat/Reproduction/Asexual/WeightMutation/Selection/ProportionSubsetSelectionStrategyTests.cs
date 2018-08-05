using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeat.Tests.Neat.Reproduction.Asexual.WeightMutation.Selection
{
    [TestClass]
    public class ProportionSubsetSelectionStrategyTests
    {
        [TestMethod]
        [TestCategory("ProportionSubsetSelectionStrategy")]
        public void TestCardinality()
        {
            var strategy = new ProportionSubsetSelectionStrategy(0.1, RandomDefaults.CreateRandomSource());
            for(int i=0; i < 100; i++)
            {
                int[] idxArr = strategy.SelectSubset(i);
                int expectedCardinalityMin = (int)Math.Floor(i * 0.1);
                int expectedCardinalityMax = (int)Math.Ceiling(i * 0.1);
                Assert.IsTrue(idxArr.Length >= expectedCardinalityMin && idxArr.Length <= expectedCardinalityMax);
            }
        }

        [TestMethod]
        [TestCategory("ProportionSubsetSelection")]
        public void TestUniqueness()
        {
            var strategy = new ProportionSubsetSelectionStrategy(0.66, RandomDefaults.CreateRandomSource());
            for (int i = 0; i < 20; i++)
            {
                int[] idxArr = strategy.SelectSubset(50);
                HashSet<int> idxSet = new HashSet<int>();

                for (int j = 0; j < idxArr.Length; j++)
                {
                    int val = idxArr[j];
                    Assert.IsFalse(idxSet.Contains(val));
                    idxSet.Add(val);
                }
            }
        }
    }
}
