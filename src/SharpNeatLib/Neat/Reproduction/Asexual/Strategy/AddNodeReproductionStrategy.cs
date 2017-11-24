using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Utils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    // TODO: Implement!


    public class AddNodeReproductionStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        #region Instance Fields

        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _innovationIdSeq;
        readonly Int32Sequence _generationSeq;
        readonly AddedNodeBuffer _addedNodeBuffer;
        readonly IRandomSource _rng;

        #endregion

        #region Constructor

        public AddNodeReproductionStrategy(
            MetaNeatGenome<T> metaNeatGenome,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq,
            Int32Sequence generationSeq,
            AddedNodeBuffer addedNodeBuffer)
        {
            _metaNeatGenome = metaNeatGenome;
            _genomeIdSeq = genomeIdSeq;
            _innovationIdSeq = innovationIdSeq;
            _generationSeq = generationSeq;
            _addedNodeBuffer = addedNodeBuffer;
            _rng = RandomSourceFactory.Create();
        }

        #endregion

        #region Public Methods

        public NeatGenome<T> CreateChildGenome(NeatGenome<T> parent)
        {
            if(0 == parent.ConnectionGenes.Length) 
            {   // No connections to split.
                return null;
            }

            // Select a connection at random.
            int connectionToSplitIdx = _rng.Next(parent.ConnectionGenes.Length);
            //var connGene = parent.ConnectionGeneArray[connectionToSplitIdx];

            //// The selected connection will be replaced with a new node and two new connections; 
            //// get innovation IDs for these.
            //AddedNodeInfo addedNodeInfo = GetInnovationIDs(connGene.Id);













            // TODO: implement!
            return null;
        }

        #endregion


        

        #region Private Methods

        private AddedNodeInfo GetInnovationIDs(int connectionToSplitId)
        {
            // Test if the selected connection has a previous split recorded in the innovation ID buffer.
            if(_addedNodeBuffer.TryLookup(connectionToSplitId, out AddedNodeInfo addedNodeInfo))
            {
                // Found existing matching structure.
                // However we can only re-use the IDs from that structure if they aren't already present in the current genome;
                // this is possible because genes can be acquired from other genomes via sexual reproduction.
                // Therefore we only re-use IDs if we can re-use all three together, otherwise we aren't assigning the IDs to matching
                // structures throughout the population, which is the reason for ID re-use.


            }
            return new AddedNodeInfo();

        }

        #endregion
    }
}
