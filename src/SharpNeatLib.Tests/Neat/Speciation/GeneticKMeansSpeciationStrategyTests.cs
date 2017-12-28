using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using SharpNeat.Neat;
using SharpNeat.Neat.DistanceMetrics;
using SharpNeat.Neat.DistanceMetrics.Double;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Speciation;
using static SharpNeat.Neat.Speciation.GeneticKMeansSpeciationStrategyUtils;

namespace SharpNeatLib.Tests.Neat.Speciation
{
    [TestClass]
    public class GeneticKMeansSpeciationStrategyTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAll_Manhattan()
        {
            var rng = new XorShiftRandom();
            var distanceMetric = new ManhattanDistanceMetric();
            TestSpeciateAll(100, 3, 2, 0.5, distanceMetric, rng);
            TestSpeciateAll(100, 10, 10, 0.2, distanceMetric, rng);
            TestSpeciateAll(100, 30, 10, 0.1, distanceMetric, rng);
        }

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAll_Euclidean()
        {
            var rng = new XorShiftRandom();
            var distanceMetric = new EuclideanDistanceMetric();
            TestSpeciateAll(100, 3, 2, 0.5, distanceMetric, rng);
            TestSpeciateAll(100, 10, 10, 0.2, distanceMetric, rng);
            TestSpeciateAll(100, 30, 10, 0.1, distanceMetric, rng);
        }

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAdd_Manhattan()
        {
            var rng = new XorShiftRandom();
            var distanceMetric = new ManhattanDistanceMetric();
            TestSpeciateAdd(100, 3, 2, 0.5, distanceMetric, rng);
            TestSpeciateAdd(100, 10, 10, 0.2, distanceMetric, rng);
            TestSpeciateAdd(100, 30, 10, 0.1, distanceMetric, rng);
        }

        [TestMethod]
        [TestCategory("Speciation")]
        public void TestSpeciateAdd_Euclidean()
        {
            var rng = new XorShiftRandom();
            var distanceMetric = new EuclideanDistanceMetric();
            TestSpeciateAdd(100, 3, 2, 0.5, distanceMetric, rng);
            TestSpeciateAdd(100, 10, 10, 0.2, distanceMetric, rng);
            TestSpeciateAdd(100, 30, 10, 0.1, distanceMetric, rng);
        }

        #endregion

        #region Private Methods

        private void TestSpeciateAll(
            int popSize,
            int inputNodeCount,
            int outputNodeCount,
            double connectionsProportion,
            IDistanceMetric<double> distanceMetric,
            IRandomSource rng)
        {
            // Create population.
            NeatPopulation<double> neatPop = CreateNeatPopulation(popSize, inputNodeCount, outputNodeCount, connectionsProportion);

            // Create speciation strategy.
            var speciationStrategy = new GeneticKMeansSpeciationStrategy<double>(distanceMetric, 20);

            for(int i=0; i < 6; i++)
            {
                int speciesCount = rng.Next(1, (neatPop.GenomeList.Count/4)+1);

                // Invoke speciation strategy.
                var speciesArr = speciationStrategy.SpeciateAll(neatPop.GenomeList, speciesCount);

                // Perform tests.
                ValidationTests(speciesArr, distanceMetric, speciesCount, neatPop.GenomeList);
            }
        }

        private void TestSpeciateAdd(
            int popSize,
            int inputNodeCount,
            int outputNodeCount,
            double connectionsProportion,
            IDistanceMetric<double> distanceMetric,
            IRandomSource rng)
        {
            // Create population.
            NeatPopulation<double> neatPop = CreateNeatPopulation(popSize, inputNodeCount, outputNodeCount, connectionsProportion);

            // Split the population into three.
            int popSize1 = popSize / 3;
            int popSize2 = popSize / 3;
            int popSize3 = popSize - (popSize1 + popSize2);

            var genomeList1 = neatPop.GenomeList.GetRange(0, popSize1);
            var genomeList2 = neatPop.GenomeList.GetRange(popSize1, popSize2);
            var genomeList3 = neatPop.GenomeList.GetRange(popSize1 + popSize2, popSize3);

            // Create speciation strategy.
            var speciationStrategy = new GeneticKMeansSpeciationStrategy<double>(distanceMetric, 20);

            for (int i = 0; i < 6; i++)
            {
                int speciesCount = rng.Next(1, (neatPop.GenomeList.Count / 4) + 1);

                var fullGenomeList = new List<NeatGenome<double>>(genomeList1);

                // Invoke speciation strategy, and run tests
                var speciesArr = speciationStrategy.SpeciateAll(genomeList1, speciesCount);
                ValidationTests(speciesArr, distanceMetric, speciesCount, fullGenomeList);

                // Add second batch of genomes, and re-run tests.
                speciationStrategy.SpeciateAdd(genomeList2, speciesArr);

                fullGenomeList.AddRange(genomeList2);
                ValidationTests(speciesArr, distanceMetric, speciesCount, fullGenomeList);

                // Add third batch of genomes, and re-run tests.
                speciationStrategy.SpeciateAdd(genomeList3, speciesArr);

                fullGenomeList.AddRange(genomeList3);
                ValidationTests(speciesArr, distanceMetric, speciesCount, fullGenomeList);
            }
        }

        private void ValidationTests(
            Species<double>[] speciesArr, 
            IDistanceMetric<double> distanceMetric,
            int speciesCountExpected,
            List<NeatGenome<double>> fullGenomeList)
        {
            // Confirm correct number of species.
            Assert.AreEqual(speciesCountExpected, speciesArr.Length);

            // Confirm no empty species.
            int minSpeciesSize = speciesArr.Select(x => x.GenomeList.Count).Min();
            Assert.IsTrue(minSpeciesSize > 0);

            // Get IDs of all genomes in species.
            var idSet = GetAllGenomeIds(speciesArr);

            // Confirm number of IDs equals number of genomes in main population list.
            Assert.AreEqual(fullGenomeList.Count, idSet.Count);

            // Confirm the genome list IDs match up with the genomes in the species.
            fullGenomeList.ForEach(x => Assert.IsTrue(idSet.Contains(x.Id)));

            // Confirm all species centroids are correct.
            Array.ForEach(speciesArr, x => Assert.AreEqual(0.0, distanceMetric.GetDistance(x.Centroid, distanceMetric.CalculateCentroid(x.GenomeList.Select(y => y.ConnectionGenes)))));

            // Confirm all genomes are in the species with the nearest centroid.
            Array.ForEach(speciesArr, x => x.GenomeList.ForEach(y => Assert.AreEqual(x, GetNearestSpecies(y, speciesArr, distanceMetric, out int nearestSpeciesIdx))));
        }

        #endregion
        
        #region Private Static Methods [Utility Methods]

        private static NeatPopulation<double> CreateNeatPopulation(
            int count,
            int inputNodeCount,
            int outputNodeCount,
            double connectionsProportion)
        {
            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: inputNodeCount,
                outputNodeCount: outputNodeCount,
                isAcyclic: true,
                activationFn: new SharpNeat.NeuralNets.Double.ActivationFunctions.ReLU());

            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 1.0, count);
            return neatPop;
        }

        private static HashSet<int> GetAllGenomeIds(Species<double>[] speciesArr)
        {
            var idSet = new HashSet<int>();
            Array.ForEach(speciesArr, x => x.GenomeList.ForEach(y => idSet.Add(y.Id)));
            return idSet;
        }

        #endregion
    }
}
