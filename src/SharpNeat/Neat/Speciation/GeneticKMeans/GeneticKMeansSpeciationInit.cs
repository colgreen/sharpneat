/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Redzen.Linq;
using Redzen.Numerics.Distributions;
using Redzen.Random;
using SharpNeat.Neat.DistanceMetrics;
using SharpNeat.Neat.Genome;
using static SharpNeat.Neat.Speciation.SpeciationUtils;

namespace SharpNeat.Neat.Speciation.GeneticKMeans
{
    /// <summary>
    /// GeneticKMeansSpeciationStrategy initialisation.
    /// 
    /// This class handles initialising the k-means clusters (species), i.e. it takes a population of genomes
    /// and forms an initial set of k-means clusters with which to begin running the k-means iterations upon.
    /// upon.
    /// 
    /// Note. this implementation applies a modified version of the k-means++ initialisation method.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    internal class GeneticKMeansSpeciationInit<T> where T : struct
    {
        readonly IDistanceMetric<T> _distanceMetric;

        #region Constructors

        public GeneticKMeansSpeciationInit(IDistanceMetric<T> distanceMetric)
        {
            _distanceMetric = distanceMetric ?? throw new ArgumentNullException(nameof(distanceMetric));
        }

        #endregion

        #region Public Methods

        public Species<T>[] InitialiseSpecies(
            IList<NeatGenome<T>> genomeList,
            int speciesCount,
            IRandomSource rng)
        {
            // Create an array of seed genomes, i.e. each of these genomes will become the initial 
            // seed/centroid of one species.
            var seedGenomeList = new List<NeatGenome<T>>(speciesCount);

            // Create a list of genomes to select and remove seed genomes from.
            var remainingGenomes = new List<NeatGenome<T>>(genomeList);

            // Select first genome at random.
            seedGenomeList.Add(GetAndRemove(remainingGenomes, rng.Next(remainingGenomes.Count)));

            // Select all other seed genomes using k-means++ method.
            for(int i=1; i < speciesCount; i++)
            {
                var seedGenome = GetSeedGenome(seedGenomeList, remainingGenomes, rng);
                seedGenomeList.Add(seedGenome);
            }

            // Create an array of species initialised with the chosen seed genomes.

            // Each species is created with an initial capacity that will reduce the need for memory
            // reallocation but that isn't too wasteful of memory.
            int initialCapacity = (genomeList.Count * 2) / speciesCount;

            var speciesArr = new Species<T>[speciesCount];
            for(int i=0; i < speciesCount; i++) 
            {
                var seedGenome = seedGenomeList[i];
                speciesArr[i] = new Species<T>(i, seedGenome.ConnectionGenes, initialCapacity);
                speciesArr[i].GenomeList.Add(seedGenome);
            }

            // Allocate all other genomes to the species centroid they are nearest too.
            foreach(var genome in remainingGenomes)
            {
                var nearestSpeciesIdx = GetNearestSpecies(_distanceMetric, genome, speciesArr);
                speciesArr[nearestSpeciesIdx].GenomeList.Add(genome);
            }

            // Recalc species centroids.
            Array.ForEach(speciesArr, x => x.Centroid = _distanceMetric.CalculateCentroid(x.GenomeList.Select(y => y.ConnectionGenes)));

            return speciesArr;
        }

        #endregion

        #region Private Methods 

        private NeatGenome<T> GetSeedGenome(
            List<NeatGenome<T>> seedGenomeList,
            List<NeatGenome<T>> remainingGenomes,
            IRandomSource rng)
        {
            // Select from a random subset of remainingGenomes rather than the full set, otherwise
            // k-means will have something like O(n^2) scalability
            int subsetCount;
            
            // For 10 or fewer genomes just select all of them.
            if(remainingGenomes.Count <= 10) {
                subsetCount = remainingGenomes.Count;
            }
            else 
            {   // For more than ten remainingGenomes we choose a subset size proportional to log(count).
                subsetCount = (int)(Math.Log10(remainingGenomes.Count) * 10.0);
            }
                        
            // Get the indexes of a random subset of remainingGenomes.
            int[] genomeIdxArr = EnumerableUtils.RangeRandomOrder(0, remainingGenomes.Count, rng).Take(subsetCount).ToArray();

            // Create an array of relative selection probabilities for the candidate genomes.
            double[] pArr = new double[subsetCount];

            for(int i=0; i < subsetCount; i++)
            {
                // Note. k-means++ assigns a probability that is the squared distance to the nearest existing centroid.
                double distance = GetDistanceFromNearestSeed(seedGenomeList, remainingGenomes[genomeIdxArr[i]]); 
                pArr[i] = distance * distance; 
            }

            // Select a remaining genome at random based on pArr; remove it from remainingGenomes and return it.
            int selectIdx = DiscreteDistribution.Sample(rng, new DiscreteDistribution(pArr));
            return GetAndRemove(remainingGenomes, genomeIdxArr[selectIdx]);
        }

        private double GetDistanceFromNearestSeed(List<NeatGenome<T>> seedGenomeList, NeatGenome<T> genome)
        {
            double minDistance = _distanceMetric.CalcDistance(seedGenomeList[0].ConnectionGenes, genome.ConnectionGenes);

            for(int i=1; i < seedGenomeList.Count; i++) 
            {
                double distance = _distanceMetric.CalcDistance(seedGenomeList[i].ConnectionGenes, genome.ConnectionGenes);
                minDistance = Math.Min(minDistance, distance);
            }
            return minDistance;
        }

        private static U GetAndRemove<U>(IList<U> list, int idx)
        {
            U tmp = list[idx];
            list.RemoveAt(idx);
            return tmp;
        }

        #endregion
    }
}
