using System;
using Redzen.Random;
using Redzen.Sorting;
using SharpNeat.EA;
using SharpNeat.Evaluation;
using SharpNeat.Neat.Genome;
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

        EAStatistics _eaStats = new EAStatistics();
        ComplexityRegulationMode _complexityRegulationMode = ComplexityRegulationMode.Complexifying;

        #endregion

        #region Constructors

        public NeatEvolutionAlgorithm(
            NeatEvolutionAlgorithmSettings eaSettings,
            IGenomeListEvaluator<NeatGenome<T>> evaluator,
            ISpeciationStrategy<NeatGenome<T>,T> speciationStrategy,
            NeatPopulation<T> population)
            : this(eaSettings, evaluator, speciationStrategy, population, RandomDefaults.CreateRandomSource())
        {}

        public NeatEvolutionAlgorithm(
            NeatEvolutionAlgorithmSettings eaSettings,
            IGenomeListEvaluator<NeatGenome<T>> evaluator,
            ISpeciationStrategy<NeatGenome<T>,T> speciationStrategy,
            NeatPopulation<T> population,
            IRandomSource rng)
        {
            _eaSettings = eaSettings ?? throw new ArgumentNullException(nameof(eaSettings));
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            _speciationStrategy = speciationStrategy ?? throw new ArgumentNullException(nameof(speciationStrategy));
            _pop = population ?? throw new ArgumentNullException(nameof(population));
             _rng = rng ?? throw new ArgumentNullException(nameof(rng));

            if(eaSettings.SpeciesCount > population.PopulationSize) {
                throw new ArgumentException("Species count is higher then the population size.");
            }
        }

        #endregion

        #region Properties

        public EAStatistics EAStats => _eaStats;

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
            InitialiseSpecies();

            // Calculate and store stats on the population as a whole, and for each species.
            PopulationStatsCalcs<T>.CalcAndStorePopulationStats(_pop);
            SpeciesStatsCalcs<T>.CalcAndStoreSpeciesStats(_pop, _eaSettings, _rng);


        }

        public void PerformOneGeneration()
        {




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

        private void InitialiseSpecies()
        {
            // Allocate the genomes to species.
            Species<T>[] speciesArr = _speciationStrategy.SpeciateAll(_pop.GenomeList, _eaSettings.SpeciesCount);
            if(null == speciesArr || speciesArr.Length != _eaSettings.SpeciesCount) {
                throw new Exception("Species array is null or has incorrect length.");
            }
            _pop.SpeciesArray = speciesArr;

            // Sort the genomes in each species. Highest fitness first, then secondary sorted by youngest genomes first.
            foreach(Species<T> species in speciesArr) {
                SortUtils.SortUnstable(species.GenomeList, GenomeFitnessAndAgeComparer<T>.Singleton, _rng);
            }
        }

        #endregion



    }
}
