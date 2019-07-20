using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using SharpNeat.Evaluation;
using SharpNeat.Neat;
using SharpNeat.Neat.DistanceMetrics.Double;
using SharpNeat.Neat.Genome;

namespace SharpNeatLib.Tests.Neat
{
    [TestClass]
    [TestCategory("NeatPopulationStats")]
    public class NeatPopulationStatsTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("NeatPopulationStats")]
        public void TestNeatPopulationStats()
        {
            // Create test population and apply speciation strategy.
            NeatPopulation<double> neatPop = CreateTestPopulation();

            // Loop the species; assign the same fitness to genomes within each species.
            for(int i=0; i< neatPop.SpeciesArray.Length; i++)
            {
                double fitness = (i+1) * 10.0;
                neatPop.SpeciesArray[i].GenomeList.ForEach(x => x.FitnessInfo = new FitnessInfo(fitness));
            }

            // Calc NeatPopulation statistics.
            neatPop.UpdateStats(PrimaryFitnessInfoComparer.Singleton);

            // Validate expected mean fitness for each species.
            for(int i=0; i < neatPop.SpeciesArray.Length; i++)
            {
                double expectedMeanFitness = (i+1) * 10.0;
                Assert.AreEqual(expectedMeanFitness, neatPop.SpeciesArray[i].Stats.MeanFitness);
            }

            // Validate SumSpeciesMeanFitness.
            double expectedSumSpeciesMeanFitness = 10.0 + 20.0 + 30.0;
            Assert.AreEqual(expectedSumSpeciesMeanFitness, neatPop.NeatPopulationStats.SumSpeciesMeanFitness);

            // Validate BestGenomeSpeciesIdx.
            Assert.AreEqual(2, neatPop.NeatPopulationStats.BestGenomeSpeciesIdx);

            // Assign a high fitness to one of the genomes, and check that BestGenomeSpeciesIdx is updated accordingly.
            neatPop.SpeciesArray[0].GenomeList[2].FitnessInfo = new FitnessInfo(100.0);

            // Note. The species must be re-initialised in order for the fit genome to be sorted correctly within its
            // containing species.
            InitialiseSpecies(neatPop);

            neatPop.UpdateStats(PrimaryFitnessInfoComparer.Singleton);
            Assert.AreEqual(0, neatPop.NeatPopulationStats.BestGenomeSpeciesIdx);

            // Perform the same test again with the best genome in he second species.
            neatPop.SpeciesArray[1].GenomeList[3].FitnessInfo = new FitnessInfo(200.0);
            InitialiseSpecies(neatPop);
            neatPop.UpdateStats(PrimaryFitnessInfoComparer.Singleton);
            Assert.AreEqual(1, neatPop.NeatPopulationStats.BestGenomeSpeciesIdx);
        }

        #endregion

        #region Private Static Methods

        private static NeatPopulation<double> CreateTestPopulation()
        {
            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 3,
                outputNodeCount: 2,
                isAcyclic: true,
                activationFn: new SharpNeat.NeuralNet.Double.ActivationFunctions.ReLU());

            int count = 30;
            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 1.0, count, RandomDefaults.CreateRandomSource());
            Assert.AreEqual(count, neatPop.GenomeList.Count);
            Assert.AreEqual(count, neatPop.GenomeIdSeq.Peek);

            // Init species.
            InitialiseSpecies(neatPop);

            return neatPop;
        }

        private static void InitialiseSpecies(NeatPopulation<double> neatPop)
        {
            // Create a speciation strategy instance.
            var distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            var speciationStrategy = new SharpNeat.Neat.Speciation.GeneticKMeans.Parallelized.GeneticKMeansSpeciationStrategy<double>(distanceMetric, 5, 4);

            // Apply the speciation strategy.
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);
            neatPop.InitialiseSpecies(speciationStrategy, 3, rng);
        }

        #endregion
    }
}
