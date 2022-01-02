using System.Runtime.InteropServices;
using Redzen.Numerics.Distributions.Double;
using Redzen.Random;
using Redzen.Sorting;
using SharpNeat.Evaluation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using Xunit;

namespace SharpNeat.Neat.Speciation.Tests
{
    public class SpeciesTests
    {
        #region Test Methods

        [Fact]
        public void SortByPrimaryFitness()
        {
            const double champFitness = 100.0;

            var genomeComparerDescending = new GenomeComparerDescending(PrimaryFitnessInfoComparer.Singleton);
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            // Run the inner test multiple times, with a different champ genome count each time.
            for(int champGenomeCount=1; champGenomeCount <= 10; champGenomeCount++)
            {
                Species<double> species = CreateTestSpecies(10);

                AssignGenomeFitnessScores(species, champGenomeCount, champFitness, rng);
                SortUtils.SortUnstable(
                    CollectionsMarshal.AsSpan(species.GenomeList),
                    genomeComparerDescending,
                    rng);

                // Assert that the champ genomes have been sorted to the head of the genome list.
                int idx = 0;
                for(; idx < champGenomeCount; idx++) {
                    Assert.Equal(champFitness, species.GenomeList[idx].FitnessInfo.PrimaryFitness);
                }

                // Assert that all other genomes have a fitness less than the champ fitness.
                for(; idx < species.GenomeList.Count; idx++) {
                    Assert.True(species.GenomeList[idx].FitnessInfo.PrimaryFitness < champFitness);
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
                genome.FitnessInfo = new FitnessInfo(fitness);
            }

            // Select a random subset to be the champ genomes, and assign them the champ fitness.
            int[] idxArr = DiscreteDistribution.SampleUniformWithoutReplacement(rng, species.GenomeList.Count, champGenomeCount);
            foreach(int idx in idxArr)
            {
                var genome = species.GenomeList[idx];
                genome.FitnessInfo = new FitnessInfo(champFitness);
            }
        }

        private static Species<double> CreateTestSpecies(int genomeCount)
        {
            // Create the species genomes; we use NeatPopulationFactory for this.
            MetaNeatGenome<double> metaNeatGenome = new(
                inputNodeCount: 1,
                outputNodeCount: 1,
                isAcyclic: true,
                activationFn: new SharpNeat.NeuralNets.Double.ActivationFunctions.ReLU());

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
