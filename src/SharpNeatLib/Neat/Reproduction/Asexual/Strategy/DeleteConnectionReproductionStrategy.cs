using Redzen.Random;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public class DeleteConnectionReproductionStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        NeatPopulation<T> _pop;
        IRandomSource _rng;

        #region Constructor

        public DeleteConnectionReproductionStrategy(NeatPopulation<T> pop)
        {
            _pop = pop;
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
                _pop.MetaNeatGenome,
                _pop.GenomeIdSeq.Next(), 
                _pop.CurrentGenerationAge,
                connArr);
        }

        #endregion
    }
}
