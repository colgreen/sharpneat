using System;
using System.Diagnostics;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Utils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    /// <summary>
    /// Add acyclic connection, asexual reproduction strategy.
    /// </summary>
    /// <typeparam name="T">Connection weight type.</typeparam>
    public class AddAcyclicConnectionReproductionStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        #region Instance Fields

        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _innovationIdSeq;
        readonly Int32Sequence _generationSeq;
        readonly AddedConnectionBuffer _addedConnectionBuffer;

        readonly IContinuousDistribution<T> _weightDistA;
        readonly IContinuousDistribution<T> _weightDistB;
        readonly IRandomSource _rng;
        readonly CyclicConnectionTest<T> _cyclicTest;

        #endregion

        #region Constructor

        public AddAcyclicConnectionReproductionStrategy(
            MetaNeatGenome<T> metaNeatGenome,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq,
            Int32Sequence generationSeq,
            AddedConnectionBuffer addedConnectionBuffer)
        {
            _metaNeatGenome = metaNeatGenome;
            _genomeIdSeq = genomeIdSeq;
            _innovationIdSeq = innovationIdSeq;
            _generationSeq = generationSeq;
            _addedConnectionBuffer = addedConnectionBuffer;

            _weightDistA = ContinuousDistributionFactory.CreateUniformDistribution<T>(metaNeatGenome.ConnectionWeightRange, true);
            _weightDistB = ContinuousDistributionFactory.CreateUniformDistribution<T>(metaNeatGenome.ConnectionWeightRange * 0.01, true);
            _rng = RandomSourceFactory.Create();
            _cyclicTest = new CyclicConnectionTest<T>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new child genome from a given parent genome.
        /// </summary>
        /// <param name="parent">The parent genome.</param>
        /// <returns>A new child genome.</returns>
        public NeatGenome<T> CreateChildGenome(NeatGenome<T> parent)
        {
            Debug.Assert(_metaNeatGenome == parent.MetaNeatGenome, "Parent genome has unexpected MetaNeatGenome.");

            // Attempt to find a new connection that we can add to the genome.
            DirectedConnection directedConn;
            if(!TryGetConnection(parent, out directedConn, out int insertIdx))
            {   // Failed to find a new connection.
                return null;
            }

            // Determine the new gene's innovation ID.
            bool highInnovationId = false;
            if(!_addedConnectionBuffer.TryLookup(directedConn, out int connectionId))
            {   
                // No matching connection found in the innovation ID buffer.
                // Get a new innovation ID and register the new connection with the innovation buffer.
                connectionId = _innovationIdSeq.Next();
                _addedConnectionBuffer.Register(directedConn, connectionId);
                highInnovationId = true;
            }

            // Determine the connection weight.
            // 50% of the time use weights very close to zero.
            // Note. this recreates the strategy used in SharpNEAT 2.x.
            // TODO: Reconsider the distribution of new weights and if there are better approaches (distributions) we could use.
            T weight = _rng.NextBool() ? _weightDistB.Sample() : _weightDistA.Sample();

            // Create a new connection gene array that consists of the parent connection genes plus the new gene
            // inserted at the correct (sorted) position.
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

            // Copy genes up to insertIdx.
            Array.Copy(parentConnArr, connArr, insertIdx);
            Array.Copy(parentWeightArr, weightArr, insertIdx);
            Array.Copy(parentIdArr, idArr, insertIdx);

            // Copy the new genome into its insertion point.
            connArr[insertIdx] = new DirectedConnection(
                directedConn.SourceId,
                directedConn.TargetId);

            weightArr[insertIdx] = weight;
            idArr[insertIdx] = connectionId;

            // Copy remaining genes (if any).
            Array.Copy(parentConnArr, insertIdx, connArr, insertIdx+1, parentLen-insertIdx);
            Array.Copy(parentWeightArr, insertIdx, weightArr, insertIdx+1, parentLen-insertIdx);
            Array.Copy(parentIdArr, insertIdx, idArr, insertIdx+1, parentLen-insertIdx);

            // Create an array of indexes into the connection genes that gives the genes in order of innovation ID.
            // Note. We can construct a NeatGenome without passing connIdxArr and it will re-calc it; however this 
            // way is more efficient.
            int[] connIdxArr = AddConnectionUtils.CreateConnectionIndexArray(parent, insertIdx, connectionId, highInnovationId);

            // Create and return a new genome.
            return new NeatGenome<T>(
                _metaNeatGenome,
                _genomeIdSeq.Next(), 
                _generationSeq.Peek,
                connGenes,
                connIdxArr);
        }

        #endregion

        #region Private Methods

        private bool TryGetConnection(NeatGenome<T> parent, out DirectedConnection conn, out int insertIdx)
        {
            // Get a sorted array of all node IDs in the parent genome (includes input, output and hidden nodes).
            int[] idArr = AddConnectionUtils.CreateNodeIdArray(
                parent.ConnectionGenes._connArr,
                _metaNeatGenome.InputNodeCount + _metaNeatGenome.OutputNodeCount);

            // Make several attempts at find a new connection, if not successful then give up.
            for(int attempts=0; attempts < 5; attempts++)
            {
                if(TryGetConnectionInner(parent, idArr, out conn, out insertIdx)) {
                    return true;
                }
            }

            conn = default(DirectedConnection);
            insertIdx = default(int);
            return false;
        }

        private bool TryGetConnectionInner(NeatGenome<T> parent, int[] idArr, out DirectedConnection conn, out int insertIdx)
        {
            // Select a source node at random.
            
            // Note. Valid source nodes are input and hidden nodes. Output nodes are not source node candidates
            // for acyclic nets, because that can prevent future connections from targeting the output if it would
            // create a cycle.
            int inputCount = _metaNeatGenome.InputNodeCount;
            int outputCount = _metaNeatGenome.OutputNodeCount;

            int srcIdx = _rng.Next(idArr.Length - outputCount);
            if(srcIdx >= inputCount) {
                srcIdx += outputCount;
            }
            int srcId = idArr[srcIdx];

            // Select a target node at random.
            // Note. Valid target nodes are all hidden and output nodes (cannot be an input node).
            int tgtId = idArr[inputCount + _rng.Next(idArr.Length - inputCount)];

            // Test for simplest cyclic connectivity - node connects to itself.
            if(srcId == tgtId)
            {   
                conn = default(DirectedConnection);
                insertIdx = default(int);
                return false;
            }

            // Test if the chosen connection already exists.
            // Note. Connection genes are always sorted by sourceId then targetId, so we can use a binary search to 
            // find an existing connection in O(log(n)) time.
            conn = new DirectedConnection(srcId, tgtId);

            if((insertIdx = Array.BinarySearch(parent.ConnectionGenes._connArr, conn)) >= 0)
            {   
                // The proposed new connection already exists.
                conn = default(DirectedConnection);
                insertIdx = default(int);
                return false;
            }

            // Test if the connection will form a cycle in the wider network.
            if(_cyclicTest.IsConnectionCyclic(parent.ConnectionGenes._connArr, conn))
            {
                conn = default(DirectedConnection);
                insertIdx = default(int);
                return false;
            }

            // Get the position in parent.ConnectionGeneArray that the new connection should be inserted at (to maintain sort order).
            insertIdx = ~insertIdx;
            return true;
        }

        #endregion
    }
}
