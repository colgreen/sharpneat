/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Collections.Generic;
using Redzen.Random;
using Redzen.Sorting;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Speciation
{
    /// <summary>
    /// Represents a NEAT species.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public class Species<T> where T : struct
    {
        #region Auto Properties

        /// <summary>
        /// Species ID.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Species centroid.
        /// </summary>
        public ConnectionGenes<T> Centroid { get; set; }

        /// <summary>
        /// The genomes that are within the species.
        /// </summary>
        public List<NeatGenome<T>> GenomeList { get; }

        /// <summary>
        /// A working dictionary of genomes keyed by ID.
        /// </summary>
        public Dictionary<int,NeatGenome<T>> GenomeById { get; }

        /// <summary>
        /// Working list of genomes to be added to GenomeById at the end of a k-means iteration.
        /// </summary>
        public List<NeatGenome<T>> PendingAddsList { get; }

        /// <summary>
        /// Working list of genome IDs to remove from GenomeById at the end of a k-means iteration.
        /// </summary>
        public List<int> PendingRemovesList { get; }

        /// <summary>
        /// Species statistics.
        /// </summary>
        public SpeciesStats Stats { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the given species ID, centroid and initial capacity.
        /// </summary>
        /// <param name="id">Species ID.</param>
        /// <param name="centroid">Species centroid.</param>
        /// <param name="capacity">Initial capacity for the species genome list.</param>
        public Species(int id, ConnectionGenes<T> centroid, int capacity = 0)
        {
            this.Id = id;
            this.Centroid = centroid;
            this.GenomeList = new List<NeatGenome<T>>(capacity);
            this.GenomeById = new Dictionary<int,NeatGenome<T>>(capacity);
            this.PendingAddsList = new List<NeatGenome<T>>(capacity);
            this.PendingRemovesList = new List<int>(capacity);
            this.Stats = new SpeciesStats();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Transfer genomes from GenomeList into GenomeById.
        /// </summary>
        public void LoadWorkingDictionary()
        {
            GenomeById.Clear();
            foreach(var genome in GenomeList) {
                GenomeById.Add(genome.Id, genome);
            }
            GenomeList.Clear();
        }

        /// <summary>
        /// Transfer genomes from GenomeById into GenomeList.
        /// </summary>
        public void FlushWorkingDictionary()
        {
            GenomeList.Clear();
            GenomeList.AddRange(GenomeById.Values);
            GenomeById.Clear();
        }

        /// <summary>
        /// Complete all pending genome moves for this species.
        /// </summary>
        public void CompletePendingMoves()
        {
            // Remove genomes that are marked for removal.
            foreach(int id in PendingRemovesList) {
                GenomeById.Remove(id);
            }

            // Process pending additions.
            foreach(var genome in PendingAddsList) {
                GenomeById.Add(genome.Id, genome);
            }

            PendingRemovesList.Clear();
            PendingAddsList.Clear();
        }

        /// <summary>
        /// Sort the species genomes by primary fitness, highest fitness first.
        /// Additionally, shuffle the genomes with the best fitness if there are two or more with the highest fitness, this ensures
        /// that the first genome (and therefore the single species 'best' genome ) is randomized if there multiple candidates for the best genome.
        /// </summary>
        /// <param name="rng">Random source.</param>
        public void SortByPrimaryFitness(IRandomSource rng)
        {
            // Sort the genomes by fitness.
            this.GenomeList.Sort(GenomePrimaryFitnessComparer<T>.Singleton);

            // If there are two or more genomes with the highest fitness score, then randomly shuffle those genomes; this ensures that the single species
            // champion (i.e. at index zero) is randomized when there are multiple candidates for species champion.
            if(this.GenomeList.Count > 1 && this.GenomeList[0].FitnessInfo.PrimaryFitness == this.GenomeList[1].FitnessInfo.PrimaryFitness)
            {
                // Scan for the end of the contiguous segment.
                double champFitness = this.GenomeList[0].FitnessInfo.PrimaryFitness;
                int count = this.GenomeList.Count;
                int endIdx = 2;
                for(; endIdx < count &&   this.GenomeList[endIdx].FitnessInfo.PrimaryFitness == champFitness; endIdx++);

                // endIdx points to the item after the segment's end, so we decrement.
                endIdx--;

                // Shuffle the champion genome candidates.
                SortUtils.Shuffle(this.GenomeList, rng, 0, endIdx);
            }
        }

        #endregion
    }
}
