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
using Redzen.Random;
using SharpNeat.Evaluation;

namespace SharpNeat.EvolutionAlgorithm
{
    /// <summary>
    /// A population of genomes.
    /// </summary>
    /// <typeparam name="TGenome">Genome type.</typeparam>
    public abstract class Population<TGenome> where TGenome : IGenome
    {
        #region Auto Properties 

        /// <summary>
        /// The list of genomes that make up the population.
        /// </summary>
        public List<TGenome> GenomeList { get; }

        /// <summary>
        /// Gets the current best genome.
        /// </summary>
        /// <remarks>
        /// Note. If the evolution algorithm has not yet been initialised then this will simply return the genome at index zero in the population.
        /// </remarks>
        public TGenome BestGenome => this.GenomeList[this.Stats.BestGenomeIndex];

        /// <summary>
        /// The number of genomes in the population.
        /// </summary>
        /// <remarks>
        /// This value is set and fixed to be the length of <see cref="GenomeList"/> at construction time.
        /// During certain phases of the evolution algorithm the length of <see cref="GenomeList"/> will vary and therefore it may not match
        /// <see cref="PopulationSize"/> at any given point in time, thus this property is the definitive source of the population size.
        /// </remarks>
        public int PopulationSize { get; }

        /// <summary>
        /// Population statistics.
        /// </summary>
        public PopulationStatistics Stats { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct the population with the provided list of genomes that make up the initial population.
        /// </summary>
        /// <param name="genomeList"></param>
        public Population(List<TGenome> genomeList)
        {
            this.GenomeList = genomeList ?? throw new ArgumentNullException(nameof(genomeList));
            if(genomeList.Count == 0) {
                throw new ArgumentException("Empty genome list. The initial population cannot be empty.", nameof(genomeList));
            }

            this.PopulationSize = genomeList.Count;
            this.Stats = CreatePopulatonStats();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the population statistics.
        /// </summary>
        /// <param name="fitnessComparer">A genome fitness comparer.</param>
        /// <param name="rng">Random source.</param>
        public abstract void UpdateStats(IComparer<FitnessInfo> fitnessComparer, IRandomSource rng);

        #endregion

        #region Protected Methods

        /// <summary>
        /// Create a new population statistics object.
        /// </summary>
        /// <returns>A new instance of <see cref="Stats"/>.</returns>
        protected virtual PopulationStatistics CreatePopulatonStats()
        {
            return new PopulationStatistics();
        }

        #endregion
    }
}
