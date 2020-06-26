using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen;
using SharpNeat.Neat;
using SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;

namespace SharpNeat.Tests.Neat.Reproduction.Sexual.Strategy.UniformCrossover
{
    [TestClass]
    public class EnumerateParentGenesTests
    {
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

            Assert.IsTrue(SpanUtils.ContentEquals<(int,int)>(expectedArr, geneIndexPairArr));
        }

        [TestMethod]
        [TestCategory("SexualReproduction")]
        public void TestEnumerateParentGenes_ExcessGeneInParent1()
        {
            NeatPopulation<double> pop = UniformCrossoverReproductionStrategyTestsUtils.CreateNeatPopulation(2);
            var genome1 = pop.GenomeList[0];
            var genome2 = pop.GenomeList[1];

            (int,int)[] geneIndexPairArr = UniformCrossoverReproductionStrategyUtils.EnumerateParentGenes(genome1.ConnectionGenes, genome2.ConnectionGenes).ToArray();

            (int,int)[] expectedArr = {
                (0,0), (1,1), (2,2),
                (3,3), (4,4), (5,-1),
                (6,5), (7,6), (8,7),
                (9,8), (10,9), (11,10) };

            Assert.IsTrue(SpanUtils.ContentEquals<(int,int)>(expectedArr, geneIndexPairArr));
        }

        [TestMethod]
        [TestCategory("SexualReproduction")]
        public void TestEnumerateParentGenes_ExcessGeneInParent2()
        {
            NeatPopulation<double> pop = UniformCrossoverReproductionStrategyTestsUtils.CreateNeatPopulation(2);
            var genome1 = pop.GenomeList[1];
            var genome2 = pop.GenomeList[0];

            (int,int)[] geneIndexPairArr = UniformCrossoverReproductionStrategyUtils.EnumerateParentGenes(genome1.ConnectionGenes, genome2.ConnectionGenes).ToArray();

            (int,int)[] expectedArr = {
                (0,0), (1,1), (2,2),
                (3,3), (4,4), (-1,5),
                (5,6), (6,7), (7,8),
                (8,9), (9,10), (10,11) };

            Assert.IsTrue(SpanUtils.ContentEquals<(int,int)>(expectedArr, geneIndexPairArr));
        }
    }
}
