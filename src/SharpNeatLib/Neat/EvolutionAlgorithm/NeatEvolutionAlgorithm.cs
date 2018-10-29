using System;
using System.Collections.Generic;
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
                  RandomDefaults.DefaultRandomSourceBuilder)
        {}

        public NeatEvolutionAlgorithm(
            NeatEvolutionAlgorithmSettings eaSettings,
            IGenomeListEvaluator<NeatGenome<T>> evaluator,
            ISpeciationStrategy<NeatGenome<T>,T> speciationStrategy,
            NeatPopulation<T> population,
            NeatReproductionAsexualSettings reproductionAsexualSettings,
            NeatReproductionSexualSettings reproductionSexualSettings,
            WeightMutationScheme<T> weightMutationScheme,
            IRandomSourceBuilder rngBuilder)
        {
            _eaSettings = eaSettings ?? throw new ArgumentNullException(nameof(eaSettings));
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            _speciationStrategy = speciationStrategy ?? throw new ArgumentNullException(nameof(speciationStrategy));
            _pop = population ?? throw new ArgumentNullException(nameof(population));

            if(reproductionAsexualSettings == null) throw new ArgumentNullException(nameof(reproductionAsexualSettings));
            if(reproductionSexualSettings == null) throw new ArgumentNullException(nameof(reproductionSexualSettings));
            
            if(rngBuilder == null) throw new ArgumentNullException(nameof(rngBuilder));
             _rng = rngBuilder.Create();

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
            // TODO: Implement! Rough plan below...


            // Create offspring.
            List<NeatGenome<T>> offspringList = _offspringBuilder.CreateOffspring(_pop.SpeciesArray, _rng);


            // Trim population back to elite genomes only.


            // Merge offspring into genomeList (and species?)


            // Evaluate all genomes in genomeList.


            // Update population and per-species stats








            //// Evaluate each genome in the population; assigning fitness info to each (a single fitness score,
            //// or perhaps a series of scores each measuring a different aspect of fitness).
            //_evaluator.Evaluate(_pop.GenomeList);

            //// Invoke the reproduction strategy (select, cull, create offspring).
            //_selectionReproStrategy.Invoke(_pop);

            //// Update stats.
            //UpdateBestGenome();

            ////_eaStats.StopConditionSatisfied = _evaluator.StopConditionSatisfied;
            //_eaStats.Generation++;
        }

        #endregion

        #region Private Methods

        
        #endregion
    }
}
