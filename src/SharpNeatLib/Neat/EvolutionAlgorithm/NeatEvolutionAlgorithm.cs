using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Redzen.Numerics;
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

        public NeatEvolutionAlgorithm(
            NeatEvolutionAlgorithmSettings eaSettings,
            IGenomeListEvaluator<NeatGenome<T>> evaluator,
            ISpeciationStrategy<NeatGenome<T>,T> speciationStrategy,
            NeatPopulation<T> population,
            NeatReproductionAsexualSettings reproductionAsexualSettings,
            NeatReproductionSexualSettings reproductionSexualSettings,
            WeightMutationScheme<T> weightMutationScheme)
            : this(eaSettings, evaluator, speciationStrategy, population,
                  reproductionAsexualSettings, reproductionSexualSettings,
                  weightMutationScheme,
                  RandomDefaults.CreateRandomSource())
        {}

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
            _evaluator.Evaluate(_pop.GenomeList);

            // Initialise species.
            _pop.InitialiseSpecies(_speciationStrategy, _eaSettings.SpeciesCount, _rng);

            // Calculate and store stats on the population as a whole, and for each species.
            PopulationStatsCalcs<T>.CalcAndStorePopulationStats(_pop);
            SpeciesStatsCalcs<T>.CalcAndStoreSpeciesStats(_pop, _eaSettings, _rng);
        }

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

            // Evaluate all of the genomes in the population, assigning fitness info to each.
            _evaluator.Evaluate(_pop.GenomeList);

            // Integrate offspring into the species.
            IntegrateOffspringIntoSpecies(offspringList, emptySpeciesFlag);

            // Update population and per-species stats
            PopulationStatsCalcs<T>.CalcAndStorePopulationStats(_pop);
            SpeciesStatsCalcs<T>.CalcAndStoreSpeciesStats(_pop, _eaSettings, _rng);

            // Update the EvolutionAlgorithm stats object.
            _eaStats.BestFitness = _pop.GenomeList[_pop.BestGenomeIdx].FitnessInfo;
            _eaStats.StopConditionSatisfied = _evaluator.TestForStopCondition(_eaStats.BestFitness);
            _eaStats.Generation = _generationSeq.Next();


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
            foreach(var species in _pop.SpeciesArray) {
                if(species.GenomeList.Count == 0) {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
