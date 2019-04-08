using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Evaluation;
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeatLib.Tests.EvolutionAlgorithm
{
    [TestClass]
    public class PopulationStatsTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("PopulationStats")]
        public void TestPopulationStats()
        {
            // Create a test population.
            Population<MockGenome> pop = CreatePopulation(100, 10.0, 2.0);

            // Modify a few genome fitnesses.
            pop.GenomeList[10].FitnessInfo = new FitnessInfo(100.0);
            pop.GenomeList[10].Complexity = 4.0;

            pop.GenomeList[20].FitnessInfo = new FitnessInfo(0.0);
            pop.GenomeList[20].Complexity = 1.0;

            // Calc/update stats.
            pop.UpdateStats(PrimaryFitnessInfoComparer.Singleton);

            // Validate stats.
            // Fitness stats.
            PopulationStatistics stats = pop.Stats;
            Assert.AreEqual(10, stats.BestGenomeIndex);
            Assert.AreEqual(100.0, stats.BestFitness.PrimaryFitness);
            Assert.AreEqual(10.8, stats.MeanFitness);
            Assert.AreEqual(1, stats.BestFitnessHistory.Length);
            Assert.AreEqual(100.0, stats.BestFitnessHistory.Total);

            // Complexity stats.
            Assert.AreEqual(4.0, stats.BestComplexity);
            Assert.AreEqual(2.01, stats.MeanComplexity);
            Assert.AreEqual(1, stats.MeanComplexityHistory.Length);
            Assert.AreEqual(2.01, stats.MeanComplexityHistory.Total);
        }

        #endregion

        #region Private Static Methods

        private static Population<MockGenome> CreatePopulation(
            int count,
            double defaultFitness,
            double defaultComplexity)
        {
            List<MockGenome> genomeList = new List<MockGenome>(count);
            for(int i=0; i < count; i++) 
            {
                var genome = new MockGenome
                {
                    Id = i,
                    BirthGeneration = 0,
                    FitnessInfo = new FitnessInfo(defaultFitness),
                    Complexity = defaultComplexity
                };

                genomeList.Add(genome);
            }

            return new Population<MockGenome>(genomeList);
        }

        #endregion
    }
}
