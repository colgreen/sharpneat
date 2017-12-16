using System;
using System.Globalization;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Utils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public partial class AddNodeStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        #region Instance Fields

        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _innovationIdSeq;
        readonly Int32Sequence _generationSeq;
        readonly AddedNodeBuffer _addedNodeBuffer;
        readonly AddedConnectionBuffer _addedConnBuffer;
        readonly IRandomSource _rng;

        #endregion

        #region Constructor

        public AddNodeStrategy(
            MetaNeatGenome<T> metaNeatGenome,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq,
            Int32Sequence generationSeq,
            AddedNodeBuffer addedNodeBuffer,
            AddedConnectionBuffer addedConnBuffer)
        {
            _metaNeatGenome = metaNeatGenome;
            _genomeIdSeq = genomeIdSeq;
            _innovationIdSeq = innovationIdSeq;
            _generationSeq = generationSeq;
            _addedNodeBuffer = addedNodeBuffer;
            _addedConnBuffer = addedConnBuffer;
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
            AddedNodeInfo addedNodeInfo = GetInnovationIDs(splitConnId, parent, out bool newInnovationIdsFlag);

            // Create the two new connections.
            var splitConn = parent.ConnectionGenes._connArr[splitConnIdx];
            var newConnArr = new DirectedConnection[] { 
                new DirectedConnection(splitConn.SourceId, addedNodeInfo.AddedNodeId),
                new DirectedConnection(addedNodeInfo.AddedNodeId, splitConn.TargetId)
            };

            // Register new connection innovation IDs.
            if(newInnovationIdsFlag)
            {
                _addedConnBuffer.Register(newConnArr[0], addedNodeInfo.AddedInputConnectionId);
                _addedConnBuffer.Register(newConnArr[1], addedNodeInfo.AddedOutputConnectionId);
            }

            // Get weights for the new connections.
            // Connection 1 gets the weight from the original connection; connection 2 gets a fixed
            // weight of _metaNeatGenome.ConnectionWeightRange.

            // TODO: Consider a better choice of weights for the new connections; this scheme has been
            // copied from sharpneat 2.x as a starting point, but can likely be improved upon.
            var newWeightArr = new T[] { 
                parent.ConnectionGenes._weightArr[splitConnIdx],
                (T)Convert.ChangeType(_metaNeatGenome.ConnectionWeightRange, typeof(T))
            };

            // IDs for the new connections.
            var newIdArr = new int[]
            {
                addedNodeInfo.AddedInputConnectionId,
                addedNodeInfo.AddedOutputConnectionId
            };

            // Ensure newConnArr is sorted.
            // Later on we'll determine their insertion indexes into the connection array, therefore this ensures that 
            // the insert indexes will be sorted correctly.
            if(newConnArr[0].CompareTo(newConnArr[1]) > 0)
            {
                var tmpConn = newConnArr[0];
                newConnArr[0] = newConnArr[1];
                newConnArr[1] = tmpConn;

                T tmpWeight = newWeightArr[0];
                newWeightArr[0] = newWeightArr[1];
                newWeightArr[1] = tmpWeight;

                int tmpid = newIdArr[0];
                newIdArr[0] = newIdArr[1];
                newIdArr[1] = tmpid;
            }

            // Create a new connection gene array that consists of the parent connection genes, 
            // with the connection that was split removed, and the two new connection genes that 
            // replace it inserted at the correct (sorted) positions.
            var parentConnArr = parent.ConnectionGenes._connArr;
            var parentWeightArr = parent.ConnectionGenes._weightArr;
            var parentIdArr = parent.ConnectionGenes._idArr;
            int parentLen = parentConnArr.Length;

            // Create the child genome's ConnectionGenes object.
            int childLen = parentLen + 1;
            var connGenes = new ConnectionGenes<T>(childLen);
            var connArr = connGenes._connArr;
            var weightArr = connGenes._weightArr;
            var idArr = connGenes._idArr;

            // Build an array of parent indexes to stop at when copying from the parent to the child connection array.
            // Note. Each index is combined with a second value; an index into newConnArr for insertions,
            // and -1 for the split index (the connection to be removed)
            int insertIdx1 = ~Array.BinarySearch(parent.ConnectionGenes._connArr, newConnArr[0]);
            int insertIdx2 = ~Array.BinarySearch(parent.ConnectionGenes._connArr, newConnArr[1]);
            (int,int)[] stopIdxArr = new []
            {
                (splitConnIdx, -1),
                (insertIdx1, 0),
                (insertIdx2, 1)
            };

            // Sort by the first index value.
            Array.Sort(stopIdxArr, ((int,int)x, (int,int)y) => x.Item1.CompareTo(y.Item1));

            // Record the insertion indexes into the child connection.
            var childInsertionArr = new (int connIdx, int id)[2];
            int childInsertionIdx = 0;

            // Loop over stopIdxArr.
            int parentIdx = 0;
            int childIdx = 0;

            for(int i=0; i<stopIdxArr.Length; i++)
            {
                int stopIdx = stopIdxArr[i].Item1;
                int newConIdx = stopIdxArr[i].Item2;

                // Copy all parent genes up to the stop index.
                int copyLen = stopIdx - parentIdx;
                if(copyLen > 0)
                {
                    Array.Copy(parentConnArr, parentIdx, connArr, childIdx, copyLen);
                    Array.Copy(parentWeightArr, parentIdx, weightArr, childIdx, copyLen);
                    Array.Copy(parentIdArr, parentIdx, idArr, childIdx, copyLen);
                }

                // Update parentIdx, childIdx.
                parentIdx = stopIdx;
                childIdx += copyLen;

                // Test what to do at the stopIdx.
                if(-1 == newConIdx)
                {   // We are at the parent connection to be skipped.
                    parentIdx++;
                    continue;
                }

                // We are at an insertion point in connArr.
                connArr[childIdx] = newConnArr[newConIdx];
                weightArr[childIdx] = newWeightArr[newConIdx];
                idArr[childIdx] = newIdArr[newConIdx];

                // Record insertions.
                childInsertionArr[childInsertionIdx++] = (childIdx, newIdArr[newConIdx]);

                childIdx++;
            }

            // Copy any remaining connection genes.
            int len = parentConnArr.Length - parentIdx;
            if (len > 0)
            {
                Array.Copy(parentConnArr, parentIdx, connArr, childIdx, len);
                Array.Copy(parentWeightArr, parentIdx, weightArr, childIdx, len);
                Array.Copy(parentIdArr, parentIdx, idArr, childIdx, len);
            }

            // Note. We can construct a NeatGenome without passing the pre-built arrays connIdxArr and hiddenNodeIdArr;
            // however this way is more efficient. The downside is that the logic to pre-build these arrays is highly complex
            // and therefore difficult to understand, modify, and is thus a possible source of defects if modifications are attempted.

            // Create an array of indexes into the connection genes that gives the genes in order of innovation ID.
            var connIdxArr = Utils.CreateConnectionIndexArray(parent, splitConnId, splitConnIdx, childInsertionArr, insertIdx1, insertIdx2, newInnovationIdsFlag);

            // Create an array of hidden node IDs.
            var hiddenNodeIdArr = Utils.GetHiddenNodeIdArray(parent, addedNodeInfo.AddedNodeId, newInnovationIdsFlag);

            // Create and return a new genome.
            return new NeatGenome<T>(
                _metaNeatGenome,
                _genomeIdSeq.Next(), 
                _generationSeq.Peek,
                connGenes,
                connIdxArr,
                hiddenNodeIdArr,
                null);
        }

        #endregion

        #region Private Methods

        private AddedNodeInfo GetInnovationIDs(int splitConnId, NeatGenome<T> parent, out bool newInnovationIdsFlag)
        {
            // Test if the selected connection has a previous split recorded in the innovation ID buffer.
            AddedNodeInfo addedNodeInfo;
            if(_addedNodeBuffer.TryLookup(splitConnId, out addedNodeInfo))
            {
                // Found existing matching structure.
                // However we can only re-use the IDs from that structure if they aren't already present in the current genome;
                // this can happen e.g. if a previous split between the two chosen nodes has had one of its connections deleted
                // and another new connection was made, and we are now splitting that connection. Sexual reproduction could also 
                // result in the same or similar situation.
                // 
                // Therefore we only re-use IDs if we can re-use all three together, otherwise we aren't assigning the IDs to matching
                // structures throughout the population, which is the reason for ID re-use.
                if(    !parent.ContainsHiddenNode(addedNodeInfo.AddedNodeId)
                    && !parent.ContainsConnection(addedNodeInfo.AddedInputConnectionId)
                    && !parent.ContainsConnection(addedNodeInfo.AddedOutputConnectionId))
                {
                    // None of the ID are present on the parent genome, therefore we can re-use them.
                    newInnovationIdsFlag = false;
                    return addedNodeInfo;
                }

                // We can't re-use the IDs from the buffer, so allocate new IDs.
                // Note. these aren't added to the buffer; instead we leave the existing buffer entry for splitConnId.
                newInnovationIdsFlag = true;
                return new AddedNodeInfo(_innovationIdSeq);
            }

            // No buffer entry found, therefore we allocate new IDs.
            newInnovationIdsFlag = true;
            addedNodeInfo = new AddedNodeInfo(_innovationIdSeq);

            // Register the new IDs with the buffer.
            _addedNodeBuffer.Register(splitConnId, addedNodeInfo);

            return addedNodeInfo;
        }

        #endregion
    }
}
