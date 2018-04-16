using System;
using System.Collections.Generic;
using System.Text;
using Redzen;
using Redzen.Sorting;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.Neat.Genome
{
    internal static class NeatGenomeValidation<T>
        where T : struct
    {
        #region Public Static Methods

        public static bool IsValid(
            MetaNeatGenome<T> metaNeatGenome,
            int id,
            int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            INodeIdMap nodeIndexByIdMap,
            DirectedGraph digraph,
            int[] connectionIndexMap)
        {
            // Mandatory ref tests.
            if(    null == metaNeatGenome 
                || null == connGenes
                || null == hiddenNodeIdArr
                || null == nodeIndexByIdMap
                || null == digraph) {
                return false;
            }

            // Acyclic/cyclic specific checks.
            if(metaNeatGenome.IsAcyclic)
            {
                if(digraph.GetType() != typeof(AcyclicDirectedGraph) || null == connectionIndexMap) {
                    return false;
                }
            }
            else
            {
                if(digraph.GetType() != typeof(DirectedGraph)) {
                    return false;
                }
            }


            // DepthInfo relates to acyclic graphs only, and is mandatory for acyclic graphs.
            // TODO: Enable once NeatGenomeAcyclicBuilder is finished.
            
            
            //if(metaNeatGenome.IsAcyclic) 
            //{
            //    if(!IsValid_Acyclic(metaNeatGenome, depthInfo)) {
            //        return false;
            //    }
            //}

            // Node counts.
            if(!ValidateNodeCounts(metaNeatGenome, hiddenNodeIdArr, nodeIndexByIdMap, digraph, connectionIndexMap)) {
                return false;
            }

            // Hidden node IDs.
            if(!ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount)) {
                return false;
            }

            // Connections.
            if (!ValidateConnections(connGenes, digraph, nodeIndexByIdMap)) {
                return false;
            }

            // All tests passed.
            return true;
        }

        #endregion

        #region Private Static Methods

        private static bool ValidateNodeCounts(
            MetaNeatGenome<T> metaNeatGenome,
            int[] hiddenNodeIdArr,
            INodeIdMap nodeIndexByIdMap,
            DirectedGraph digraph,
            int[] connectionIndexMap)
        {
            int totalNodeCount = metaNeatGenome.InputNodeCount + metaNeatGenome.OutputNodeCount + hiddenNodeIdArr.Length;

            return digraph.InputCount == metaNeatGenome.InputNodeCount
                && digraph.OutputCount == metaNeatGenome.OutputNodeCount
                && digraph.TotalNodeCount == totalNodeCount
                && nodeIndexByIdMap.Count == totalNodeCount;
        }

        private static bool ValidateConnections(
            ConnectionGenes<T> connGenes,
            DirectedGraph digraph,
            INodeIdMap nodeIndexByIdMap)
        {
            // Connection counts.
            if(connGenes._connArr.Length != digraph.ConnectionIdArrays.Length) {
                return false;
            }

            // Connection order.
            if(   !SortUtils.IsSortedAscending(connGenes._connArr)
               || !IsSortedAscending(digraph.ConnectionIdArrays)) {
                return false;
            }

            // Connection node ID mappings.
            DirectedConnection[] connArr = connGenes._connArr;
            int[] srcIdArr = digraph.ConnectionIdArrays._sourceIdArr;
            int[] tgtIdArr = digraph.ConnectionIdArrays._targetIdArr;

            for(int i=0; i < connGenes._connArr.Length; i++)
            {
                if(   nodeIndexByIdMap.Map(connArr[i].SourceId) != srcIdArr[i]
                   || nodeIndexByIdMap.Map(connArr[i].TargetId) != tgtIdArr[i]) {
                    return false;
                }
            }

            return true;
        }

        private static bool IsValid_Acyclic(
            MetaNeatGenome<T> metaNeatGenome,
            GraphDepthInfo depthInfo)
        {
            // DepthInfo is mandatory for acyclic graphs.
            if(null == depthInfo) {
                return false;
            }

            // Test that all input nodes are at depth zero.
            // Any input node with a non-zero depth must have an input connection, and this is not supported.
            if(!ArrayUtils.Equals(depthInfo._nodeDepthArr, 0, 0, metaNeatGenome.InputNodeCount)) {
                return false;
            }

            // TODO: More acyclic graph validation.

            return true;
        }

        #endregion

        #region Private Static Methods [ConnectionIdArrays Sort Order]

        private static bool IsSortedAscending(
            ConnectionIdArrays connIdArrays)
        {
            if(0 == connIdArrays.Length) {
                return true;
            }

            int[] srcIdArr = connIdArrays._sourceIdArr;
            int[] tgtIdArr = connIdArrays._targetIdArr;

            for(int i=0; i < srcIdArr.Length -1 ; i++)
            {
                if(Compare(srcIdArr[0], tgtIdArr[0], srcIdArr[i+1], tgtIdArr[i+1]) > 0) {
                    return false;
                }
            }

            return true;
        }

        private static int Compare(int srcIdA, int tgtIdA, int srcIdB, int tgtIdB)
        {
            if(srcIdA < srcIdB) { return -1; }
            if(srcIdA > srcIdB) { return 1; }

            if(tgtIdA < tgtIdB) { return -1; }
            if(tgtIdA > tgtIdB) { return 1; }
            return 0;
        }

        #endregion
    }
}
