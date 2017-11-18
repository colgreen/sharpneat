using System;
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
            Array.Copy(parentConnArr, connArr, deleteIdx);

            // Copy remaining genes (if any).
            Array.Copy(parentConnArr, deleteIdx+1, connArr, deleteIdx, childLen-deleteIdx);

            // Create an array of indexes into the connection genes that gives the genes in order of innovation ID.
            // Note. We can construct a NeatGenome without passing connIdxArr and it will re-calc it; however this 
            // way is more efficient.
            var connIdxArr = CreateConnectionIndexArray(parent, deleteIdx);

            // Create and return a new genome.
            return new NeatGenome<T>(
                _metaNeatGenome,
                _genomeIdSeq.Next(), 
                _generationSeq.Peek,
                connArr,
                connIdxArr);
        }

        #endregion

        #region Private Static Methods

        private static int[] CreateConnectionIndexArray<T>(NeatGenome<T> parent, int deleteIdx)
            where T : struct
        {
            int parentLen = parent.ConnectionGeneArray.Length;
            int childLen = parentLen - 1;

            var connIdxArr = new int[childLen];

            // Lookup the deletion index in parent.ConnectionIndexArray.
            int connectionId = parent.ConnectionGeneArray[deleteIdx].Id;
            int deleteIdxB = ConnectionGeneUtils.BinarySearchId(parent.ConnectionIndexArray, parent.ConnectionGeneArray, connectionId);

            // Copy indexes from the parent to the child array, skipping the element to be deleted.
            Array.Copy(parent.ConnectionIndexArray, connIdxArr, deleteIdxB);
            Array.Copy(parent.ConnectionIndexArray, deleteIdxB+1, connIdxArr, deleteIdxB, childLen - deleteIdxB);

            // All connections after the deleted connection will have been shifted by one element left, therefore
            // indexes to these genes will need decrementing by one.
            for(int i=0; i < connIdxArr.Length; i++) {
                if(connIdxArr[i] >= deleteIdx) { connIdxArr[i]--; }
            }

            return connIdxArr;
        }

        #endregion
    }
}
