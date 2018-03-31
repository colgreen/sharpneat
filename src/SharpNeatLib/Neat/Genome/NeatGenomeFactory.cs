using System;
using System.Diagnostics;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;
using SharpNeatLib.Neat.Genome;
using SharpNeatLib.Network;

namespace SharpNeat.Neat.Genome
{
    public class NeatGenomeFactory<T> : INeatGenomeFactory<T>
        where T : struct
    {
        MetaNeatGenome<T> _metaNeatGenome;

        #region Constructor

        public NeatGenomeFactory(MetaNeatGenome<T> metaNeatGenome)
        {
            Debug.Assert(null != metaNeatGenome);
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
            // Assert non-null parameters.
            Debug.Assert(null != connGenes);
            
            // Validity tests.
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));

            // Create genome.
            return CreateInner(id, birthGeneration, connGenes);
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
            // Assert non-null parameters.
            Debug.Assert(null != connGenes);
            Debug.Assert(null != hiddenNodeIdArr);

            // Validity tests.
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, _metaNeatGenome.InputOutputNodeCount));

            // Create genome.
            return CreateInner(id, birthGeneration, connGenes, hiddenNodeIdArr);
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
            // Assert non-null parameters.
            Debug.Assert(null != connGenes);
            Debug.Assert(null != hiddenNodeIdArr);
            Debug.Assert(null != nodeIndexByIdMap);

            // Validity tests.
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, _metaNeatGenome.InputOutputNodeCount));

            // Create genome.
            return CreateInner(id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap);
        }

        /// <summary>
        /// Create a NeatGenome with the given meta data, connection genes and supplementary data.
        /// </summary>
        /// <param name="id">Genome ID.</param>
        /// <param name="birthGeneration">Birth generation.</param>
        /// <param name="connGenes">Connection genes.</param>
        /// <param name="hiddenNodeIdArr">An array of the hidden node IDs in the genome, in ascending order.</param>
        /// <param name="digraph">A DirectedGraph that mirrors the structure described by the connection genes.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public NeatGenome<T> Create(
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            INodeIdMap nodeIndexByIdMap,
            DirectedGraph digraph)
        {
            // Assert non-null parameters.
            Debug.Assert(null != connGenes);
            Debug.Assert(null != hiddenNodeIdArr);
            Debug.Assert(null != nodeIndexByIdMap);
            Debug.Assert(null != digraph);

            // Validity tests.
            Debug.Assert(digraph.InputCount == _metaNeatGenome.InputNodeCount);
            Debug.Assert(digraph.OutputCount == _metaNeatGenome.OutputNodeCount);
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, _metaNeatGenome.InputOutputNodeCount));

            // Invoke constructor.
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
        /// <param name="depthInfo">Graph depth information.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public NeatGenome<T> Create(
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            INodeIdMap nodeIndexByIdMap,
            DirectedGraph digraph,
            GraphDepthInfo depthInfo)
        {
            // Assert non-null parameters.
            Debug.Assert(null != connGenes);
            Debug.Assert(null != hiddenNodeIdArr);
            Debug.Assert(null != nodeIndexByIdMap);
            Debug.Assert(null != digraph);

            // Validity tests.
            Debug.Assert(digraph.InputCount == _metaNeatGenome.InputNodeCount);
            Debug.Assert(digraph.OutputCount == _metaNeatGenome.OutputNodeCount);
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, _metaNeatGenome.InputOutputNodeCount));

            // Notes.
            // 1) If calling this overload of Create() then we require depthInfo to be provided.
            // 2) GraphDepthInfo relates to, and is used for, acyclic graphs only.
            // 3) GraphDepthInfo is optional for acyclic graphs, but if not supplied then one of the other overloads of Create() should be used.
            Debug.Assert(null != depthInfo && _metaNeatGenome.IsAcyclic);

            // Construct genome.
            return new NeatGenome<T>(_metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap, digraph, depthInfo);
        }

        #endregion

        #region Private Methods

        private NeatGenome<T> CreateInner(
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes)
        {
            // Determine the set of node IDs, and create a mapping from node IDs to node indexes.
            int[] hiddenNodeIdArr = ConnectionGenesUtils.CreateHiddenNodeIdArray(connGenes._connArr, _metaNeatGenome.InputOutputNodeCount);

            // Call subroutine to create further supplementary data structures, and create a new genome.
            return CreateInner(id, birthGeneration, connGenes, hiddenNodeIdArr);
        }

        private NeatGenome<T> CreateInner(
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr)
        {
            // Create a mapping from node IDs to node indexes.
            INodeIdMap nodeIndexByIdMap = DirectedGraphUtils.CompileNodeIdMap(hiddenNodeIdArr, _metaNeatGenome.InputOutputNodeCount);

            // Call subroutine to create further supplementary data structures, and create a new genome.
            return CreateInner(id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap);
        }

        private NeatGenome<T> CreateInner(
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            INodeIdMap nodeIndexByIdMap)
        {
            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            int totalNodeCount =  _metaNeatGenome.InputOutputNodeCount + hiddenNodeIdArr.Length;
            DirectedGraph digraph = CreateDirectedGraph(connGenes, nodeIndexByIdMap, totalNodeCount);

            // Construct genome.
            return new NeatGenome<T>(_metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdMap, digraph, null);
        }

        #endregion

        #region Private Methods [CreateDirectedGraph]

        private DirectedGraph CreateDirectedGraph(
            ConnectionGenes<T> connGenes,
            INodeIdMap nodeIndexByIdMap,
            int totalNodeCount)
        {
            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            // Notes. 
            // The array contents will be manipulated, so copying this avoids modification of the genome's
            // connection gene list.
            // The IDs are substituted for node indexes here.
            CopyAndMapIds(
                connGenes._connArr,
                nodeIndexByIdMap,
                out ConnectionIdArrays connIdArrays);

            // Construct a new DirectedGraph.
            var digraph = new DirectedGraph(
                connIdArrays,
                _metaNeatGenome.InputNodeCount,
                _metaNeatGenome.OutputNodeCount,
                totalNodeCount);

            return digraph;
        }

        private static void CopyAndMapIds(
            DirectedConnection[] connArr,
            INodeIdMap nodeIdMap,
            out ConnectionIdArrays connIdArrays)
        {
            int count = connArr.Length;
            int[] srcIdArr = new int[count];
            int[] tgtIdArr = new int[count];

            for(int i=0; i < count; i++) 
            {
                srcIdArr[i] = nodeIdMap.Map(connArr[i].SourceId);
                tgtIdArr[i] = nodeIdMap.Map(connArr[i].TargetId);
            }

            connIdArrays = new ConnectionIdArrays(srcIdArr, tgtIdArr);
        }

        #endregion
    }
}
