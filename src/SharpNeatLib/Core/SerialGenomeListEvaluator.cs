using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Core
{
    /// <summary>
    /// An implementation of IGenomeListEvaluator that evaluates genomes in series on a single CPU thread,
    /// this can be useful in various scenarios e.g. when debugging code.
    /// 
    /// Genome decoding is performed by a provided IGenomeDecoder.
    /// Phenome evaluation is performed by a provided IPhenomeEvaluator.
    /// </summary>
    /// <typeparam name="TGenome">The genome type that is decoded.</typeparam>
    /// <typeparam name="TPhenome">The phenome type that is decoded to and then evaluated.</typeparam>
    public class SerialGenomeListEvaluator<TGenome,TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, IGenome
        where TPhenome: class
    {
        #region Instance Fields

        readonly IGenomeDecoder<TGenome,TPhenome> _genomeDecoder;
        readonly IPhenomeEvaluator<TPhenome> _phenomeEvaluator;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided IGenomeDecoder and IPhenomeEvaluator.
        /// Phenome caching is enabled by default.
        /// </summary>
        public SerialGenomeListEvaluator(IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
                                         IPhenomeEvaluator<TPhenome> phenomeEvaluator)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
        }

        #endregion

        #region IGenomeListEvaluator<TGenome> Members

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _phenomeEvaluator.StopConditionSatisfied; }
        }

        /// <summary>
        /// Evaluates a list of genomes. Here we decode each genome in series using the contained
        /// IGenomeDecoder and evaluate the resulting TPhenome using the contained IPhenomeEvaluator.
        /// </summary>
        public void Evaluate(IList<TGenome> genomeList)
        {
            // Decode and evaluate each genome in turn.
            foreach(TGenome genome in genomeList)
            {
                // TODO: Implement phenome caching (to avoid cost of decode when re-evaluating with a non-deterministic evaluation scheme).
                TPhenome phenome = _genomeDecoder.Decode(genome);
                if(null == phenome)
                {   // Non-viable genome.
                    genome.Fitness = 0.0;
                }
                else 
                {   
                    genome.Fitness = _phenomeEvaluator.Evaluate(phenome);
                }
            }
        }

        #endregion
    }
}
