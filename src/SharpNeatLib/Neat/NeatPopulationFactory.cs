using System;
using System.Collections.Generic;
using Redzen.Numerics;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Utils;

namespace SharpNeat.Neat
{
    public class NeatPopulationFactory<T> where T : struct
    {
        #region Instance Fields

        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly double _connectionsProportion;

        readonly ConnectionDefinition[] _connectionDefArr;
        
        readonly IRandomSource _rng;
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _innovationIdSeq;        
        readonly IUniformDistribution<T> _connWeightDist;

        #endregion

        #region Constructor

        private NeatPopulationFactory(MetaNeatGenome<T> metaNeatGenome, double connectionsProportion)
        {
            _metaNeatGenome = metaNeatGenome;
            _connectionsProportion = connectionsProportion;

            // Define the set of all possible connections between the input and output nodes (fully interconnected).
            int inputCount = metaNeatGenome.InputNodeCount;
            int outputCount = metaNeatGenome.OutputNodeCount;
            _connectionDefArr = new ConnectionDefinition[inputCount * outputCount];

            // Notes.
            // Connections and nodes are assigned innovation IDs from the same ID space (from the same 'pool' of numbers).
            // By convention the input nodes are assigned IDs first starting at zero, then the output nodes. Thus, because all 
            // of the evolved networks have a fixed number of inputs and outputs, the IDs of these nodes are fixed by convention.
            // Here we also allocate IDs to connections, and these start at the first ID after the last output node. From there evolution
            // will create connections and nodes, and IDs are allocated in whatever order the nodes and connections are created in.
            int firstOutputNodeId = inputCount;
            int nextInnovationId = inputCount + outputCount;

            for(int srcId=0, i=0; srcId < inputCount; srcId++) {    
                for(int tgtIdx=0; tgtIdx < outputCount; tgtIdx++, nextInnovationId++) {
                    _connectionDefArr[i++] = new ConnectionDefinition(nextInnovationId, srcId, firstOutputNodeId + tgtIdx);
                }
            }

            // Init RNG and ID sequences.
            _rng = RandomSourceFactory.Create();
            _genomeIdSeq = new Int32Sequence();
            _innovationIdSeq = new Int32Sequence(nextInnovationId);

            // Init random connection weight source.
            _connWeightDist = ContinuousDistributionFactory.CreateUniformDistribution<T>(_metaNeatGenome.ConnectionWeightRange, true);

        }

        #endregion

        #region Private Methods

        public NeatPopulation<T> CreatePopulation(int size)
        {
            var genomeList = CreateGenomeList(size);
            return new NeatPopulation<T>(_genomeIdSeq, _innovationIdSeq, genomeList, _metaNeatGenome);
        }

        /// <summary>
        /// Creates a list of randomly initialised genomes.
        /// </summary>
        /// <param name="length">The number of genomes to create.</param>
        private List<NeatGenome<T>> CreateGenomeList(int count)
        {
            List<NeatGenome<T>> genomeList = new List<NeatGenome<T>>(count);
            for(int i=0; i < count; i++) 
            {
                NeatGenome<T> genome = CreateGenome();
                genomeList.Add(genome);
            }
            return genomeList;
        }

        /// <summary>
        /// Creates a single randomly initialised genome.
        /// </summary>
        private NeatGenome<T> CreateGenome()
        {
            // Determine how many connections to create in the new genome, as a proportion of all possible connections
            // between the input and output nodes.
            int connectionCount = (int)NumericsUtils.ProbabilisticRound(_connectionDefArr.Length * _connectionsProportion, _rng);

            // Ensure there is at least one connection.
            connectionCount = Math.Max(1, connectionCount);

            // Select a random subset of all possible connections between the input and output nodes.
            int totalConnectionCount = _connectionDefArr.Length;
            int[] sampleArr = new int[connectionCount];
            DiscreteDistributionUtils.SampleUniformWithoutReplacement(totalConnectionCount, sampleArr, _rng);

            // Sort the samples.
            // Note. This helps keep the neural net connections (and thus memory accesses) in-order.
            Array.Sort(sampleArr);

            // Create the connection gene list and populate it.
            var connectionGeneArr = new ConnectionGene<T>[connectionCount];

            for(int i=0; i < sampleArr.Length; i++)
            {
                ConnectionDefinition cdef = _connectionDefArr[sampleArr[i]];

                ConnectionGene<T> cgene = new ConnectionGene<T>(
                    cdef._connectionId,
                    cdef._srcNodeId,
                    cdef._tgtNodeId,
                    _connWeightDist.Sample(_metaNeatGenome.ConnectionWeightRange, true));

                connectionGeneArr[i] = cgene;
            }

            // Get create a new genome with a new ID, birth generation of zero.
            int id = _genomeIdSeq.Next();
            return new NeatGenome<T>(_metaNeatGenome, id, 0, connectionGeneArr);
        }

        #endregion

        #region Public Static Factory Method

        /// <summary>
        /// Create a new NeatPopulation with randomly initialised genomes.
        /// Genomes are randomly initialised by giving each a random subset of all possible connections between the input and output layer.
        /// </summary>
        /// <param name="metaNeatGenome">Genome metadata, e.g. the number of input and output nodes that each genome should have.</param>
        /// <param name="connectionsProportion">The proportion of possible connections between the input and output layers, to create in each new genome.</param>
        /// <param name="popSize">Population size. The number of new genomes to create.</param>
        /// <returns>A new NeatPopulation.</returns>
        public static NeatPopulation<T> CreatePopulation(MetaNeatGenome<T> metaNeatGenome, double connectionsProportion, int popSize)
        {
            var factory = new NeatPopulationFactory<T>(metaNeatGenome, connectionsProportion);
            return factory.CreatePopulation(popSize);
        }

        #endregion

        #region Inner Class [ConnectionDefinition]

        struct ConnectionDefinition
        {
            public readonly int _connectionId;
            public readonly int _srcNodeId;
            public readonly int _tgtNodeId;

            public ConnectionDefinition(int innovationId, int srcNodeId, int tgtNodeId)
            {
                _connectionId = innovationId;
                _srcNodeId = srcNodeId;
                _tgtNodeId = tgtNodeId;
            }
        }

        #endregion
    }
}
