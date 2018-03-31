using System;
using System.Diagnostics;
using Redzen.Random;
using Redzen.Structures;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeatLib.Neat.Genome;
using static SharpNeat.Neat.Reproduction.Asexual.Strategy.AddConnectionUtils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    /// <summary>
    /// Add acyclic connection, asexual reproduction strategy.
    /// </summary>
    /// <typeparam name="T">Connection weight type.</typeparam>
    public class AddAcyclicConnectionStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        #region Instance Fields

        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly INeatGenomeFactory<T> _genomeFactory;
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _innovationIdSeq;
        readonly Int32Sequence _generationSeq;

        readonly IContinuousDistribution<T> _weightDistA;
        readonly IContinuousDistribution<T> _weightDistB;
        readonly IRandomSource _rng;
        readonly CyclicConnectionTest _cyclicTest;

        #endregion

        #region Constructor

        public AddAcyclicConnectionStrategy(
            MetaNeatGenome<T> metaNeatGenome,
            INeatGenomeFactory<T> genomeFactory,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq,
            Int32Sequence generationSeq)
        {
            _metaNeatGenome = metaNeatGenome;
            _genomeFactory = genomeFactory;
            _genomeIdSeq = genomeIdSeq;
            _innovationIdSeq = innovationIdSeq;
            _generationSeq = generationSeq;

            _weightDistA = ContinuousDistributionFactory.CreateUniformDistribution<T>(metaNeatGenome.ConnectionWeightRange, true);
            _weightDistB = ContinuousDistributionFactory.CreateUniformDistribution<T>(metaNeatGenome.ConnectionWeightRange * 0.01, true);
            _rng = RandomSourceFactory.Create();
            _cyclicTest = new CyclicConnectionTest();
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

            // Determine the connection weight.
            // 50% of the time use weights very close to zero.
            // Note. this recreates the strategy used in SharpNEAT 2.x.
            // ENHANCEMENT: Reconsider the distribution of new weights and if there are better approaches (distributions) we could use.
            T weight = _rng.NextBool() ? _weightDistB.Sample() : _weightDistA.Sample();

            // Create a new connection gene array that consists of the parent connection genes plus the new gene
            // inserted at the correct (sorted) position.
            var parentConnArr = parent.ConnectionGenes._connArr;
            var parentWeightArr = parent.ConnectionGenes._weightArr;
            int parentLen = parentConnArr.Length;

            // Create the child genome's ConnectionGenes object.
            int childLen = parentLen + 1;
            var connGenes = new ConnectionGenes<T>(childLen);
            var connArr = connGenes._connArr;
            var weightArr = connGenes._weightArr;

            // Copy genes up to insertIdx.
            Array.Copy(parentConnArr, connArr, insertIdx);
            Array.Copy(parentWeightArr, weightArr, insertIdx);

            // Copy the new genome into its insertion point.
            connArr[insertIdx] = new DirectedConnection(
                directedConn.SourceId,
                directedConn.TargetId);

            weightArr[insertIdx] = weight;

            // Copy remaining genes (if any).
            Array.Copy(parentConnArr, insertIdx, connArr, insertIdx+1, parentLen-insertIdx);
            Array.Copy(parentWeightArr, insertIdx, weightArr, insertIdx+1, parentLen-insertIdx);

            // Create and return a new genome.
            // Note. The set of hidden node IDs remains unchanged from the parent, therefore we are able to re-use parent.HiddenNodeIdArray.
            return _genomeFactory.Create(
                _metaNeatGenome,
                _genomeIdSeq.Next(), 
                _generationSeq.Peek,
                connGenes,
                parent.HiddenNodeIdArray,
                parent.NodeIndexByIdMap);
        }

        #endregion

        #region Private Methods

        private bool TryGetConnection(NeatGenome<T> parent, out DirectedConnection conn, out int insertIdx)
        {
            // Make several attempts at find a new connection, if not successful then give up.
            for(int attempts=0; attempts < 5; attempts++)
            {
                if(TryGetConnectionInner(parent, out conn, out insertIdx)) {
                    return true;
                }
            }

            conn = default(DirectedConnection);
            insertIdx = default(int);
            return false;
        }

        private bool TryGetConnectionInner(NeatGenome<T> parent, out DirectedConnection conn, out int insertIdx)
        {
            int inputCount = _metaNeatGenome.InputNodeCount;
            int outputCount = _metaNeatGenome.OutputNodeCount;
            int hiddenCount = parent.HiddenNodeIdArray.Length;

            // Select a source node at random.
            
            // Note. Valid source nodes are input and hidden nodes. Output nodes are not source node candidates
            // for acyclic nets, because that can prevent future connections from targeting the output if it would
            // create a cycle.
            int inputHiddenCount = inputCount + hiddenCount;
            int srcIdx = _rng.Next(inputHiddenCount);

            if(srcIdx >= inputCount) {
                srcIdx += outputCount;
            }
            int srcId = GetNodeIdFromIndex(parent, srcIdx);

            // Select a target node at random.
            // Note. Valid target nodes are all hidden and output nodes (cannot be an input node).
            int outputHiddenCount = outputCount + hiddenCount;
            int tgtId = GetNodeIdFromIndex(parent, inputCount + _rng.Next(outputHiddenCount));;

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
            int totalNodeCount = _metaNeatGenome.InputOutputNodeCount + hiddenCount;
            if(_cyclicTest.IsConnectionCyclic(parent.ConnectionGenes._connArr, parent.NodeIndexByIdMap, totalNodeCount, conn))
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
