using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Utils;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public class AddAcyclicConnectionReproductionStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        #region Instance Fields

        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _innovationIdSeq;
        readonly Int32Sequence _generationSeq;

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
            Int32Sequence generationSeq)
        {
            _metaNeatGenome = metaNeatGenome;
            _genomeIdSeq = genomeIdSeq;
            _innovationIdSeq = innovationIdSeq;
            _generationSeq = generationSeq;

            _weightDistA = ContinuousDistributionFactory.CreateUniformDistribution<T>(metaNeatGenome.ConnectionWeightRange, true);
            _weightDistB = ContinuousDistributionFactory.CreateUniformDistribution<T>(metaNeatGenome.ConnectionWeightRange * 0.01, true);
            _rng = RandomSourceFactory.Create();
            _cyclicTest = new CyclicConnectionTest<T>();
        }

        #endregion

        #region Public Methods

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
            // TODO: Reconsider the distribution of new weights and if there are better approaches (distributions) we could use.
            T weight = _rng.NextBool() ? _weightDistB.Sample() : _weightDistA.Sample();

            // Create a new connection gene.
            var connGene = new ConnectionGene<T>(
                _innovationIdSeq.Next(),
                directedConn.SourceId,
                directedConn.TargetId,
                weight
            );

            // Create a new connection gene array that consists of the parent connection genes plus the new gene,
            // inserted at the correct (sorted) position.
            var parentConnArr = parent.ConnectionGeneArray;
            int parentLen = parentConnArr.Length;

            // Alloc child gene array.
            int childLen = parentLen + 1;
            var connArr = new ConnectionGene<T>[childLen];


            // Copy genes up to insertIdx.
            for(int i=0; i < insertIdx; i++) {
                connArr[i] = new ConnectionGene<T>(parentConnArr[i]);
            }

            // Copy the new genome into its insertion point.
            connArr[insertIdx] = connGene;

            // Copy remaining genes (if any).
            for(int i=insertIdx+1; i < childLen; i++) {
                connArr[i] = new ConnectionGene<T>(parentConnArr[i-1]);
            }

            // Create and return a new genome.
            return new NeatGenome<T>(
                _metaNeatGenome,
                _genomeIdSeq.Next(), 
                _generationSeq.Peek,
                connArr);
        }

        #endregion

        #region Private Methods

        private int[] CreateNodeIdArray(NeatGenome<T> parent)
        {
            // Determine the set of node IDs in the parent genome.
            var parentConnArr = parent.ConnectionGeneArray;
            int parentLen = parentConnArr.Length;
            var idSet = new HashSet<int>();

            // Include invariant nodes (input and output nodes).
            // Note. These nodes have fixed predetermined IDs.
            int ioCount = _metaNeatGenome.InputNodeCount + _metaNeatGenome.OutputNodeCount;
            for(int i=0; i<ioCount; i++) {
                idSet.Add(i);
            }

            // Ensure all other (hidden) nodes are included.
            for(int i=0; i<parentLen; i++) 
            {
                idSet.Add(parentConnArr[i].SourceId);
                idSet.Add(parentConnArr[i].TargetId);
            }
            int[] idArr = idSet.ToArray();
            Array.Sort(idArr);
            return idArr;
        }

        private bool TryGetConnection(NeatGenome<T> parent, out DirectedConnection conn, out int insertIdx)
        {
            // Get a sorted array of all node IDs in the parent genome (includes input, output and hidden nodes).
            int[] idArr = CreateNodeIdArray(parent);

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
            int ouputCount = _metaNeatGenome.OutputNodeCount;
            int srcId = idArr[_rng.Next(idArr.Length - ouputCount)];
            if(srcId >= inputCount) {
                srcId += ouputCount;
            }

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

            if((insertIdx = Array.BinarySearch(parent.ConnectionGeneArray, (IDirectedConnection)conn, DirectedConnectionComparer.__Instance)) >= 0)
            {   
                // The proposed new connection already exists.
                conn = default(DirectedConnection);
                insertIdx = default(int);
                return false;
            }

            // Test if the connection will form a cycle in the wider network.
            if(_cyclicTest.IsConnectionCyclic(parent.ConnectionGeneArray, conn))
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
