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
using System.Linq;
using Redzen.Random;
using Redzen.Sorting;
using Redzen.Structures;
using SharpNeat.Evaluation;
using SharpNeat.EvolutionAlgorithm;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Speciation;
using static SharpNeat.Neat.NeatPopulationUtils;

namespace SharpNeat.Neat
{
    /// <summary>
    /// A NEAT population.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public class NeatPopulation<T> : Population<NeatGenome<T>>
        where T : struct
    {
        #region Consts / Statics

        // ENHANCEMENT: Consider increasing buffer capacity, and different capacities for the two different buffers.
        const int __defaultInnovationHistoryBufferSize = 0x20000; // = 131,072.

        #endregion

        #region Auto Properties

        /// <summary>
        /// NeatGenome metadata.
        /// </summary>
        public MetaNeatGenome<T> MetaNeatGenome { get; }

        /// <summary>
        /// NeatGenome builder.
        /// </summary>
        public INeatGenomeBuilder<T> GenomeBuilder { get; }

        /// <summary>
        /// Genome ID sequence; for obtaining new genome IDs.
        /// </summary>
        public Int32Sequence GenomeIdSeq { get; }

        /// <summary>
        /// Innovation ID sequence; for obtaining new innovation IDs.
        /// </summary>
        public Int32Sequence InnovationIdSeq { get; }

        /// <summary>
        /// A history buffer of added nodes.
        /// Used when adding new nodes to check if an identical node has been added to a genome elsewhere in the population.
        /// This allows re-use of the same innovation ID for like nodes.
        /// </summary>
        public AddedNodeBuffer AddedNodeBuffer { get; }

        /// <summary>
        /// Species array.
        /// </summary>
        public Species<T>[] SpeciesArray { get; set; }

        /// <summary>
        /// NeatPopulation statistics.
        /// </summary>
        public NeatPopulationStatistics NeatPopulationStats => (NeatPopulationStatistics)this.Stats;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new population with the provided genomes.
        /// </summary>
        /// <param name="metaNeatGenome">NeatGenome metadata.</param>
        /// <param name="genomeBuilder">NeatGenome builder.</param>
        /// <param name="genomeList">A list of genomes that will make up the population.</param>
        public NeatPopulation(
            MetaNeatGenome<T> metaNeatGenome,
            INeatGenomeBuilder<T> genomeBuilder,
            List<NeatGenome<T>> genomeList) 
            : base(genomeList)
        {
            GetMaxObservedIds(genomeList, out int maxGenomeId, out int maxInnovationId);

            this.MetaNeatGenome = metaNeatGenome ?? throw new ArgumentNullException(nameof(metaNeatGenome));
            this.GenomeBuilder = genomeBuilder ?? throw new ArgumentNullException(nameof(genomeBuilder));
            this.GenomeIdSeq = new Int32Sequence(maxGenomeId + 1);
            this.InnovationIdSeq = new Int32Sequence(maxInnovationId + 1);
            this.AddedNodeBuffer = new AddedNodeBuffer(__defaultInnovationHistoryBufferSize);            
        }

        /// <summary>
        /// Construct a new population with the provided genomes and ID sequence objects.
        /// </summary>
        /// <param name="metaNeatGenome">NeatGenome metadata.</param>
        /// <param name="genomeBuilder">NeatGenome builder.</param>
        /// <param name="genomeList">A list of genomes that will make up the population.</param>
        /// <param name="genomeIdSeq">Genome ID sequence.</param>
        /// <param name="innovationIdSeq">Innovation ID sequence.</param>
        public NeatPopulation(
            MetaNeatGenome<T> metaNeatGenome,
            INeatGenomeBuilder<T> genomeBuilder,
            List<NeatGenome<T>> genomeList,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq)
        : this(metaNeatGenome, genomeBuilder, genomeList, genomeIdSeq, innovationIdSeq, __defaultInnovationHistoryBufferSize, __defaultInnovationHistoryBufferSize)
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaNeatGenome">NeatGenome metadata.</param>
        /// <param name="genomeBuilder">NeatGenome builder.</param>
        /// <param name="genomeList">A list of genomes that will make up the population.</param>
        /// <param name="genomeIdSeq">Genome ID sequence.</param>
        /// <param name="innovationIdSeq">Innovation ID sequence.</param>
        /// <param name="addedConnectionHistoryBufferSize">The size to allocate for the added connection history buffer.</param>
        /// <param name="addedNodeHistoryBufferSize">The size to allocate for the added node history buffer.</param>
        public NeatPopulation(
            MetaNeatGenome<T> metaNeatGenome,
            INeatGenomeBuilder<T> genomeBuilder,
            List<NeatGenome<T>> genomeList,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq,
            int addedConnectionHistoryBufferSize,
            int addedNodeHistoryBufferSize)
        : base(genomeList)
        {
            this.MetaNeatGenome = metaNeatGenome ?? throw new ArgumentNullException(nameof(metaNeatGenome));
            this.GenomeBuilder = genomeBuilder ?? throw new ArgumentNullException(nameof(genomeBuilder));
            this.GenomeIdSeq = genomeIdSeq ?? throw new ArgumentNullException(nameof(genomeIdSeq));
            this.InnovationIdSeq = innovationIdSeq ?? throw new ArgumentNullException(nameof(innovationIdSeq));;
            this.AddedNodeBuffer = new AddedNodeBuffer(addedNodeHistoryBufferSize);

            // Assert that the ID sequences have a current IDs higher than any existing ID.
            Debug.Assert(ValidateIdSequences(genomeList, genomeIdSeq, innovationIdSeq));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise (or re-initialise) the population species.
        /// </summary>
        /// <param name="speciationStrategy">The speciation strategy to use.</param>
        /// <param name="speciesCount">The required number of species.</param>
        /// <param name="rng">Random source.</param>
        public void InitialiseSpecies(
            ISpeciationStrategy<NeatGenome<T>,T> speciationStrategy,
            int speciesCount,
            IRandomSource rng)
        {
            // Allocate the genomes to species.
            Species<T>[] speciesArr = speciationStrategy.SpeciateAll(this.GenomeList, speciesCount, rng);
            if(null == speciesArr || speciesArr.Length != speciesCount) {
                throw new Exception("Species array is null or has incorrect length.");
            }
            this.SpeciesArray = speciesArr;

            // Sort the genomes in each species. Highest fitness first, then secondary sorted by youngest genomes first.
            foreach(Species<T> species in speciesArr) {
                SortUtils.SortUnstable(species.GenomeList, GenomeFitnessAndAgeComparer<T>.Singleton, rng);
            }
        }

        /// <summary>
        /// Update the population statistics object.
        /// </summary>
        /// <param name="fitnessComparer">A genome fitness comparer.</param>
        public override void UpdateStats(IComparer<FitnessInfo> fitnessComparer)
        {
            // Update non-NEAT based population stats.
            base.UpdateStats(fitnessComparer);

            // Calc NEAT based population stats.
            UpdateNeatPopulationStats(fitnessComparer);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Create a new population statistics object.
        /// </summary>
        /// <returns>A new instance of <see cref="PopulationStatistics"/>.</returns>
        protected override PopulationStatistics CreatePopulatonStats()
        {
            return new NeatPopulationStatistics();
        }

        #endregion

        #region Private Methods

        private void UpdateNeatPopulationStats(IComparer<FitnessInfo> fitnessComparer)
        {
            // TODO: Unit test

            // Calc NEAT based population stats.

            // Loop the species, and calculate/determine the following:
            // - Calculate the mean fitness of genomes in each species.
            // - Calculate sum of the species mean fitnesses.
            // - Identify the species that contains the best genome in the population as a whole.

            // Notes.
            // The best genome has already been identified by base.UpdatePopulationStats() (and the 
            // result stored in {PopulationStats.BestGenomeIndex})
            //
            // The genomes in each species are sorted by fitness, fittest genome first. Therefore we
            // don't need to scan the full genome list of each species, we can just scan as far as the
            // first genome with a fitness less than the best genome, noting that there may be multiple
            // genomes with a fitness equal to the single chosen 'best' genome.
            var bestGenome = this.GenomeList[this.Stats.BestGenomeIndex];
            double sumMeanFitness = 0.0;
            int bestGenomeSpeciesIdx = -1;

            Species<T>[] speciesArr = this.SpeciesArray;
            for(int i=0; i < speciesArr.Length; i++)
            {
                Species<T> species = speciesArr[i];

                // Calc mean of the primary fitness, store the result, and update the sum.
                double meanFitness = species.GenomeList.Average(x => x.FitnessInfo.PrimaryFitness);
                species.Stats.MeanFitness = meanFitness;
                sumMeanFitness += meanFitness;

                // Test if this species contains the best genome.
                FitnessInfo bestFitness = bestGenome.FitnessInfo;

                if(bestGenomeSpeciesIdx == -1)
                {
                    // Note. Although the species genomes are sorted by fitness (fittest first), the population best genome may not be at index 0
                    // if multiple genomes have he best fitness score.
                    // TODO: We can avoid this if we find the best genome by sorting the species first, and then selecting the best genome from index 0 of each species.
                    for(int j=0; j < species.GenomeList.Count && fitnessComparer.Compare(species.GenomeList[j].FitnessInfo, bestFitness) >= 0; j++)
                    {
                        if(species.GenomeList[j] == bestGenome) {
                            bestGenomeSpeciesIdx = i;
                        }
                    }
                }
            }

            // Store best genome species index.
            NeatPopulationStatistics neatPopStats = this.NeatPopulationStats;
            neatPopStats.SumSpeciesMeanFitness = sumMeanFitness;

            if(bestGenomeSpeciesIdx != -1) {
                neatPopStats.BestGenomeSpeciesIdx = bestGenomeSpeciesIdx;
            } else {
                throw new InvalidOperationException("Invalid state; none of the species contain the best genome.");
            }
        }

        #endregion
    }
}
