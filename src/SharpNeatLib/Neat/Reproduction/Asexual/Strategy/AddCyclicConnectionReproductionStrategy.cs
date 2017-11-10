using System;
using System.Collections.Generic;
using System.Linq;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public class AddCyclicConnectionReproductionStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        #region Instance Fields

        NeatPopulation<T> _pop;
        IContinuousDistribution<T> _weightDistA;
        IContinuousDistribution<T> _weightDistB;
        IRandomSource _rng;

        #endregion

        #region Constructor

        public AddCyclicConnectionReproductionStrategy(NeatPopulation<T> pop)
        {
            _pop = pop;
            _weightDistA = ContinuousDistributionFactory.CreateUniformDistribution<T>(_pop.MetaNeatGenome.ConnectionWeightRange, true);
            _weightDistB = ContinuousDistributionFactory.CreateUniformDistribution<T>(_pop.MetaNeatGenome.ConnectionWeightRange * 0.01, true);
            _rng = RandomSourceFactory.Create();
        }

        #endregion

        #region Public Methods

        public NeatGenome<T> CreateChildGenome(NeatGenome<T> parent)
        {
            // Attempt to find a new connection that we can add to the genome.
            if(!TryGetConnection(parent, out DirectedConnection directedConn, out int insertIdx))
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
                _pop.InnovationIdSeq.Next(),
                directedConn.SourceId,
                directedConn.TargetId,
                weight
            );

            // Create a new connection gene array that consists of the parent connection genes plus the new gene
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
                _pop.MetaNeatGenome,
                _pop.GenomeIdSeq.Next(), 
                _pop.CurrentGenerationAge,
                connArr);
        }

        #endregion

        #region Private Methods

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
            // Note. this can be any node (input, output or hidden).
            int srcId = idArr[_rng.Next(idArr.Length)];

            // Select a target node at random.
            // Note. This cannot be an input node (so must be a hidden or output node).
            int inputCount = _pop.MetaNeatGenome.InputNodeCount;
            int tgtId = idArr[inputCount + _rng.Next(idArr.Length - inputCount)];

            // Test if the chosen connection already exists.
            // Note. Connection genes are always sorted by sourceId then targetId, so we can use a binary search to 
            // find an existing connection in O(log(n)) time.
            conn = new DirectedConnection(srcId, tgtId);

            if((insertIdx = Array.BinarySearch(parent.ConnectionGeneArray, (IDirectedConnection)conn, DirectedConnectionComparer.__Instance)) < 0)
            {   
                // The proposed new connection does not already exist, therefore we can use it.
                // Get the position in parent.ConnectionGeneArray that the new connection should be inserted at (to maintain sort order).
                insertIdx = ~insertIdx;
                return true;
            }

            conn = default(DirectedConnection);
            insertIdx = default(int);
            return false;
        }

        private int[] CreateNodeIdArray(NeatGenome<T> parent)
        {
            // Determine the set of node IDs in the parent genome.
            var parentConnArr = parent.ConnectionGeneArray;
            int parentLen = parentConnArr.Length;
            var idSet = new HashSet<int>();

            // Include invariant nodes (input and output nodes).
            // Note. These nodes have fixed predetermined IDs.
            int ioCount = _pop.MetaNeatGenome.InputNodeCount + _pop.MetaNeatGenome.OutputNodeCount;
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

        #endregion
    }
}
