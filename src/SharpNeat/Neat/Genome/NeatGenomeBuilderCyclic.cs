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
using System.Collections.Generic;
using System.Diagnostics;
using SharpNeat.Graphs;

namespace SharpNeat.Neat.Genome
{
    /// <summary>
    /// For building instances of <see cref="NeatGenome{T}"/>. For use when evolving cyclic graphs only.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public sealed class NeatGenomeBuilderCyclic<T> : INeatGenomeBuilder<T>
        where T : struct
    {
        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly HashSet<int> _workingIdSet;

        #region Constructor

        /// <summary>
        /// Construct with the given NEAT genome metadata.
        /// </summary>
        /// <param name="metaNeatGenome">NEAT genome metadata.</param>
        public NeatGenomeBuilderCyclic(MetaNeatGenome<T> metaNeatGenome)
        {
            Debug.Assert(metaNeatGenome is object && !metaNeatGenome.IsAcyclic);
            _metaNeatGenome = metaNeatGenome;
            _workingIdSet = new HashSet<int>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a NeatGenome with the given meta data and connection genes.
        /// </summary>
        /// <param name="id">Genome ID.</param>
        /// <param name="birthGeneration">Birth generation.</param>
        /// <param name="connGenes">Connection genes.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public NeatGenome<T> Create(
            int id,
            int birthGeneration,
            ConnectionGenes<T> connGenes)
        {
            // Determine the set of node IDs, and create a mapping from node IDs to node indexes.
            int[] hiddenNodeIdArr = ConnectionGenesUtils.CreateHiddenNodeIdArray(connGenes._connArr, _metaNeatGenome.InputOutputNodeCount, _workingIdSet);

            return Create(id, birthGeneration, connGenes, hiddenNodeIdArr);
        }

        /// <summary>
        /// Create a NeatGenome with the given meta data, connection genes and supplementary data.
        /// </summary>
        /// <param name="id">Genome ID.</param>
        /// <param name="birthGeneration">Birth generation.</param>
        /// <param name="connGenes">Connection genes.</param>
        /// <param name="hiddenNodeIdArr">An array of the hidden node IDs in the genome, in ascending order.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public NeatGenome<T> Create(
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr)
        {
            // Create a mapping from node IDs to node indexes.
            INodeIdMap nodeIndexByIdMap = DirectedGraphBuilderUtils.CompileNodeIdMap(
                _metaNeatGenome.InputOutputNodeCount, hiddenNodeIdArr);

            return Create(id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap);
        }

        /// <summary>
        /// Create a NeatGenome with the given meta data, connection genes and supplementary data.
        /// </summary>
        /// <param name="id">Genome ID.</param>
        /// <param name="birthGeneration">Birth generation.</param>
        /// <param name="connGenes">Connection genes.</param>
        /// <param name="hiddenNodeIdArr">An array of the hidden node IDs in the genome, in ascending order.</param>
        /// <param name="nodeIndexByIdMap">Provides a mapping from node ID to node index.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public NeatGenome<T> Create(
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            INodeIdMap nodeIndexByIdMap)
        {
            // Create a digraph from the genome.
            DirectedGraph digraph = NeatGenomeBuilderUtils.CreateDirectedGraph(
                _metaNeatGenome, connGenes, nodeIndexByIdMap);

            return new NeatGenome<T>(_metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap, digraph, null);
        }

        /// <summary>
        /// Create a NeatGenome with the given meta data, connection genes and supplementary data.
        /// </summary>
        /// <param name="id">Genome ID.</param>
        /// <param name="birthGeneration">Birth generation.</param>
        /// <param name="connGenes">Connection genes.</param>
        /// <param name="hiddenNodeIdArr">An array of the hidden node IDs in the genome, in ascending order.</param>
        /// <param name="nodeIndexByIdMap">Provides a mapping from node ID to node index.</param>
        /// <param name="digraph">A DirectedGraph that mirrors the structure described by the connection genes.</param>
        /// <param name="connectionIndexMap">Mapping from genome connection indexes (in NeatGenome.ConnectionGenes) to reordered connections, based on depth based
        /// node index allocations.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public NeatGenome<T> Create(
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            INodeIdMap nodeIndexByIdMap,
            DirectedGraph digraph,
            int[]? connectionIndexMap)
        {
            // This should always be null when evolving cyclic genomes/graphs.
            Debug.Assert(connectionIndexMap is null);

            return new NeatGenome<T>(_metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap, digraph, null);
        }

        #endregion
    }
}
