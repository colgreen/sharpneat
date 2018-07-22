using System.Collections.Generic;
using SharpNeat.EA;

namespace SharpNeat.Evaluation
{
    /// <summary>
    /// An implementation of <see cref="IGenomeListEvaluator{TGenome}"/> that evaluates genomes in series on a single CPU thread,
    /// this can be useful in various scenarios e.g. when debugging code.
    /// </summary>
    /// <typeparam name="TGenome">The genome type that is decoded.</typeparam>
    /// <typeparam name="TPhenome">The phenome type that is decoded to and then evaluated.</typeparam>
    /// <remarks>
    /// Genome decoding is performed by a provided IGenomeDecoder.
    /// Phenome evaluation is performed by a provided IPhenomeEvaluator.
    /// </remarks>
    public class SerialGenomeListEvaluator<TGenome,TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : IGenome
        where TPhenome : class
    {
        #region Instance Fields

        readonly IGenomeDecoder<TGenome,TPhenome> _genomeDecoder;
        readonly IPhenomeEvaluator<TPhenome> _phenomeEvaluator;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided <see cref="IGenomeDecoder"/> and <see cref="IPhenomeEvaluator"/>.
        /// Phenome caching is enabled by default.
        /// </summary>
        public SerialGenomeListEvaluator(
            IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
            IPhenomeEvaluator<TPhenome> phenomeEvaluator)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
        }

        #endregion

        #region IGenomeListEvaluator Members

        /// <summary>
        /// Evaluates a collection of genomes and assigns fitness info to each.
        /// </summary>
        public void Evaluate(ICollection<TGenome> genomeList)
        {
            // Decode and evaluate each genome in turn.
            foreach(TGenome genome in genomeList)
            {
                // TODO: Implement phenome caching (to avoid decode cost when re-evaluating with a non-deterministic evaluation scheme).
                TPhenome phenome = _genomeDecoder.Decode(genome);
                if(null == phenome)
                {   // Non-viable genome.
                    genome.FitnessInfo = _phenomeEvaluator.NullFitness;
                }
                else 
                {   
                    genome.FitnessInfo = _phenomeEvaluator.Evaluate(phenome);
                }
            }
        }

        /// <summary>
        /// Gets a fitness comparer. 
        /// </summary>
        /// <remarks>
        /// Typically there is a single fitness score whereby a higher score is better, however if there are multiple fitness scores
        /// per genome then we need a more general purpose comparer to determine an ordering on FitnessInfo(s), i.e. to be able to 
        /// determine which is the better FitenssInfo between any two.
        /// </remarks>
        public IComparer<FitnessInfo> FitnessComparer => _phenomeEvaluator.FitnessComparer;

        #endregion
    }
}
