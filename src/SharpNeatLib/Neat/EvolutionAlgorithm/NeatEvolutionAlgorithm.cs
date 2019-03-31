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
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;
using SharpNeat.Neat.Reproduction.Sexual;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Neat.EvolutionAlgorithm
{
    /// <summary>
    /// The NEAT  evolution algorithm.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public class NeatEvolutionAlgorithm<T> : IEvolutionAlgorithm 
        where T : struct
    {
        #region Instance Fields

        readonly NeatEvolutionAlgorithmSettings _eaSettings;
        readonly IGenomeListEvaluator<NeatGenome<T>> _evaluator;
        readonly ISpeciationStrategy<NeatGenome<T>,T> _speciationStrategy;
        readonly NeatPopulation<T> _pop;
        readonly IRandomSource _rng;

        readonly Int32Sequence _generationSeq;
        readonly NeatReproductionAsexual<T> _reproductionAsexual;
        readonly NeatReproductionSexual<T> _reproductionSexual;

        readonly OffspringBuilder<T> _offspringBuilder;

        EvolutionAlgorithmStatistics _eaStats = new EvolutionAlgorithmStatistics();
        ComplexityRegulationMode _complexityRegulationMode = ComplexityRegulationMode.Complexifying;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="eaSettings">NEAT evolution algorithm settings.</param>
        /// <param name="evaluator">An evaluator of lists of genomes.</param>
        /// <param name="speciationStrategy">Speciation strategy.</param>
        /// <param name="population">An initial population of genomes.</param>
        /// <param name="reproductionAsexualSettings">Asexual reproduction settings.</param>
        /// <param name="reproductionSexualSettings">Sexual reproduction settings.</param>
        /// <param name="weightMutationScheme">Connection weight mutation scheme.</param>
        public NeatEvolutionAlgorithm(
            NeatEvolutionAlgorithmSettings eaSettings,
            IGenomeListEvaluator<NeatGenome<T>> evaluator,
            ISpeciationStrategy<NeatGenome<T>,T> speciationStrategy,
            NeatPopulation<T> population,
            NeatReproductionAsexualSettings reproductionAsexualSettings,
            NeatReproductionSexualSettings reproductionSexualSettings,
            WeightMutationScheme<T> weightMutationScheme)
            : this(
                eaSettings,
                evaluator,
                speciationStrategy,
                population,
                reproductionAsexualSettings,
                reproductionSexualSettings,
                weightMutationScheme,
                RandomDefaults.CreateRandomSource())
        {}

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="eaSettings">NEAT evolution algorithm settings.</param>
        /// <param name="evaluator">An evaluator of lists of genomes.</param>
        /// <param name="speciationStrategy">Speciation strategy.</param>
        /// <param name="population">An initial population of genomes.</param>
        /// <param name="reproductionAsexualSettings">Asexual reproduction settings.</param>
        /// <param name="reproductionSexualSettings">Sexual reproduction settings.</param>
        /// <param name="weightMutationScheme">Connection weight mutation scheme.</param>
        /// <param name="rng">Random source.</param>
        public NeatEvolutionAlgorithm(
            NeatEvolutionAlgorithmSettings eaSettings,
            IGenomeListEvaluator<NeatGenome<T>> evaluator,
            ISpeciationStrategy<NeatGenome<T>,T> speciationStrategy,
            NeatPopulation<T> population,
            NeatReproductionAsexualSettings reproductionAsexualSettings,
            NeatReproductionSexualSettings reproductionSexualSettings,
            WeightMutationScheme<T> weightMutationScheme,
            IRandomSource rng)
        {
            _eaSettings = eaSettings ?? throw new ArgumentNullException(nameof(eaSettings));
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            _speciationStrategy = speciationStrategy ?? throw new ArgumentNullException(nameof(speciationStrategy));
            _pop = population ?? throw new ArgumentNullException(nameof(population));

            if(reproductionAsexualSettings == null) throw new ArgumentNullException(nameof(reproductionAsexualSettings));
            if(reproductionSexualSettings == null) throw new ArgumentNullException(nameof(reproductionSexualSettings));

             _rng = rng;

            if(eaSettings.SpeciesCount > population.PopulationSize) {
                throw new ArgumentException("Species count is higher then the population size.");
            }

            _generationSeq = new Int32Sequence();

            _reproductionAsexual = new NeatReproductionAsexual<T>(
                _pop.MetaNeatGenome, _pop.GenomeBuilder,
                _pop.GenomeIdSeq, population.InnovationIdSeq, _generationSeq,
                _pop.AddedNodeBuffer, reproductionAsexualSettings, weightMutationScheme);

            _reproductionSexual = new NeatReproductionSexual<T>(
                _pop.MetaNeatGenome, _pop.GenomeBuilder,
                _pop.GenomeIdSeq, population.InnovationIdSeq, _generationSeq,
                _pop.AddedNodeBuffer, reproductionSexualSettings);

            _offspringBuilder = new OffspringBuilder<T>(_reproductionAsexual, _reproductionSexual, _eaSettings.InterspeciesMatingProportion);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NeatPopulation{T}"/>.
        /// </summary>
        public NeatPopulation<T> Population => _pop;

        /// <summary>
        /// Gets the current evolution algorithm statistics.
        /// </summary>
        public EvolutionAlgorithmStatistics Stats => _eaStats;

        /// <summary>
        /// Gets or sets the complexity regulation mode.
        /// </summary>
        public ComplexityRegulationMode ComplexityRegulationMode 
        { 
            get => _complexityRegulationMode;
            set => _complexityRegulationMode = value; 
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise the evolutionary algorithm.
        /// </summary>
        public void Initialise()
        {
            // Evaluate each genome in the new population.
            Evaluate(_pop.GenomeList);

            // Initialise species.
            _pop.InitialiseSpecies(_speciationStrategy, _eaSettings.SpeciesCount, _rng);

            // Update population and evolution algorithm statistics.
            UpdateStats();
        }

        /// <summary>
        /// Perform one generation of the evolution algorithm.
        /// </summary>
        public void PerformOneGeneration()
        {
            // Create offspring.
            List<NeatGenome<T>> offspringList = _offspringBuilder.CreateOffspring(_pop.SpeciesArray, _rng);

            // Trim population back to elite genomes only.
            // Get back a flag that indicates if one or more species are now completely empty (i.e. species.genomeList.Count == 0).
            bool emptySpeciesFlag = TrimSpeciesBackToElite();

            // Rebuild _pop.GenomeList. It will now contain just the elite genomes from each species.
            RebuildGenomeList();

            // Append offspring genomes to the elite genomes in _pop.GenomeList. We do this before calling the
            // _genomeListEvaluator.Evaluate() because some evaluation schemes re-evaluate the elite genomes 
            // (otherwise we could just evaluate offspringList).
            _pop.GenomeList.AddRange(offspringList);

            // TODO: Review this. We don't necessarily want to re-evaluate genomes even if the evaluation scheme is non-deterministic.
            // Evaluate the genomes in the population.
            // If the evaluation scheme is deterministic then only the new genomes (the offspring) need to be evaluated;
            // otherwise all of the genomes are evaluated, thus the elite genomes are re-evaluated at each generation.
            if(_evaluator.IsDeterministic) {
                Evaluate(offspringList);
            }
            else {
                Evaluate(_pop.GenomeList);
            }

            // Integrate offspring into the species.
            IntegrateOffspringIntoSpecies(offspringList, emptySpeciesFlag);

            // Update statistics.
            UpdateStats();

            // TODO: Complexity regulation logic.
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Trims the genomeList in each specie back to the number of elite genomes defined for each species.
        /// Returns true if there are empty species following trimming.
        /// </summary>
        /// <remarks>
        /// The genomes in species.GenomeList are ordered best to worst, thus the genomes must be removed from the end of the list.
        /// </remarks>
        private bool TrimSpeciesBackToElite()
        {
            bool emptySpeciesFlag = false;
            int speciesCount = _pop.SpeciesArray.Length;

            for(int i=0; i < speciesCount; i++)
            {
                Species<T> species = _pop.SpeciesArray[i];
                int eliteSizeInt = species.Stats.EliteSizeInt;
                int removeCount = species.GenomeList.Count - eliteSizeInt;
                species.GenomeList.RemoveRange(eliteSizeInt, removeCount);

                if(0 == eliteSizeInt) {
                    emptySpeciesFlag = true;
                }
            }

            return emptySpeciesFlag;
        }

        /// <summary>
        /// Rebuild _genomeList from genomes held within the species.
        /// </summary>
        private void RebuildGenomeList()
        {
            var genomeList = _pop.GenomeList;

            genomeList.Clear();
            foreach(var species in _pop.SpeciesArray) {
                genomeList.AddRange(species.GenomeList);
            }
        }

        private void IntegrateOffspringIntoSpecies(List<NeatGenome<T>> offspringList, bool emptySpeciesFlag)
        {
            // Handle special case of one or more species having a zero target size (dead species).
            if(emptySpeciesFlag)
            {   
                // There are one or more terminated species. Therefore we need to fully re-speciate all genomes 
                // to evenly divide them between the required number of species.

                // Clear all genomes from species (note. we still have all genomes in _pop.GenomeList).
                ClearAllSpecies();

                // Re-initialise the species.
                _pop.InitialiseSpecies(_speciationStrategy, _eaSettings.SpeciesCount, _rng);    
            }
            else
            {
                // Integrate offspring into the existing species. 
                _speciationStrategy.SpeciateAdd(offspringList, _pop.SpeciesArray, _rng);

                // Sort the genomes in each species. Highest fitness first, then secondary sorted by youngest genomes first.
                foreach(var species in _pop.SpeciesArray) {
                    SortUtils.SortUnstable(species.GenomeList, GenomeFitnessAndAgeComparer<T>.Singleton, _rng);
                }
            }

            Debug.Assert(!TestForEmptySpecies(), "Speciation resulted in one or more empty species.");
        }

        private void UpdateStats()
        {
            // Update population statistics.
            _pop.UpdateStats(_evaluator.FitnessComparer);

            // Store the current generation number, and increment.
            _eaStats.Generation = _generationSeq.Peek;
            _generationSeq.Next();

            // Test if the evaluator is signalling that the best fitness is good enough to stop the evolution algorithm.
            _eaStats.StopConditionSatisfied = _evaluator.TestForStopCondition(_pop.Stats.BestFitness);

            // Update total number of evaluations performed.
            // TODO: Get number of genome evaluations that have been performed in the current generation.
            ulong evaluationCountDelta = 0;
            _eaStats.TotalEvaluationCount += evaluationCountDelta;

            // Update species allocation sizes.
            SpeciesAllocationCalcs<T>.UpdateSpeciesAllocationSizes(_pop, _eaSettings, _rng);
        }

        #endregion

        #region Private Methods [Low Level]

        /// <summary>
        /// Clear the genome list of all species.
        /// </summary>
        private void ClearAllSpecies()
        {
            foreach(var species in _pop.SpeciesArray) {
                species.GenomeList.Clear();
            }
        }

        /// <summary>
        /// Returns true if there is one or more empty species.
        /// </summary>
        private bool TestForEmptySpecies()
        {
            return _pop.SpeciesArray.Any(x => (x.GenomeList.Count == 0));
        }

        private void Evaluate(ICollection<NeatGenome<T>> genomeList)
        {
            _evaluator.Evaluate(genomeList);
            _eaStats.TotalEvaluationCount += (ulong)genomeList.Count;
        }

        #endregion
    }
}
