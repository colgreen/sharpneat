using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using SharpNeat.Evaluation;
using SharpNeat.Neat;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Tests.Neat.EvolutionAlgorithm
{
    [TestClass]
    public class SpeciesAllocationCalcsTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("SelectionReproduction")]
        public void TestSpeciesAllocation()
        {
            NeatEvolutionAlgorithmSettings eaSettings = new NeatEvolutionAlgorithmSettings() {
                SpeciesCount = 4
            };

            // Create population.
            NeatPopulation<double> neatPop = CreateNeatPopulation(100, eaSettings.SpeciesCount, 2, 2, 1.0);

            // Manually set-up some species.
            var speciesArr = neatPop.SpeciesArray;

            speciesArr[0].GenomeList.AddRange(neatPop.GenomeList.Take(25));
            speciesArr[1].GenomeList.AddRange(neatPop.GenomeList.Skip(25).Take(25));
            speciesArr[2].GenomeList.AddRange(neatPop.GenomeList.Skip(50).Take(25));
            speciesArr[3].GenomeList.AddRange(neatPop.GenomeList.Skip(75).Take(25));

            // Manually assign fitness scores to the genomes.
            speciesArr[0].GenomeList.ForEach(x => x.FitnessInfo = new FitnessInfo(100.0));
            speciesArr[1].GenomeList.ForEach(x => x.FitnessInfo = new FitnessInfo(200.0));
            speciesArr[2].GenomeList.ForEach(x => x.FitnessInfo = new FitnessInfo(400.0));
            speciesArr[3].GenomeList.ForEach(x => x.FitnessInfo = new FitnessInfo(800.0));

            // Invoke species target size calcs.
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            SpeciesStatsCalcs<double>.CalcAndStoreSpeciesStats(neatPop, eaSettings, rng);

            // Species target sizes should be relative to the species mean fitness.
            double totalMeanFitness = 1500.0;
            double popSize = 100.0;

            Assert.AreEqual((100.0 / totalMeanFitness) * popSize, speciesArr[0].Stats.TargetSizeReal);
            Assert.AreEqual((200.0 / totalMeanFitness) * popSize, speciesArr[1].Stats.TargetSizeReal);
            Assert.AreEqual((400.0 / totalMeanFitness) * popSize, speciesArr[2].Stats.TargetSizeReal);
            Assert.AreEqual((800.0 / totalMeanFitness) * popSize, speciesArr[3].Stats.TargetSizeReal);

            // Note. Discretized target sizes will generally be equal to ceil(TargetSizeReal) or floor(TargetSizeReal),
            // but may not be due to the target size adjustment logic that is used to ensure that sum(TargetSizeInt) is equal
            // to the required population size.

            // Check that sum(TargetSizeInt) is equal to the required population size.
            Assert.AreEqual(speciesArr.Sum(x => x.Stats.TargetSizeInt), neatPop.GenomeList.Count);
        }

        #endregion

        #region Private Static Methods

        private static NeatPopulation<double> CreateNeatPopulation(
            int populationSize,
            int speciesCount,
            int inputNodeCount,
            int outputNodeCount,
            double connectionsProportion)
        {
            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: inputNodeCount,
                outputNodeCount: outputNodeCount,
                isAcyclic: true,
                activationFn: new SharpNeat.NeuralNet.Double.ActivationFunctions.ReLU());

            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 1.0, populationSize, RandomDefaults.CreateRandomSource());
            neatPop.SpeciesArray = new Species<double>[speciesCount];

            for(int i=0; i < speciesCount; i++) {
                neatPop.SpeciesArray[i] = new Species<double>(i, null);
            }

            return neatPop;
        }

        #endregion
    }
}
