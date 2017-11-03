using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeatLib.Tests.Neat.Reproduction.Asexual.WeightMutation.Selection
{
    [TestClass]
    public class CardinalSubsetSelectionStrategyTests
    {
        [TestMethod]
        [TestCategory("CardinalSubsetSelectionStrategy")]
        public void TestCardinality()
        {
            var strategy = new CardinalSubsetSelectionStrategy(30);
            for(int i=0; i < 101; i++)
            {
                int[] idxArr = strategy.SelectSubset(i);
                int expectedCardinality = Math.Min(30, i);
                Assert.AreEqual(expectedCardinality, idxArr.Length);
            }
        }

        [TestMethod]
        [TestCategory("CardinalSubsetSelectionStrategy")]
        public void TestUniqueness()
        {
            var strategy = new CardinalSubsetSelectionStrategy(30);
            for(int i=0; i < 20; i++)
            {
                int[] idxArr = strategy.SelectSubset(20);
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
