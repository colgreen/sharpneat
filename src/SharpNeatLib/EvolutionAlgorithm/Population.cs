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
using System.Diagnostics;
using Redzen.Random;
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
        public PopulationStatistics Stats { get; }

        #endregion

        #region Instance Fields

        /// <summary>
        /// A reusable/working list. Stores the index of the genome with the best fitness, or multiple indexes when two or more genomes have 
        /// the best fitness score.
        /// </summary>
        private readonly List<int> _fittestGenomeIndexList;
        
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
            _fittestGenomeIndexList = new List<int>(genomeList.Count);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the population statistics.
        /// </summary>
        /// <param name="fitnessComparer">A genome fitness comparer.</param>
        /// <param name="rng">Random source.</param>
        public virtual void UpdateStats(IComparer<FitnessInfo> fitnessComparer, IRandomSource rng)
        {
            // TODO: Unit tests.

            // Ensure working list is empty.
            _fittestGenomeIndexList.Clear();

            // Determine best fitness, sum of PrimaryFitness, and sum of Complexity.
            List<TGenome> genomeList = this.GenomeList;
            Debug.Assert(genomeList.Count != 0);

            // Assume genome zero is the fittest until we find a fitter genome.
            _fittestGenomeIndexList.Add(0);
            FitnessInfo bestFitness = genomeList[0].FitnessInfo;

            // Keep a running sum of genome primary fitness and complexity.
            double primaryFitnessSum = genomeList[0].FitnessInfo.PrimaryFitness;
            double complexitySum = genomeList[0].Complexity;

            // Loop all other genomes.
            int count = genomeList.Count;
            for(int idx=1; idx < count; idx++)
            {
                TGenome genome = genomeList[idx];
                int comparisonResult = fitnessComparer.Compare(genome.FitnessInfo, bestFitness);

                if(comparisonResult > 0)
                {
                    // A new best fitness has been found.
                    _fittestGenomeIndexList.Clear();
                    _fittestGenomeIndexList.Add(idx);
                    bestFitness = genome.FitnessInfo;
                }
                else if(comparisonResult == 0)
                {
                    // Add genome to the list of fittest candidates.
                    _fittestGenomeIndexList.Add(idx);
                }

                // Update running sums of genome primary fitness and complexity.
                primaryFitnessSum += genome.FitnessInfo.PrimaryFitness;
                complexitySum += genome.Complexity;
            }

            // Select a single best genome; if there are more than one with the highest fitness, then select one at random.
            int bestGenomeIdx;
            if(_fittestGenomeIndexList.Count == 1)
            {   
                // There is a single fittest genome; select it.
                bestGenomeIdx = _fittestGenomeIndexList[0];
            }
            else
            {
                // Select one of the fittest genomes at random.
                bestGenomeIdx = _fittestGenomeIndexList[rng.Next(_fittestGenomeIndexList.Count)];
            }
            TGenome bestGenome = genomeList[bestGenomeIdx];

            // Update population stats object.
            PopulationStatistics stats = this.Stats;

            // Update fitness stats.
            stats.BestGenomeIndex = bestGenomeIdx;
            stats.BestFitness = bestGenome.FitnessInfo;
            stats.MeanFitness = primaryFitnessSum / count;
            stats.BestFitnessHistory.Enqueue(bestGenome.FitnessInfo.PrimaryFitness);

            // Update complexity stats.
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
        protected virtual PopulationStatistics CreatePopulatonStats()
        {
            return new PopulationStatistics();
        }

        #endregion
    }
}
