using Redzen.Random;
using Redzen.Structures;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Sexual.Strategy;
using SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;

namespace SharpNeat.Neat.Reproduction.Sexual
{
    /// <summary>
    /// Creation of offspring given two parents (sexual reproduction).
    /// </summary>
    public class NeatReproductionSexual<T> where T : struct
    {
        readonly NeatReproductionSexualSettings _settings;
        readonly ISexualReproductionStrategy<T> _strategy;

        #region Constructor

        public NeatReproductionSexual(
            MetaNeatGenome<T> metaNeatGenome,
            INeatGenomeBuilder<T> genomeBuilder,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq,
            Int32Sequence generationSeq,
            AddedNodeBuffer addedNodeBuffer,
            NeatReproductionSexualSettings settings,
            IRandomSourceBuilder rngBuilder)
        {
            _settings = settings;
            _strategy = new UniformCrossoverReproductionStrategy<T>(
                                metaNeatGenome, genomeBuilder,
                                genomeIdSeq, generationSeq, 
                                rngBuilder.Create());
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
