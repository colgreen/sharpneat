using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using SharpNeat.Neat;
using SharpNeat.Neat.DistanceMetrics;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Tests.Neat.Speciation
{
    internal static class SpeciationStrategyTestUtils
    {
        #region Private Methods

        public static void TestSpeciateAll(
            int popSize,
            int inputNodeCount,
            int outputNodeCount,
            double connectionsProportion,
            IDistanceMetric<double> distanceMetric,
            ISpeciationStrategy<NeatGenome<double>,double> speciationStrategy,
            IRandomSource rng,
            bool validateNearestSpecies = true)
        {
            // Create population.
            NeatPopulation<double> neatPop = CreateNeatPopulation(popSize, inputNodeCount, outputNodeCount, connectionsProportion);

            for(int i=0; i < 6; i++)
            {
                int speciesCount = rng.Next(1, (neatPop.GenomeList.Count/4)+1);

                // Invoke speciation strategy.
                var speciesArr = speciationStrategy.SpeciateAll(neatPop.GenomeList, speciesCount, rng);

                // Perform tests.
                ValidationTests(speciesArr, distanceMetric, speciesCount, neatPop.GenomeList, validateNearestSpecies);
            }
        }

        public static void TestSpeciateAdd(
            int popSize,
            int inputNodeCount,
            int outputNodeCount,
            double connectionsProportion,
            IDistanceMetric<double> distanceMetric,
            ISpeciationStrategy<NeatGenome<double>,double> speciationStrategy,
            IRandomSource rng,
            bool validateNearestSpecies = true)
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

            for (int i = 0; i < 6; i++)
            {
                int speciesCount = rng.Next(1, (neatPop.GenomeList.Count / 4) + 1);

                var fullGenomeList = new List<NeatGenome<double>>(genomeList1);

                // Invoke speciation strategy, and run tests
                var speciesArr = speciationStrategy.SpeciateAll(genomeList1, speciesCount, rng);
                ValidationTests(speciesArr, distanceMetric, speciesCount, fullGenomeList, validateNearestSpecies);

                // Add second batch of genomes, and re-run tests.
                speciationStrategy.SpeciateAdd(genomeList2, speciesArr, rng);

                fullGenomeList.AddRange(genomeList2);
                ValidationTests(speciesArr, distanceMetric, speciesCount, fullGenomeList, validateNearestSpecies);

                // Add third batch of genomes, and re-run tests.
                speciationStrategy.SpeciateAdd(genomeList3, speciesArr, rng);

                fullGenomeList.AddRange(genomeList3);
                ValidationTests(speciesArr, distanceMetric, speciesCount, fullGenomeList, validateNearestSpecies);
            }
        }

        public static void ValidationTests(
            Species<double>[] speciesArr, 
            IDistanceMetric<double> distanceMetric,
            int speciesCountExpected,
            List<NeatGenome<double>> fullGenomeList,
            bool validateNearestSpecies)
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
            Array.ForEach(speciesArr, x => Assert.AreEqual(0.0, distanceMetric.CalcDistance(x.Centroid, distanceMetric.CalculateCentroid(x.GenomeList.Select(y => y.ConnectionGenes)))));

            if(validateNearestSpecies)
            {
                // Confirm all genomes are in the species with the nearest centroid.
                // Note. If there are two or more species that are equally near then we test that a genome is in one of those.
                Array.ForEach(speciesArr, species => species.GenomeList.ForEach(genome => Assert.IsTrue(GetNearestSpeciesList(genome, speciesArr, distanceMetric).Contains(species))));
            }
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
                activationFn: new SharpNeat.NeuralNet.Double.ActivationFunctions.ReLU());

            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, connectionsProportion, count, RandomDefaults.CreateRandomSource());
            return neatPop;
        }

        private static HashSet<int> GetAllGenomeIds(Species<double>[] speciesArr)
        {
            var idSet = new HashSet<int>();
            Array.ForEach(speciesArr, x => x.GenomeList.ForEach(y => idSet.Add(y.Id)));
            return idSet;
        }

        /// <summary>
        /// Gets the species with a centroid closest to the given genome.
        /// If multiple species are equally close then we return all of the those species.
        /// </summary>
        public static List<Species<T>> GetNearestSpeciesList<T>(
            NeatGenome<T> genome,
            Species<T>[] speciesArr,
            IDistanceMetric<T> distanceMetric)
        where T : struct
        {
            var nearestSpeciesList = new List<Species<T>>(4) {
                speciesArr[0]
            };
            double nearestDistance = distanceMetric.CalcDistance(genome.ConnectionGenes, speciesArr[0].Centroid);

            for(int i=1; i < speciesArr.Length; i++)
            {
                double distance = distanceMetric.CalcDistance(genome.ConnectionGenes, speciesArr[i].Centroid);
                if(distance < nearestDistance)
                {
                    nearestSpeciesList.Clear();
                    nearestSpeciesList.Add(speciesArr[i]);
                    nearestDistance = distance;
                }
                else if(distance == nearestDistance)
                {
                    nearestSpeciesList.Add(speciesArr[i]);
                }
            }
            return nearestSpeciesList;
        }

        #endregion
    }
}
