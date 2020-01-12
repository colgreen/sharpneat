using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Numerics.Distributions;
using Redzen.Random;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Speciation;

namespace SharpNeatLib.Tests.Neat.Speciation
{
    [TestClass]
    public class SpeciesTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Species")]
        public void SortByPrimaryFitness()
        {
            const double champFitness = 100.0;

            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            // Run the inner test multiple times, with a different champ genome count each time.
            for(int champGenomeCount=1; champGenomeCount <= 10; champGenomeCount++)
            { 
                Species<double> species = CreateTestSpecies(10);

                AssignGenomeFitnessScores(species, champGenomeCount, champFitness, rng);
                species.SortByPrimaryFitness(rng);

                // Assert that the champ genomes have been sorted to the head of the genome list.
                int idx = 0;
                for(; idx < champGenomeCount; idx++) {
                    Assert.AreEqual(champFitness, species.GenomeList[idx].FitnessInfo.PrimaryFitness);
                }

                // Assert that all other genomes have a fitness less than the champ fitness.
                for(; idx < species.GenomeList.Count; idx++) {
                    Assert.IsTrue(species.GenomeList[idx].FitnessInfo.PrimaryFitness < champFitness);    
                }
            }
        }

        #endregion

        #region Private Static Methods

        private static void AssignGenomeFitnessScores(
            Species<double> species,
            int champGenomeCount,
            double champFitness,
            IRandomSource rng)
        {
            // Assign a fitness less than the champ fitness to all genomes.
            foreach(var genome in species.GenomeList)
            { 
                double fitness = champFitness * rng.NextDouble();
                genome.FitnessInfo = new SharpNeat.Evaluation.FitnessInfo(fitness);
            }

            // Select a random subset to be the champ genomes, and assign them the champ fitness.
            int[] idxArr = DiscreteDistribution.SampleUniformWithoutReplacement(rng, species.GenomeList.Count, champGenomeCount);
            foreach(int idx in idxArr)
            {
                var genome = species.GenomeList[idx];
                genome.FitnessInfo = new SharpNeat.Evaluation.FitnessInfo(champFitness);
            }
        }

        private static Species<double> CreateTestSpecies(int genomeCount)
        {
            // Create the species genomes; we use NeatPopulationFactory for this.
            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 1,
                outputNodeCount: 1,
                isAcyclic: true,
                activationFn: new SharpNeat.NeuralNet.Double.ActivationFunctions.ReLU());

            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 1.0, genomeCount, RandomDefaults.CreateRandomSource());

            // Create the species object, and assign all of the created genomes to it.
            var species = new Species<double>(0, new ConnectionGenes<double>(0));
            species.GenomeList.AddRange(neatPop.GenomeList);

            // Return the species.
            return species;
        }

        #endregion
    }
}
