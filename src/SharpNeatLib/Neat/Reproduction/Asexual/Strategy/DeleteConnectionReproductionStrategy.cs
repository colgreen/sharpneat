using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Utils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public class DeleteConnectionReproductionStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _generationSeq;
        readonly IRandomSource _rng;

        #region Constructor

        public DeleteConnectionReproductionStrategy(
            MetaNeatGenome<T> metaNeatGenome,
            Int32Sequence genomeIdSeq,
            Int32Sequence generationSeq)
        {
            _metaNeatGenome = metaNeatGenome;
            _genomeIdSeq = genomeIdSeq;
            _generationSeq = generationSeq;
            _rng = RandomSourceFactory.Create();
        }

        #endregion

        #region Public Methods

        public NeatGenome<T> CreateChildGenome(NeatGenome<T> parent)
        {
            // We require at least two connections in the parent, i.e. we avoid creating genomes with
            // no connections, which would be pointless.
            if(parent.ConnectionGeneArray.Length < 2) {
                return null;
            }

            // Select a gene at random to delete.
            var parentConnArr = parent.ConnectionGeneArray;
            int parentLen = parentConnArr.Length;
            int deleteIdx = _rng.Next(parentLen);

            // Alloc child gene array.
            int childLen = parentLen - 1;
            var connArr = new ConnectionGene<T>[childLen];

            // Copy genes up to deleteIdx.
            for(int i=0; i < deleteIdx; i++) {
                connArr[i] = new ConnectionGene<T>(parentConnArr[i]);
            }            

            // Copy remaining genes (if any).
            for(int i = deleteIdx; i < childLen; i++) {
                connArr[i] = new ConnectionGene<T>(parentConnArr[i+1]);
            }

            // Create and return a new genome.
            return new NeatGenome<T>(
                _metaNeatGenome,
                _genomeIdSeq.Next(), 
                _generationSeq.Peek,
                connArr);
        }

        #endregion
    }
}
