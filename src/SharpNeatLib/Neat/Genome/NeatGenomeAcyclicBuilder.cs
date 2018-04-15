using System;
using System.Diagnostics;
using Redzen.Sorting;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.Neat.Genome
{
    public class NeatGenomeAcyclicBuilder<T> : INeatGenomeBuilder<T>
        where T : struct
    {
        MetaNeatGenome<T> _metaNeatGenome;

        #region Constructor

        public NeatGenomeAcyclicBuilder(MetaNeatGenome<T> metaNeatGenome)
        {
            Debug.Assert(null != metaNeatGenome && metaNeatGenome.IsAcyclic);
            _metaNeatGenome = metaNeatGenome;
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
            int[] hiddenNodeIdArr = ConnectionGenesUtils.CreateHiddenNodeIdArray(connGenes._connArr, _metaNeatGenome.InputOutputNodeCount);

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
            INodeIdMap nodeIndexByIdMap = DirectedGraphUtils.CompileNodeIdMap(hiddenNodeIdArr, _metaNeatGenome.InputOutputNodeCount);

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

            // Calc the depth of each node in the digraph.
            GraphDepthInfo depthInfo = AcyclicGraphDepthAnalysis.CalculateNodeDepths(digraph);

            // Create a weighted acyclic digraph.
            // Note. This also outputs connectionIndexMap. For each connection in the acyclic graph this gives
            // the index of the same connection in the genome; this is because connections are re-ordered based 
            // on node depth in the acyclic graph.
            var acyclicDigraph = AcyclicDirectedGraphBuilderUtils.CreateAcyclicDirectedGraph(
                digraph,
                depthInfo,
                out int[] connectionIndexMap);

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
        /// <param name="digraph">A DirectedGraph that mirrors the structure described by the connection genes.</param>
        /// <param name="acyclicInfoCache">Cached info related to acyclic digraphs only.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public NeatGenome<T> Create(
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            INodeIdMap nodeIndexByIdMap,
            DirectedGraph digraph,
            int[] connectionIndexMap)
        {
            Debug.Assert(digraph is AcyclicDirectedGraph);
            // Mandatory when evolving acyclic genomes/graphs.
            Debug.Assert(null != connectionIndexMap);

            return new NeatGenome<T>(_metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap, digraph, connectionIndexMap);
        }

        #endregion
    }
}
