using System;
using System.Collections.Generic;
using System.Text;
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
            GraphDepthInfo depthInfo)
        {
            // Mandatory ref tests.
            if(null == metaNeatGenome 
                || null == connGenes
                || null == hiddenNodeIdArr
                || null == nodeIndexByIdMap
                || null == digraph) {
                return false;
            }

            // DepthInfo relates to acyclic graphs only, and is mandatory for acyclic graphs.
            // TODO: Reinstate once depthInfo is not null!
            //if(metaNeatGenome.IsAcyclic && null == depthInfo) return false;

            // Node counts.
            if(digraph.InputCount != metaNeatGenome.InputNodeCount) {
                return false;
            }

            if(digraph.OutputCount != metaNeatGenome.OutputNodeCount) {
                return false;
            }

            if(digraph.TotalNodeCount != nodeIndexByIdMap.Count) {
                return false;
            }

            // Hidden node IDs.
            if(!ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount)) {
                return false;
            }

            int hiddenNodeCount = (digraph.TotalNodeCount - digraph.InputCount) - digraph.OutputCount;
            if(hiddenNodeCount != hiddenNodeIdArr.Length) {
                return false;
            }

            // Connections.
            if(!IsValid_Connections(connGenes, digraph, nodeIndexByIdMap)) {
                return false;
            }

            // All tests passed.
            return true;
        }

        #endregion

        #region Private Static Methods

        private static bool IsValid_Connections(
            ConnectionGenes<T> connGenes,
            DirectedGraph digraph,
            INodeIdMap nodeIndexByIdMap)
        {
            // Connection counts.
            if(connGenes._connArr.Length != digraph.ConnectionIdArrays.Length) {
                return false;
            }

            // Connection order.
            if(!SortUtils.IsSortedAscending(connGenes._connArr)) {
                return false;
            }

            if(!IsSortedAscending(digraph.ConnectionIdArrays)) {
                return false;
            }

            // Connection node ID mappings.
            DirectedConnection[] connArr = connGenes._connArr;
            int[] srcIdArr = digraph.ConnectionIdArrays._sourceIdArr;
            int[] tgtIdArr = digraph.ConnectionIdArrays._targetIdArr;

            for(int i=0; i < connGenes._connArr.Length; i++)
            {
                if(nodeIndexByIdMap.Map(connArr[i].SourceId) != srcIdArr[i]) {
                    return false;
                }

                if(nodeIndexByIdMap.Map(connArr[i].TargetId) != tgtIdArr[i]) {
                    return false;
                }
            }

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

            for(int i=1; i < srcIdArr.Length; i++)
            {
                if(Compare(srcIdArr[i-1], tgtIdArr[i-1], srcIdArr[i], tgtIdArr[i]) > 0) {
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
