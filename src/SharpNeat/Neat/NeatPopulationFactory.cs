/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;
using Redzen.Numerics;
using Redzen.Numerics.Distributions;
using Redzen.Numerics.Distributions.Double;
using Redzen.Random;
using Redzen.Structures;
using SharpNeat.Neat.Genome;
using SharpNeat.Graphs;

namespace SharpNeat.Neat
{
    /// <summary>
    /// Factory class for creating new instances of <see cref="NeatPopulationFactory{T}"/>.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public class NeatPopulationFactory<T> where T : struct
    {
        #region Instance Fields

        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly INeatGenomeBuilder<T> _genomeBuilder;
        readonly double _connectionsProportion;
        readonly DirectedConnection[] _connectionDefArr;

        readonly IRandomSource _rng;
        readonly Int32Sequence _genomeIdSeq;
        readonly Int32Sequence _innovationIdSeq;
        readonly IStatelessSampler<T> _connWeightDist;

        #endregion

        #region Constructor

        private NeatPopulationFactory(
            MetaNeatGenome<T> metaNeatGenome,
            double connectionsProportion,
            IRandomSource rng)
        {
            _metaNeatGenome = metaNeatGenome;
            _genomeBuilder = NeatGenomeBuilderFactory<T>.Create(metaNeatGenome);
            _connectionsProportion = connectionsProportion;

            // Define the set of all possible connections between the input and output nodes (fully interconnected).
            int inputCount = metaNeatGenome.InputNodeCount;
            int outputCount = metaNeatGenome.OutputNodeCount;
            _connectionDefArr = new DirectedConnection[inputCount * outputCount];

            // Notes.
            // Nodes are assigned innovation IDs. By convention the input nodes are assigned IDs first starting at zero, then the output nodes.
            // Thus, because all of the evolved networks have a fixed number of inputs and outputs, the IDs of these nodes are always fixed.
            int firstOutputNodeId = inputCount;

            for(int srcId=0, i=0; srcId < inputCount; srcId++) {
                for(int tgtIdx=0; tgtIdx < outputCount; tgtIdx++) {
                    _connectionDefArr[i++] = new DirectedConnection(srcId, firstOutputNodeId + tgtIdx);
                }
            }

            // Init RNG and ID sequences.
            _rng = rng;
            _genomeIdSeq = new Int32Sequence();
            int nextInnovationId = inputCount + outputCount;
            _innovationIdSeq = new Int32Sequence(nextInnovationId);

            // Init random connection weight source.
            _connWeightDist = UniformDistributionSamplerFactory.CreateStatelessSampler<T>(_metaNeatGenome.ConnectionWeightScale, true);

        }

        #endregion

        #region Private Methods

        private NeatPopulation<T> CreatePopulation(int size)
        {
            var genomeList = CreateGenomeList(size);
            return new NeatPopulation<T>(_metaNeatGenome, _genomeBuilder, genomeList, _genomeIdSeq, _innovationIdSeq);
        }

        /// <summary>
        /// Creates a list of randomly initialised genomes.
        /// </summary>
        /// <param name="count">The number of genomes to create.</param>
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
            int[] sampleArr = new int[connectionCount];
            DiscreteDistribution.SampleUniformWithoutReplacement(
                _rng, _connectionDefArr.Length, sampleArr);

            // Sort the samples.
            // Note. This results in the neural net connections being sorted by sourceID then targetID.
            Array.Sort(sampleArr);

            // Create the connection gene arrays and populate them.
            var connGenes = new ConnectionGenes<T>(connectionCount);
            var connArr = connGenes._connArr;
            var weightArr = connGenes._weightArr;

            for(int i=0; i < sampleArr.Length; i++)
            {
                DirectedConnection cdef = _connectionDefArr[sampleArr[i]];

                connArr[i] = new DirectedConnection(
                    cdef.SourceId,
                    cdef.TargetId);

                weightArr[i] = _connWeightDist.Sample(_rng);
            }

            // Get create a new genome with a new ID, birth generation of zero.
            int id = _genomeIdSeq.Next();
            return _genomeBuilder.Create(id, 0, connGenes);
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
        /// <returns>A new instance of <see cref="NeatPopulation{T}"/>.</returns>
        public static NeatPopulation<T> CreatePopulation(
            MetaNeatGenome<T> metaNeatGenome,
            double connectionsProportion, int popSize)
        {
            var factory = new NeatPopulationFactory<T>(metaNeatGenome, connectionsProportion, RandomDefaults.CreateRandomSource());
            return factory.CreatePopulation(popSize);
        }

        /// <summary>
        /// Create a new NeatPopulation with randomly initialised genomes.
        /// Genomes are randomly initialised by giving each a random subset of all possible connections between the input and output layer.
        /// </summary>
        /// <param name="metaNeatGenome">Genome metadata, e.g. the number of input and output nodes that each genome should have.</param>
        /// <param name="connectionsProportion">The proportion of possible connections between the input and output layers, to create in each new genome.</param>
        /// <param name="popSize">Population size. The number of new genomes to create.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>A new instance of <see cref="NeatPopulation{T}"/>.</returns>
        public static NeatPopulation<T> CreatePopulation(
            MetaNeatGenome<T> metaNeatGenome,
            double connectionsProportion, int popSize,
            IRandomSource rng)
        {
            var factory = new NeatPopulationFactory<T>(metaNeatGenome, connectionsProportion, rng);
            return factory.CreatePopulation(popSize);
        }

        #endregion
    }
}
