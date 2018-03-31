using System;
using System.Diagnostics;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.Neat.Genome
{
    public static class NeatGenomeFactory<T> 
        where T : struct
    {
        #region Public Static Factory Methods

        /// <summary>
        /// Create a NeatGenome with the given meta data and connection genes.
        /// </summary>
        /// <param name="metaNeatGenome">Genome metadata.</param>
        /// <param name="id">Genome ID.</param>
        /// <param name="birthGeneration">Birth generation.</param>
        /// <param name="connGenes">Connection genes.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, 
            int birthGeneration,
            ConnectionGenes<T> connGenes)
        {
            // Assert non-null parameters.
            Debug.Assert(null != metaNeatGenome);
            Debug.Assert(null != connGenes);
            
            // Validity tests.
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));

            // Create genome.
            return CreateInner(metaNeatGenome, id, birthGeneration, connGenes);
        }

        /// <summary>
        /// Create a NeatGenome with the given meta data, connection genes and supplementary data.
        /// </summary>
        /// <param name="metaNeatGenome">Genome metadata.</param>
        /// <param name="id">Genome ID.</param>
        /// <param name="birthGeneration">Birth generation.</param>
        /// <param name="connGenes">Connection genes.</param>
        /// <param name="hiddenNodeIdArr">An array of the hidden node IDs in the genome, in ascending order.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr)
        {
            // Assert non-null parameters.
            Debug.Assert(null != metaNeatGenome);
            Debug.Assert(null != connGenes);
            Debug.Assert(null != hiddenNodeIdArr);

            // Validity tests.
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

            // Create genome.
            return CreateInner(metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr);
        }

        /// <summary>
        /// Create a NeatGenome with the given meta data, connection genes and supplementary data.
        /// </summary>
        /// <param name="metaNeatGenome">Genome metadata.</param>
        /// <param name="id">Genome ID.</param>
        /// <param name="birthGeneration">Birth generation.</param>
        /// <param name="connGenes">Connection genes.</param>
        /// <param name="hiddenNodeIdArr">An array of the hidden node IDs in the genome, in ascending order.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            Func<int,int> nodeIndexByIdFn)
        {
            // Assert non-null parameters.
            Debug.Assert(null != metaNeatGenome);
            Debug.Assert(null != connGenes);
            Debug.Assert(null != hiddenNodeIdArr);
            Debug.Assert(null != nodeIndexByIdFn);

            // Validity tests.
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

            // Create genome.
            return CreateInner(metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdFn);
        }

        /// <summary>
        /// Create a NeatGenome with the given meta data, connection genes and supplementary data.
        /// </summary>
        /// <param name="metaNeatGenome">Genome metadata.</param>
        /// <param name="id">Genome ID.</param>
        /// <param name="birthGeneration">Birth generation.</param>
        /// <param name="connGenes">Connection genes.</param>
        /// <param name="hiddenNodeIdArr">An array of the hidden node IDs in the genome, in ascending order.</param>
        /// <param name="digraph">A DirectedGraph that mirrors the structure described by the connection genes.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            Func<int,int> nodeIndexByIdFn,
            DirectedGraph digraph)
        {
            // Assert non-null parameters.
            Debug.Assert(null != metaNeatGenome);
            Debug.Assert(null != connGenes);
            Debug.Assert(null != hiddenNodeIdArr);
            Debug.Assert(null != nodeIndexByIdFn);
            Debug.Assert(null != digraph);

            // Validity tests.
            Debug.Assert(digraph.InputCount == metaNeatGenome.InputNodeCount);
            Debug.Assert(digraph.OutputCount == metaNeatGenome.OutputNodeCount);
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

            // Invoke constructor.
            return new NeatGenome<T>(metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdFn, digraph, null);
        }

        /// <summary>
        /// Create a NeatGenome with the given meta data, connection genes and supplementary data.
        /// </summary>
        /// <param name="metaNeatGenome">Genome metadata.</param>
        /// <param name="id">Genome ID.</param>
        /// <param name="birthGeneration">Birth generation.</param>
        /// <param name="connGenes">Connection genes.</param>
        /// <param name="hiddenNodeIdArr">An array of the hidden node IDs in the genome, in ascending order.</param>
        /// <param name="digraph">A DirectedGraph that mirrors the structure described by the connection genes.</param>
        /// <param name="depthInfo">Graph depth information.</param>
        /// <returns>A new NeatGenome instance.</returns>
        public static NeatGenome<T> CreateAcyclic(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            Func<int,int> nodeIndexByIdFn,
            DirectedGraph digraph,
            GraphDepthInfo depthInfo)
        {
            // Assert non-null parameters.
            Debug.Assert(null != metaNeatGenome);
            Debug.Assert(null != connGenes);
            Debug.Assert(null != hiddenNodeIdArr);
            Debug.Assert(null != nodeIndexByIdFn);
            Debug.Assert(null != digraph);

            // Validity tests.
            Debug.Assert(digraph.InputCount == metaNeatGenome.InputNodeCount);
            Debug.Assert(digraph.OutputCount == metaNeatGenome.OutputNodeCount);
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

            // Notes.
            // 1) If calling this overload of Create() then we require depthInfo to be provided.
            // 2) GraphDepthInfo relates to, and is used for, acyclic graphs only.
            // 3) GraphDepthInfo is optional for acyclic graphs, but if not supplied then one of the other overloads of Create() should be used.
            Debug.Assert(null != depthInfo && metaNeatGenome.IsAcyclic);

            // Construct genome.
            return new NeatGenome<T>(metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdFn, digraph, depthInfo);
        }

        #endregion

        #region Private Static Methods

        private static NeatGenome<T> CreateInner(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes)
        {
            // Determine the set of node IDs, and create a mapping from node IDs to node indexes.
            int[] hiddenNodeIdArr = ConnectionGenesUtils.CreateHiddenNodeIdArray(connGenes._connArr, metaNeatGenome.InputOutputNodeCount);

            // Call subroutine to create further supplementary data structures, and create a new genome.
            return CreateInner(metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr);
        }

        private static NeatGenome<T> CreateInner(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr)
        {
            // Create a mapping from node IDs to node indexes.
            Func<int,int> nodeIndexByIdFn = DirectedGraphUtils.CompileNodeIdMap(hiddenNodeIdArr, metaNeatGenome.InputOutputNodeCount);

            // Call subroutine to create further supplementary data structures, and create a new genome.
            return CreateInner(metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdFn);
        }

        private static NeatGenome<T> CreateInner(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            Func<int,int> nodeIndexByIdFn)
        {
            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            int totalNodeCount =  metaNeatGenome.InputOutputNodeCount + hiddenNodeIdArr.Length;
            DirectedGraph digraph = CreateDirectedGraph(metaNeatGenome, connGenes, nodeIndexByIdFn, totalNodeCount);

            // Construct genome.
            return new NeatGenome<T>(metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdFn, digraph, null);
        }

        #endregion

        #region Private Static Methods [CreateDirectedGraph]

        private static DirectedGraph CreateDirectedGraph(
            MetaNeatGenome<T> metaNeatGenome,
            ConnectionGenes<T> connGenes,
            Func<int,int> nodeIndexByIdFn,
            int totalNodeCount)
        {
            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            // Notes. 
            // The array contents will be manipulated, so copying this avoids modification of the genome's
            // connection gene list.
            // The IDs are substituted for node indexes here.
            CopyAndMapIds(
                connGenes._connArr,
                nodeIndexByIdFn,
                out ConnectionIdArrays connIdArrays);

            // Construct a new DirectedGraph.
            var digraph = new DirectedGraph(
                connIdArrays,
                metaNeatGenome.InputNodeCount,
                metaNeatGenome.OutputNodeCount,
                totalNodeCount);

            return digraph;
        }

        private static void CopyAndMapIds(
            DirectedConnection[] connArr,
            Func<int,int> nodeIdMap,
            out ConnectionIdArrays connIdArrays)
        {
            int count = connArr.Length;
            int[] srcIdArr = new int[count];
            int[] tgtIdArr = new int[count];

            for(int i=0; i < count; i++) 
            {
                srcIdArr[i] = nodeIdMap(connArr[i].SourceId);
                tgtIdArr[i] = nodeIdMap(connArr[i].TargetId);
            }

            connIdArrays = new ConnectionIdArrays(srcIdArr, tgtIdArr);
        }

        #endregion
    }
}
