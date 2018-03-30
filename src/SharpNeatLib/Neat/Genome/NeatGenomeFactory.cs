using System;
using System.Diagnostics;
using SharpNeat.Network;

namespace SharpNeat.Neat.Genome
{
    public static class NeatGenomeFactory<T> 
        where T : struct
    {
        #region Public Static Factory Methods

        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, 
            int birthGeneration,
            ConnectionGenes<T> connGenes)
        {
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));

            // Determine the set of node IDs, and create a mapping from node IDs to node indexes.
            int[] hiddenNodeIdArr = ConnectionGenesUtils.CreateHiddenNodeIdArray(connGenes._connArr, metaNeatGenome.InputOutputNodeCount);
            Func<int,int> nodeIndexByIdFn = DirectedGraphUtils.CompileNodeIdMap(hiddenNodeIdArr, metaNeatGenome.InputOutputNodeCount);

            return new NeatGenome<T>(metaNeatGenome, id, birthGeneration, connGenes, null, hiddenNodeIdArr, nodeIndexByIdFn, null);
        }

        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr)
        {
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

            // Create a mapping from node IDs to node indexes.
            Func<int,int> nodeIndexByIdFn = DirectedGraphUtils.CompileNodeIdMap(hiddenNodeIdArr, metaNeatGenome.InputOutputNodeCount);

            return new NeatGenome<T>(metaNeatGenome, id, birthGeneration, connGenes, null, hiddenNodeIdArr, nodeIndexByIdFn, null);
        }

        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            Func<int,int> nodeIndexByIdFn)
        {
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

            return new NeatGenome<T>(metaNeatGenome, id, birthGeneration, connGenes, null, hiddenNodeIdArr, nodeIndexByIdFn, null);
        }

        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            Func<int,int> nodeIndexByIdFn,
            GraphDepthInfo depthInfo)
        {
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

            // Notes.
            // 1) If calling this overload of Create() then we require depthInfo to be provided.
            // 2) GraphDepthInfo relates to, and is used for, acyclic graphs only.
            // 3) GraphDepthInfo is optional for acyclic graphs, but if not supplied then one of the other overloads of Create() should be used.
            Debug.Assert(null != depthInfo && metaNeatGenome.IsAcyclic);

            return new NeatGenome<T>(metaNeatGenome, id, birthGeneration, connGenes, null, hiddenNodeIdArr, nodeIndexByIdFn, depthInfo);
        }

        #endregion


    }
}
