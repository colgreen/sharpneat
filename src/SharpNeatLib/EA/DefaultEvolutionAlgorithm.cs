using SharpNeat.Evaluation;

namespace SharpNeat.EA
{
    public class DefaultEvolutionAlgorithm<TGenome> : IEvolutionAlgorithm 
        where TGenome : IGenome
    {
        #region Instance Fields

        readonly EAParameters _eaParams;
        readonly IGenomeListEvaluator<TGenome> _evaluator;
        readonly ISelectionReproductionStrategy<TGenome> _selectionReproStrategy;
        readonly Population<TGenome> _pop;

        EAStatistics _eaStats = new EAStatistics();
        ComplexityRegulationMode _complexityRegulationMode = ComplexityRegulationMode.Complexifying;
        TGenome _currentBestGenome;

        #endregion

        #region Constructors

        public DefaultEvolutionAlgorithm(
            EAParameters eaParams,
            IGenomeListEvaluator<TGenome> evaluator,
            ISelectionReproductionStrategy<TGenome> selectionReproStrategy,
            Population<TGenome> population)
        {
            _eaParams = eaParams;
            _evaluator = evaluator;
            _selectionReproStrategy = selectionReproStrategy;
            _pop = population;
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

        public void PerformOneGeneration()
        {
            // Evaluate each genome in the population; assigning fitness info to each (a single fitness score,
            // or perhaps a series of scores each measuring a different aspect of fitness).
            _evaluator.Evaluate(_pop.GenomeList);

            // Invoke the reproduction strategy (select, cull, create offspring).
            _selectionReproStrategy.Invoke(_pop);

            // Update stats.
            UpdateBestGenome();
            //_eaStats.StopConditionSatisfied = _evaluator.StopConditionSatisfied;
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
