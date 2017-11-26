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
    public class AddNodeStrategy<T> : IAsexualReproductionStrategy<T>
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

        public AddNodeStrategy(
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
            {   // No connections to split (nodes are added by splitting an existing connection).
                return null;
            }

            // Select a connection at random.
            int splitConnIdx = _rng.Next(parent.ConnectionGenes.Length);
            int splitConnId = parent.ConnectionGenes._idArr[splitConnIdx];
            

            // The selected connection will be replaced with a new node and two new connections; 
            // get innovation IDs for these.
            AddedNodeInfo addedNodeInfo = GetInnovationIDs(splitConnId, parent);




            // TODO: implement!
            return null;
        }

        #endregion

        #region Private Methods

        private AddedNodeInfo GetInnovationIDs(int splitConnId, NeatGenome<T> parent)
        {
            // Test if the selected connection has a previous split recorded in the innovation ID buffer.
            AddedNodeInfo addedNodeInfo;
            if(_addedNodeBuffer.TryLookup(splitConnId, out addedNodeInfo))
            {
                // Found existing matching structure.
                // However we can only re-use the IDs from that structure if they aren't already present in the current genome;
                // this can happen e.g. if a previous split between the two chosen nodes has been had one of its connections deleted
                // and another new connection was made, and we are now splitting that connection. Sexual reproduction could also 
                // result in the same or similar situation.
                // 
                // Therefore we only re-use IDs if we can re-use all three together, otherwise we aren't assigning the IDs to matching
                // structures throughout the population, which is the reason for ID re-use.
                if(    parent.ContainsHiddenNode(addedNodeInfo.AddedNodeId)
                    && parent.ContainsConnection(addedNodeInfo.AddedInputConnectionId)
                    && parent.ContainsConnection(addedNodeInfo.AddedOutputConnectionId))
                {
                    // None of the ID are present on the parent genome, therefore we can re-use them.
                    return addedNodeInfo;
                }

                // We can't re-use the IDs from the buffer, so allocate new IDs.
                // Note. these aren't added to the buffer; instead we leave the existing buffer entry for splitConnId.
                return new AddedNodeInfo(_innovationIdSeq);
            }

            // No buffer entry found, therefore we allocate new IDs.
            addedNodeInfo = new AddedNodeInfo(_innovationIdSeq);

            // Register the new IDs with the buffer.
            _addedNodeBuffer.Register(splitConnId, addedNodeInfo);

            return addedNodeInfo;
        }

        #endregion
    }
}
