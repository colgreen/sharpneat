using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Sexual.Strategy;
using SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;
using SharpNeat.Utils;

namespace SharpNeat.Neat.Reproduction.Sexual
{
    /// <summary>
    /// Creation of offspring given two parents (sexual reproduction).
    /// </summary>
    public class NeatReproductionSexual<T> where T : struct
    {
        readonly NeatReproductionSexualSettings _settings;
        readonly IRandomSource _rng;
        readonly ISexualReproductionStrategy<T> _strategy;

        #region Constructor

        public NeatReproductionSexual(
            MetaNeatGenome<T> metaNeatGenome,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq,
            Int32Sequence generationSeq,
            AddedNodeBuffer addedNodeBuffer,
            NeatReproductionSexualSettings settings)
        {
            _settings = settings;
            _rng = RandomSourceFactory.Create();

            _strategy = new UniformCrossoverReproductionStrategy<T>(
                                metaNeatGenome, genomeIdSeq, generationSeq);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sexual reproduction.
        /// </summary>
        /// <param name="parent1">Parent genome 1.</param>
        /// <param name="parent2">Parent genome 2.</param>
        
        public NeatGenome<T> CreateGenome(
            NeatGenome<T> parent1,
            NeatGenome<T> parent2)
        {
            // Invoke the reproduction strategy.
            return _strategy.CreateGenome(parent1, parent2);            
        }

        #endregion
    }
}
