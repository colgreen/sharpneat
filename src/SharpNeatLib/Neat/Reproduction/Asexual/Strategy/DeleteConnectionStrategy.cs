using System;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
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

            // Get an array of hidden node IDs.
            var hiddenNodeIdArr = GetHiddenNodeIdArray(parent, deleteIdx, connArr);

            // Create and return a new genome.
            return new NeatGenome<T>(
                _metaNeatGenome,
                _genomeIdSeq.Next(), 
                _generationSeq.Peek,
                connGenes,
                connIdxArr,
                hiddenNodeIdArr);
        }

        #endregion

        #region Private Static Methods 

        private static int[] CreateConnectionIndexArray(NeatGenome<T> parent, int deleteIdx)
        {
            int childLen = parent.ConnectionGenes.Length - 1;

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

        #region Private Static Methods [GetHiddenNodeIdArray]

        /// <summary>
        /// Get an array of hidden node IDs in the child genome.
        /// </summary>
        private static int[] GetHiddenNodeIdArray(
            NeatGenome<T> parent, int deleteIdx,
            DirectedConnection[] childConnArr)
        {
            // Determine which hidden nodes in the parent have been deleted, i.e. are not in the child.
            (int?, int?) idTuple = GetDeletedNodeIds(parent, deleteIdx, childConnArr);
            if(!idTuple.Item1.HasValue && !idTuple.Item2.HasValue)
            {
                // The connection deleted resulting in no hidden nodes being deleted, therefore we can re-use
                // the parent's hidden node ID array.
                return parent.HiddenNodeIdArray;
            }

            // Determine the length of the new ID array, and allocate memory.
            int childLen = parent.HiddenNodeIdArray.Length;
            if(idTuple.Item1.HasValue && idTuple.Item2.HasValue) {
                childLen -= 2;
            } else {
                childLen--;
            }
            int[] childIdArr = new int[childLen];

            // Copy the parent's hidden node IDs, except the removed IDs.
            int[] parentIdArr = parent.HiddenNodeIdArray;
            for(int parentIdx=0, childIdx=0; parentIdx < parentIdArr.Length; parentIdx++)
            {
                int nodeId = parentIdArr[parentIdx];
                if(nodeId != idTuple.Item1 && nodeId != idTuple.Item2) {
                    childIdArr[childIdx++] = parentIdArr[parentIdx];
                }
            }

            return childIdArr;
        }

        /// <summary>
        /// Determine the set of hidden node IDs that have been deleted as a result of a connection deletion.
        /// I.e. a node only exists if a connection connects to it, therefore if there are no other connections
        /// referring to a node then it has been deleted, with the exception of input and output nodes that 
        /// always exist.
        /// </summary>
        private static (int?, int?) GetDeletedNodeIds(
            NeatGenome<T> parent, int deleteIdx,
            DirectedConnection[] childConnArr)
        {
            // Get the two node IDs referred to by the deleted connection.
            int? nodeId1 = parent.ConnectionGenes._connArr[deleteIdx].SourceId;
            int? nodeId2 = parent.ConnectionGenes._connArr[deleteIdx].TargetId;

            // Set IDs to null for input/output nodes (these nodes are fixed and therefore cannot be deleted).
            if(nodeId1.Value < parent.MetaNeatGenome.InputOutputNodeCount) {
                nodeId1 = null;
            }

            if(nodeId2.Value < parent.MetaNeatGenome.InputOutputNodeCount) {
                nodeId2 = null;
            }

            if(!nodeId1.HasValue && !nodeId2.HasValue) {
                return (null, null);
            }

            if(!nodeId1.HasValue)
            {
                // 'Shuffle up' nodeId2 into nodeId1.
                nodeId1 = nodeId2;
                nodeId2 = null;
            }

            if(!nodeId2.HasValue)
            {
                if(!IsNodeConnectedTo(childConnArr, nodeId1.Value)) {
                    return (nodeId1.Value, null);
                }
                return (null, null);
            }

            (bool, bool) isConnectedTuple = AreNodesConnectedTo(childConnArr, nodeId1.Value, nodeId2.Value);
            if(!isConnectedTuple.Item1 && !isConnectedTuple.Item2) {
                return (nodeId1.Value, nodeId2.Value);
            }

            if(!isConnectedTuple.Item1) {
                return (nodeId1.Value, null);
            }

            if(!isConnectedTuple.Item2) {
                return (nodeId2.Value, null);
            }

            return (null, null);
        }

        /// <summary>
        /// Is nodeId referred to by any of the connections in connArr.
        /// </summary>
        private static bool IsNodeConnectedTo(DirectedConnection[] connArr, int nodeId)
        {
            // Is nodeId referred to by any of the connections in connArr.
            foreach(var conn in connArr)
            {
                if(conn.SourceId == nodeId || conn.TargetId == nodeId) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Are nodeId1 and nodeId2 connected to by any of the connections in connArr.
        /// </summary>
        private static (bool, bool) AreNodesConnectedTo(DirectedConnection[] connArr, int nodeId1, int nodeId2)
        {
            bool id1Used = false;
            bool id2Used = false;

            foreach(var conn in connArr)
            {
                if(conn.SourceId == nodeId1 || conn.TargetId == nodeId1) 
                {
                    id1Used = true;
                    if(id2Used) {
                        break;
                    }
                }
                if(conn.SourceId == nodeId2 || conn.TargetId == nodeId2) 
                {
                    id2Used = true;
                    if(id1Used) {
                        break;
                    }
                }
            }
            return (id1Used, id2Used);
        }

        #endregion
    }
}
