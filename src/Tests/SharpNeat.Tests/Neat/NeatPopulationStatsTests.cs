using Redzen.Random;
using SharpNeat.Evaluation;
using SharpNeat.EvolutionAlgorithm;
using SharpNeat.Neat.DistanceMetrics.Double;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using Xunit;

namespace SharpNeat.Neat.Tests
{
    public class NeatPopulationStatsTests
    {
        #region Test Methods

        [Fact]
        public void VerifyPopulationStats()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            // Create a test population.
            NeatPopulation<double> pop = CreateNeatPopulation(100, 10.0, rng);

            // Modify a few genome fitnesses.
            pop.GenomeList[10].FitnessInfo = new FitnessInfo(100.0);
            pop.GenomeList[20].FitnessInfo = new FitnessInfo(0.0);

            // Initialise the NEAT population species; the wider population stats are dependent on this occurring.
            InitialiseSpecies(pop);

            // Calc/update stats.
            pop.UpdateStats(PrimaryFitnessInfoComparer.Singleton, rng);

            // Validate stats.
            // Fitness stats.
            PopulationStatistics stats = pop.Stats;
            Assert.Equal(10, stats.BestGenomeIndex);
            Assert.Equal(100.0, stats.BestFitness.PrimaryFitness);
            Assert.Equal(10.8, stats.MeanFitness);
            Assert.Equal(1, stats.BestFitnessHistory.Length);
            Assert.Equal(100.0, stats.BestFitnessHistory.Sum);

            // Complexity stats.
            Assert.Equal(6.0, stats.BestComplexity);
            Assert.Equal(6.0, stats.MeanComplexity);
            Assert.Equal(1, stats.MeanComplexityHistory.Length);
            Assert.Equal(6.0, stats.MeanComplexityHistory.Sum);
        }

        [Fact]
        public void VerifyNeatPopulationStats()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            // Create test population and apply speciation strategy.
            NeatPopulation<double> neatPop = CreateNeatPopulation(30, 10.0, rng);

            // Loop the species; assign the same fitness to genomes within each species.
            for(int i=0; i < neatPop.SpeciesArray.Length; i++)
            {
                double fitness = (i+1) * 10.0;
                neatPop.SpeciesArray[i].GenomeList.ForEach(x => x.FitnessInfo = new FitnessInfo(fitness));
            }

            // Calc NeatPopulation statistics.
            neatPop.UpdateStats(PrimaryFitnessInfoComparer.Singleton, rng);

            // Validate expected mean fitness for each species.
            for(int i=0; i < neatPop.SpeciesArray.Length; i++)
            {
                double expectedMeanFitness = (i+1) * 10.0;
                Assert.Equal(expectedMeanFitness, neatPop.SpeciesArray[i].Stats.MeanFitness);
            }

            // Validate SumSpeciesMeanFitness.
            double expectedSumSpeciesMeanFitness = 10.0 + 20.0 + 30.0;
            Assert.Equal(expectedSumSpeciesMeanFitness, neatPop.NeatPopulationStats.SumSpeciesMeanFitness);

            // Validate BestGenomeSpeciesIdx.
            Assert.Equal(2, neatPop.NeatPopulationStats.BestGenomeSpeciesIdx);

            // Assign a high fitness to one of the genomes, and check that BestGenomeSpeciesIdx is updated accordingly.
            neatPop.SpeciesArray[0].GenomeList[2].FitnessInfo = new FitnessInfo(100.0);

            // Note. The species must be re-initialised in order for the fit genome to be sorted correctly within its
            // containing species.
            InitialiseSpecies(neatPop);

            neatPop.UpdateStats(PrimaryFitnessInfoComparer.Singleton, rng);
            Assert.Equal(0, neatPop.NeatPopulationStats.BestGenomeSpeciesIdx);

            // Perform the same test again with the best genome in the second species.
            neatPop.SpeciesArray[1].GenomeList[3].FitnessInfo = new FitnessInfo(200.0);
            InitialiseSpecies(neatPop);
            neatPop.UpdateStats(PrimaryFitnessInfoComparer.Singleton, rng);
            Assert.Equal(1, neatPop.NeatPopulationStats.BestGenomeSpeciesIdx);
        }

        #endregion

        #region Private Static Methods

        private static NeatPopulation<double> CreateNeatPopulation(
            int count,
            double defaultFitness,
            IRandomSource rng)
        {
            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 3,
                outputNodeCount: 2,
                isAcyclic: true,
                activationFn: new NeuralNets.Double.ActivationFunctions.ReLU());

            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 1.0, count, rng);
            Assert.Equal(count, neatPop.GenomeList.Count);
            Assert.Equal(count, neatPop.GenomeIdSeq.Peek);

            // Assign the default fitness to all genomes.
            var genomeList = neatPop.GenomeList;
            for(int i=0; i < count; i++)
            {
                var genome = genomeList[i];
                genome.FitnessInfo = new FitnessInfo(defaultFitness);
            }

            // Init species.
            InitialiseSpecies(neatPop);

            return neatPop;
        }

        private static void InitialiseSpecies(NeatPopulation<double> neatPop)
        {
            // Create a speciation strategy instance.
            var distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            var speciationStrategy = new Speciation.GeneticKMeans.Parallelized.GeneticKMeansSpeciationStrategy<double>(distanceMetric, 5, 4);

            // Apply the speciation strategy.
            var genomeComparerDescending = new GenomeComparerDescending(PrimaryFitnessInfoComparer.Singleton);
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);
            neatPop.InitialiseSpecies(speciationStrategy, 3, genomeComparerDescending, rng);
        }

        #endregion
    }
}
