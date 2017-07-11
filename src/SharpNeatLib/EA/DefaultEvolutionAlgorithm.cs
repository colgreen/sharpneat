using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;

namespace SharpNeat.EA
{
    public class DefaultEvolutionAlgorithm<TGenome> : IEvolutionAlgorithm where TGenome : class, IGenome
    {
        #region Instance Fields

        EAParameters _eaParams;
        IGenomeListEvaluator<TGenome> _evaluator;
        IDifferentialReproductionStrategy<TGenome> _diffReproductionStrategy;
        Population<TGenome> _population;
        EAStatistics _eaStats = new EAStatistics();
        ComplexityRegulationMode _complexityRegulationMode = ComplexityRegulationMode.Complexifying;
        TGenome _currentBestGenome;

        #endregion

        #region Constructors

        public DefaultEvolutionAlgorithm(
            EAParameters eaParams,
            IGenomeListEvaluator<TGenome> evaluator,
            IDifferentialReproductionStrategy<TGenome> diffReproductionStrategy,
            Population<TGenome> population)
        {
            _eaParams = eaParams;
            _evaluator = evaluator;
            _diffReproductionStrategy = diffReproductionStrategy;
            _population = population;
        }

        #endregion

        #region Properties

        public EAStatistics EAStats { get { return _eaStats; } }

        /// <summary>
        /// Gets or sets the complexity regulation mode.
        /// </summary>
        public ComplexityRegulationMode ComplexityRegulationMode 
        { 
            get { return _complexityRegulationMode; }
            set { _complexityRegulationMode = value; } 
        }

        #endregion

        #region Public Methods

        public void PerformOneGeneration()
        {
            // Evaluate each genome in the population; assigning fitness info to each (a single fitness score,
            // or perhaps a series of scores each measuring a different aspect of fitness).
            _evaluator.Evaluate(_population.GenomeList);

            // Invoke the reproduction strategy (select, cull, create offspring).
            _diffReproductionStrategy.Invoke(_population);

            // Update stats.
            UpdateBestGenome();
            _eaStats.StopConditionSatisfied = _evaluator.StopConditionSatisfied;
        }

        #endregion

        #region Private Methods

        private void UpdateBestGenome()
        {
            // If all genomes have the same fitness (including zero) then we simply return the first genome.
            TGenome bestGenome = _population.GenomeList[0];
            double bestFitness = _population.GenomeList[0].Fitness;

            foreach(TGenome genome in _population.GenomeList)
            {
                if(genome.Fitness > bestFitness)
                {
                    bestGenome = genome;
                    bestFitness = genome.Fitness;
                }
            }
            _currentBestGenome = bestGenome;
        }

        #endregion
    }
}
