using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;

namespace SharpNeat.EA
{
    public class DefaultEvolutionAlgorithm<TGenome> : IEvolutionAlgorithm where TGenome : IGenome
    {
        #region Instance Fields

        EAParameters _eaParams;
        IGenomeListEvaluator<TGenome> _evaluator;
        IDifferentialReproductionStrategy<TGenome> _diffReproductionStrategy;
        Population<TGenome> _population;
        EAStatistics _eaStats = new EAStatistics();

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

        #region IEvolutionaryAlgorithm

        public EAStatistics EAStats { get { return _eaStats; } }

        public void PerformOneGeneration()
        {
            // Evaluate each genome in the population; assigning fitness info to each (a single fitness score,
            // or perhaps a series of scores each measuring a different aspect of fitness).
            _evaluator.Evaluate(_population.GenomeList);

            // Invoke the reproduction strategy (select, cull, create offspring).
            _diffReproductionStrategy.Invoke(_population);
        }

        #endregion
    }
}
