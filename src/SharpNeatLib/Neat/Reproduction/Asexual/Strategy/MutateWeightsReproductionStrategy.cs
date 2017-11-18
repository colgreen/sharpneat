using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;
using SharpNeat.Utils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public class MutateWeightsReproductionStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _generationSeq;
        readonly WeightMutationScheme<T> _weightMutationScheme;

        #region Constructor

        public MutateWeightsReproductionStrategy(
            MetaNeatGenome<T> metaNeatGenome,
            Int32Sequence genomeIdSeq,
            Int32Sequence generationSeq,
            WeightMutationScheme<T> weightMutationScheme)
        {
            _metaNeatGenome = metaNeatGenome;
            _genomeIdSeq = genomeIdSeq;
            _generationSeq = generationSeq;
            _weightMutationScheme = weightMutationScheme;
        }

        #endregion

        #region Public Methods

        public NeatGenome<T> CreateChildGenome(NeatGenome<T> parent)
        {
            // Clone the parent's connection genes.
            var connArr = ConnectionGene<T>.CloneArray(parent.ConnectionGeneArray);

            // Apply mutation to the connection genes.
            _weightMutationScheme.MutateWeights(connArr);

            // Create and return a new genome.
            // Note. The parent's ConnectionIndexArray can be re-used here because the new genome has the same set of connections 
            // (same neural net structure, it just has different weights).
            return new NeatGenome<T>(
                _metaNeatGenome,
                _genomeIdSeq.Next(), 
                _generationSeq.Peek,
                connArr,
                parent.ConnectionIndexArray);
        }

        #endregion
    }
}
