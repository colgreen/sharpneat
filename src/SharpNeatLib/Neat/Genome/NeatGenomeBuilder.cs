using System;
using System.Collections.Generic;
using System.Diagnostics;
using Redzen.Sorting;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.Neat.Genome
{
    public class NeatGenomeBuilder<T> : INeatGenomeBuilder<T>
        where T : struct
    {
        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly HashSet<int> _workingIdSet;

        #region Constructor

        public NeatGenomeBuilder(MetaNeatGenome<T> metaNeatGenome)
        {
            Debug.Assert(null != metaNeatGenome && !metaNeatGenome.IsAcyclic);
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
            INodeIdMap nodeIndexByIdMap = DirectedGraphUtils.CompileNodeIdMap_InputOutputCount_HiddenNodeIdArr(
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
            int[] connectionIndexMap)
        {
            // This should always be null when evolving cyclic genomes/graphs.
            Debug.Assert(null == connectionIndexMap);

            return new NeatGenome<T>(_metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap, digraph, null);
        }

        #endregion
    }
}
