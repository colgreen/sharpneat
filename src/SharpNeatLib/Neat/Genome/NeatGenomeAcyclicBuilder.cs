using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            // Create a DictionaryNodeIdMap.
            DictionaryNodeIdMap nodeIndexByIdMap = new DictionaryNodeIdMap(inputCount, nodeIdxById);

            // Create a digraph from the genome.
            DirectedGraph digraph = NeatGenomeBuilderUtils.CreateDirectedGraph(
                _metaNeatGenome, connGenes, nodeIndexByIdMap);

            // Calc the depth of each node in the digraph.
            GraphDepthInfo depthInfo = AcyclicGraphDepthAnalysis.CalculateNodeDepths(digraph);

            // Create a weighted acyclic digraph.
            // Note. This also outputs connectionIndexMap. For each connection in the acyclic graph this gives
            // the index of the same connection in the genome; this is because connections are re-ordered based 
            // on node depth in the acyclic graph.
            AcyclicDirectedGraph acyclicDigraph = AcyclicDirectedGraphBuilderUtils.CreateAcyclicDirectedGraph(
                digraph,
                depthInfo,
                out int[] newIdByOldId,
                out int[] connectionIndexMap);

            // TODO: Write unit tests to cover this!
            // Update nodeIdxById with the new depth based node index allocations.
            // Notes.
            // The current nodeIndexByIdMap maps node IDs (also know as innovation IDs in NEAT) to a compact 
            // ID space in which any gaps have been removed, i.e. a compacted set of IDs that can be used as indexes,
            // i.e. if there are N nodes in total then the highest node ID will be N-1.
            //
            // Here we map the new compact IDs to an alternative ID space that is also compact, but ensures that nodeIDs
            // reflect the depth of a node in the acyclic graph.

            // ENHANCEMENT: Avoid extraction of dictionary contents with ToArray(); 
            // this is done because a dictionary cannot be modified within an enumeration loop of that dictionary.
            KeyValuePair<int,int>[] kvpairArr = nodeIdxById.ToArray();
            foreach(var kvpair in kvpairArr) {
                nodeIdxById[kvpair.Key] = newIdByOldId[kvpair.Value];
            }

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
            return new NeatGenome<T>(_metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap, digraph, connectionIndexMap);
        }

        #endregion
    }
}
