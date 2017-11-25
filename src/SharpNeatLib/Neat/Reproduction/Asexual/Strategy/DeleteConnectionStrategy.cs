using System;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Utils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public class DeleteConnectionStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _generationSeq;
        readonly IRandomSource _rng;

        #region Constructor

        public DeleteConnectionStrategy(
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
            if(parent.ConnectionGenes.Length < 2) {
                return null;
            }

            // Select a gene at random to delete.
            var parentConnArr = parent.ConnectionGenes._connArr;
            var parentWeightArr = parent.ConnectionGenes._weightArr;
            var parentIdArr = parent.ConnectionGenes._idArr;
            int parentLen = parentConnArr.Length;
            int deleteIdx = _rng.Next(parentLen);

            // Create the child genome's ConnectionGenes object.
            int childLen = parentLen - 1;
            var connGenes = new ConnectionGenes<T>(childLen);
            var connArr = connGenes._connArr;
            var weightArr = connGenes._weightArr;
            var idArr = connGenes._idArr;

            // Copy genes up to deleteIdx.
            Array.Copy(parentConnArr, connArr, deleteIdx);
            Array.Copy(parentWeightArr, weightArr, deleteIdx);
            Array.Copy(parentIdArr, idArr, deleteIdx);

            // Copy remaining genes (if any).
            Array.Copy(parentConnArr, deleteIdx+1, connArr, deleteIdx, childLen-deleteIdx);
            Array.Copy(parentWeightArr, deleteIdx+1, weightArr, deleteIdx, childLen-deleteIdx);
            Array.Copy(parentIdArr, deleteIdx+1, idArr, deleteIdx, childLen-deleteIdx);

            // Create an array of indexes into the connection genes that gives the genes in order of innovation ID.
            // Note. We can construct a NeatGenome without passing connIdxArr and it will re-calc it; however this 
            // way is more efficient.
            var connIdxArr = CreateConnectionIndexArray(parent, deleteIdx);

            // Create and return a new genome.
            return new NeatGenome<T>(
                _metaNeatGenome,
                _genomeIdSeq.Next(), 
                _generationSeq.Peek,
                connGenes,
                connIdxArr);
        }

        #endregion

        #region Private Static Methods

        private static int[] CreateConnectionIndexArray(NeatGenome<T> parent, int deleteIdx)
        {
            int parentLen = parent.ConnectionGenes.Length;
            int childLen = parentLen - 1;

            var connIdxArr = new int[childLen];

            // Lookup the deletion index in parent.ConnectionIndexArray.
            int connectionId = parent.ConnectionGenes._idArr[deleteIdx];
            int deleteIdxB = ConnectionGenesUtils.BinarySearchId(
                parent.ConnectionIndexArray,
                parent.ConnectionGenes._idArr,
                connectionId);

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
