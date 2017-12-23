using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat;
using SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeatLib.Tests.Neat.Reproduction.Sexual.Strategy
{
    [TestClass]
    public class EnumerateParentGenesTests
    {
        // TODO: More tests!


        [TestMethod]
        [TestCategory("SexualReproduction")]
        public void TestEnumerateParentGenes_CompareWithSelf()
        {
            NeatPopulation<double> pop = UniformCrossoverReproductionStrategyTestsUtils.CreateNeatPopulation(1);
            var genome = pop.GenomeList[0];

            (int,int)[] geneIndexPairArr = UniformCrossoverReproductionStrategyUtils.EnumerateParentGenes(genome.ConnectionGenes, genome.ConnectionGenes).ToArray();

            (int,int)[] expectedArr = {
                (0,0), (1,1), (2,2),
                (3,3), (4,4), (5,5),
                (6,6), (7,7), (8,8),
                (9,9), (10,10), (11,11) };

            Assert.IsTrue(AreEqual(expectedArr, geneIndexPairArr));
        }

        #region Private Static Methods

        private static bool AreEqual((int,int)[] expectedArr, (int,int)[] actualArr)
        {
            if(expectedArr.Length != actualArr.Length) {
                return false;
            }

            for(int i=0; i < expectedArr.Length; i++) 
            {
                if(!AreEqual(expectedArr[i], actualArr[i])) {
                    return false;
                }
            }
            return true;
        }

        private static bool AreEqual((int,int) expected, (int,int) actual)
        {
            return expected.Item1 == actual.Item1 && expected.Item2 == actual.Item2;
        }

        #endregion
    }
}
