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
using System.Diagnostics;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.Neat.Genome
{
    /// <summary>
    /// For building instances of <see cref="NeatGenome{T}"/>. For use when evolving acyclic graphs only.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public class NeatGenomeBuilderAcyclic<T> : INeatGenomeBuilder<T>
        where T : struct
    {
        #region Instance Fields

        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly AcyclicGraphDepthAnalysis _graphDepthAnalysis;
        readonly HashSet<int> _workingIdSet;

        // Temp working data for timsort. Allocated once and re-used to minimise object allocate and GC overhead.
        int[] _timesortWorkArr;
        int[] _timesortWorkVArr;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the given NEAT genome metadata.
        /// </summary>
        /// <param name="metaNeatGenome">NEAT genome metadata.</param>
        public NeatGenomeBuilderAcyclic(MetaNeatGenome<T> metaNeatGenome)
        {
            Debug.Assert(null != metaNeatGenome && metaNeatGenome.IsAcyclic);
            _metaNeatGenome = metaNeatGenome;
            _graphDepthAnalysis = new AcyclicGraphDepthAnalysis();
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
            int inputCount = _metaNeatGenome.InputNodeCount;

            // Create a mapping from node IDs to node indexes.
            Dictionary<int,int> nodeIdxById = BuildNodeIndexById(hiddenNodeIdArr);

            // Create a DictionaryNodeIdMap.
            DictionaryNodeIdMap nodeIndexByIdMap = new DictionaryNodeIdMap(inputCount, nodeIdxById);

            // Create a digraph from the genome.
            DirectedGraph digraph = NeatGenomeBuilderUtils.CreateDirectedGraph(
                _metaNeatGenome, connGenes, nodeIndexByIdMap);

            // Calc the depth of each node in the digraph.
            GraphDepthInfo depthInfo = _graphDepthAnalysis.CalculateNodeDepths(digraph);

            // Create a weighted acyclic digraph.
            // Note. This also outputs connectionIndexMap. For each connection in the acyclic graph this gives
            // the index of the same connection in the genome; this is because connections are re-ordered based 
            // on node depth in the acyclic graph.
            DirectedGraphAcyclic acyclicDigraph = DirectedGraphAcyclicBuilderUtils.CreateAcyclicDirectedGraph(
                digraph,
                depthInfo,
                out int[] newIdByOldId,
                out int[] connectionIndexMap,
                ref _timesortWorkArr,
                ref _timesortWorkVArr);

            // TODO: Write unit tests to cover this!
            // Update nodeIdxById with the new depth based node index allocations.
            // Notes.
            // The current nodeIndexByIdMap maps node IDs (also know as innovation IDs in NEAT) to a compact 
            // ID space in which any gaps have been removed, i.e. a compacted set of IDs that can be used as indexes,
            // i.e. if there are N nodes in total then the highest node ID will be N-1.
            //
            // Here we map the new compact IDs to an alternative ID space that is also compact, but ensures that nodeIDs
            // reflect the depth of a node in the acyclic graph.
            UpdateNodeIndexById(nodeIdxById, hiddenNodeIdArr, newIdByOldId);

            // Create the neat genome.
            return new NeatGenome<T>(
                _metaNeatGenome, id, birthGeneration,
                connGenes,
                hiddenNodeIdArr,
                nodeIndexByIdMap,
                acyclicDigraph,
                connectionIndexMap);
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
            // Note. Not required for acyclic graphs.
            // In acyclic graphs nodeIndexByIdMap is so closely related/tied to digraph and connectionIndexMap that 
            // these three objects exist as a logical unit, i.e. we get all three or none at all.
            throw new NotImplementedException();
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
        /// <param name="connectionIndexMap">A mapping between genome connection indexes (in NeatGenome.ConnectionGenes), to reordered connections
        /// based on depth based node index allocations (as utilised in AcyclicDirectedGraph).</param>
        /// <returns>A new NeatGenome instance.</returns>
        public NeatGenome<T> Create(
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            INodeIdMap nodeIndexByIdMap,
            DirectedGraph digraph,
            int[] connectionIndexMap)
        {
            return new NeatGenome<T>(_metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap, digraph, connectionIndexMap);
        }

        #endregion

        #region Private Methods

        private Dictionary<int,int> BuildNodeIndexById(int[] hiddenNodeIdArr)
        {
            int inputCount = _metaNeatGenome.InputNodeCount;
            int outputCount = _metaNeatGenome.OutputNodeCount;
            int inputOutputCount = _metaNeatGenome.InputOutputNodeCount;

            // Create a mapping from node IDs to node indexes.
            var nodeIdxById = new Dictionary<int,int>(outputCount + hiddenNodeIdArr.Length);

            // Insert fixed output node IDs (these will become unfixed later, hence they are added to the dictionary
            // rather than just being covered by DictionaryNodeIdMap.fixedNodeCount.)
            // Output node indexes start after the last input node index.
            for(int nodeIdx = inputCount; nodeIdx < inputOutputCount; nodeIdx++) {
                nodeIdxById.Add(nodeIdx, nodeIdx);
            }

            // Insert the hidden node ID mappings. Hidden nodes are allocated node indexes starting directly
            // after the last output node index.
            for(int i=0, nodeIdx = inputOutputCount; i < hiddenNodeIdArr.Length; i++) {
                nodeIdxById.Add(hiddenNodeIdArr[i], nodeIdx + i);
            }

            return nodeIdxById;
        }

        private void UpdateNodeIndexById(
            Dictionary<int,int> nodeIdxById,
            int[] hiddenNodeIdArr,
            int[] newIdxByOldIdx)
        {
            // Note. This method essentially repeats the logic in BuildNodeIndexById() but 
            // the values placed into the dictionary are mapped to different values, and
            // we are updating existing dictionary entries rather than inserting new ones.
            //
            // This still requires dictionary lookups and so can be optimised further with
            // a customised dictionary implementation that allows direct access and updating 
            // of the keyed values. We could do that here by wrapping each entry in an object
            // reference (i.e. boxing), but that creates additional overhead (object header 
            // allocation, heap allocation, garbage collection, etc.).
            int inputCount = _metaNeatGenome.InputNodeCount;
            int inputOutputCount = _metaNeatGenome.InputOutputNodeCount;

            // Update the fixed output node IDs.
            // Pre-mapped output node indexes start after the last input node index.
            for(int nodeIdx = inputCount; nodeIdx < inputOutputCount; nodeIdx++) {
                nodeIdxById[nodeIdx] = newIdxByOldIdx[nodeIdx];
            }

            // Update the hidden node ID mappings.
            // Pre-mapped hidden nodes are allocated node indexes starting directly
            // after the last output node index.
            for(int i=0, nodeIdx = inputOutputCount; i < hiddenNodeIdArr.Length; i++) {
                nodeIdxById[hiddenNodeIdArr[i]] = newIdxByOldIdx[nodeIdx + i];
            }
        }

        #endregion
    }
}
