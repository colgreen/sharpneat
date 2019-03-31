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
using System;
using System.Collections.Generic;
using Redzen.Structures;
using SharpNeat.Evaluation;

namespace SharpNeat.EvolutionAlgorithm
{
    /// <summary>
    /// A population of genomes.
    /// </summary>
    /// <typeparam name="TGenome">Genome type.</typeparam>
    public class Population<TGenome> where TGenome : IGenome
    {
        #region Auto Properties

        /// <summary>
        /// The list of genomes that make up the population.
        /// </summary>
        public List<TGenome> GenomeList { get; }

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
        public PopulationStats Stats { get; }

        // TODO: Consider if this belongs on NeatEvolutionAlgorithm.
        /// <summary>
        /// A sequence that provides the current generation number.
        /// </summary>
        public Int32Sequence GenerationSeq { get; }

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
            this.GenerationSeq = new Int32Sequence();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the population statistics.
        /// </summary>
        /// <param name="fitnessComparer">A genome fitness comparer.</param>
        public virtual void UpdateStats(IComparer<FitnessInfo> fitnessComparer)
        {
            // TODO: Unit test

            // Determine best fitness and sum(fitness).
            List<TGenome> genomeList = this.GenomeList;

            TGenome bestGenome = genomeList[0];
            int bestGenomeIdx = 0;
            FitnessInfo bestFitness = bestGenome.FitnessInfo;
            double bestGenomeComplexity = bestGenome.Complexity;
            double primaryFitnessSum = bestFitness.PrimaryFitness;
            double complexitySum = bestGenome.Complexity;

            int count = genomeList.Count;
            for(int i=1; i < count; i++)
            {
                TGenome genome = genomeList[i];

                if(fitnessComparer.Compare(genome.FitnessInfo, bestFitness) > 0) 
                {
                    bestGenomeIdx = i;
                    bestGenome = genome;
                    bestFitness = genome.FitnessInfo;
                }

                primaryFitnessSum += genome.FitnessInfo.PrimaryFitness;
                complexitySum += genome.Complexity;
            }

            // Update population stats object.
            PopulationStats stats = this.Stats;

            // Fitness stats.
            stats.BestGenomeIndex = bestGenomeIdx;
            stats.BestFitness = bestFitness;
            stats.MeanFitness = primaryFitnessSum / count;
            stats.BestFitnessHistory.Enqueue(bestFitness.PrimaryFitness);

            // Complexity stats.
            stats.BestComplexity = bestGenome.Complexity;
            double meanComplexity = complexitySum / count;
            stats.MeanComplexity = meanComplexity;
            stats.MeanComplexityHistory.Enqueue(meanComplexity);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Create a new population statistics object.
        /// </summary>
        /// <returns>A new instance of <see cref="Stats"/>.</returns>
        protected virtual PopulationStats CreatePopulatonStats()
        {
            return new PopulationStats();
        }

        #endregion
    }
}
