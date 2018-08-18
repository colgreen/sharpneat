using System;
using SharpNeat.Evaluation;

namespace SharpNeat.EA
{
    public class DefaultEvolutionAlgorithm<TGenome> : IEvolutionAlgorithm 
        where TGenome : IGenome
    {
        #region Instance Fields

        readonly EvolutionAlgorithmSettings _eaSettings;
        readonly IGenomeListEvaluator<TGenome> _evaluator;
        readonly ISelectionReproductionStrategy<TGenome> _selectionReproStrategy;
        readonly Population<TGenome> _pop;

        EAStatistics _eaStats = new EAStatistics();
        ComplexityRegulationMode _complexityRegulationMode = ComplexityRegulationMode.Complexifying;
        TGenome _currentBestGenome;

        #endregion

        #region Constructors

        public DefaultEvolutionAlgorithm(
            EvolutionAlgorithmSettings eaSettings,
            IGenomeListEvaluator<TGenome> evaluator,
            ISelectionReproductionStrategy<TGenome> selectionReproStrategy,
            Population<TGenome> population)
        {
            _eaSettings = eaSettings ?? throw new ArgumentNullException(nameof(eaSettings));
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            _selectionReproStrategy = selectionReproStrategy ?? throw new ArgumentNullException(nameof(selectionReproStrategy));
            _pop = population ?? throw new ArgumentNullException(nameof(population));

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

            // Initialise the selection-reproduction strategy.
            _selectionReproStrategy.Initialise(_pop);
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

        private void UpdateBestGenome()
        {
            // If all genomes have the same fitness (including zero) then we simply return the first genome.
            TGenome bestGenome = _pop.GenomeList[0];
            FitnessInfo bestFitnessInfo = _pop.GenomeList[0].FitnessInfo;
            var comparer = _evaluator.FitnessComparer;

            foreach(TGenome genome in _pop.GenomeList)
            {
                if(comparer.Compare(bestFitnessInfo, genome.FitnessInfo) < 0)
                {
                    bestGenome = genome;
                    bestFitnessInfo = genome.FitnessInfo;
                }
            }
            _currentBestGenome = bestGenome;
        }

        #endregion
    }
}
