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
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public class NeatReproductionSexual<T> : ISexualReproductionStrategy<T>
        where T : struct
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
            NeatReproductionSexualSettings settings)
        {
            _settings = settings;
            _strategy = new UniformCrossoverReproductionStrategy<T>(
                                metaNeatGenome.IsAcyclic,
                                settings.SecondaryParentGeneProbability,
                                genomeBuilder,
                                genomeIdSeq, generationSeq);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new child genome based on the genetic content of two parent genome.
        /// </summary>
        /// <param name="parent1">Parent 1.</param>
        /// <param name="parent2">Parent 2.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>A new child genome.</returns>
        public NeatGenome<T> CreateGenome(NeatGenome<T> parent1, NeatGenome<T> parent2, IRandomSource rng)
        {
            // Invoke the reproduction strategy.
            return _strategy.CreateGenome(parent1, parent2, rng);
        }

        #endregion
    }
}
