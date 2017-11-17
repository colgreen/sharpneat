using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeat.Neat.Network
{
    public static class NeatDirectedGraphFactory<T> 
        where T : struct
    {
        #region Public Static Methods

        /// <summary>
        /// Create a directed graph based on the provided connections (between node IDs) and a predefined set of node IDs.
        /// Clearly the set of nodeIDs could be discovered by iterating over the connections. This overload exists to allow
        /// for additional fixed node IDs to be allocated regardless of whether they are connected to or not, this is primarily
        /// to allow for the allocation of NeatGenome input and output nodes, which are defined with fixed IDs but aren't
        /// necessarily connected to.
        /// </summary>
        public static WeightedDirectedGraph<T> Create(ConnectionGene<T>[] connGeneArr, int inputCount, int outputCount)
        {
            // Debug assert that the connections are sorted.
            Debug.Assert(ConnectionGeneUtils.IsSorted(connGeneArr));

            // Determine the full set of hidden node IDs.
            int inputOutputCount = inputCount + outputCount;
            var hiddenNodeIdSet = GetHiddenNodeIdSet(connGeneArr, inputOutputCount);

            // Compile a mapping from current nodeIDs to new IDs (i.e. removing gaps in the ID space).
            Func<int,int> nodeIdMapFn = DirectedGraphFactoryUtils.CompileNodeIdMap(hiddenNodeIdSet, inputOutputCount);

            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            // Notes. 
            // The array contents will be manipulated, so copying this avoids modification of the genome's
            // connection gene list.
            // The IDs are substituted for node indexes here.
            CopyAndMapIds(connGeneArr, nodeIdMapFn,
                out ConnectionIdArrays connIdArrays,
                out T[] weightArr);

            // Construct and return a new WeightedDirectedGraph.
            int totalNodeCount =  inputOutputCount + hiddenNodeIdSet.Count;
            return new WeightedDirectedGraph<T>(connIdArrays, inputCount, outputCount, totalNodeCount, weightArr);
        }

        #endregion

        #region Private Static Methods 

        private static HashSet<int> GetHiddenNodeIdSet(ConnectionGene<T>[] connGeneArr, int inputOutputCount)
        {
            // Build a hash set of all hidden nodes IDs referred to by the connections.
            // TODO: Re-use this hashset to avoid memory alloc and GC overhead.
            var hiddenNodeIdSet = new HashSet<int>();
            
            // Extract hidden node IDs from the connections, to build a complete set of hidden nodeIDs.
            for(int i=0; i<connGeneArr.Length; i++)
            {
                if(connGeneArr[i].SourceId >= inputOutputCount) { 
                    hiddenNodeIdSet.Add(connGeneArr[i].SourceId); 
                }
                if(connGeneArr[i].TargetId >= inputOutputCount) { 
                    hiddenNodeIdSet.Add(connGeneArr[i].TargetId); 
                }
            }
            return hiddenNodeIdSet;
        }

        /// <summary>
        /// Split each IWeightedDirectedConnection in a list into an array of DirectedConnections(s), and an array of weights.
        /// Map the node IDs to indexes as we go.
        /// </summary>
        private static void CopyAndMapIds(
            ConnectionGene<T>[] connGeneArr,
            Func<int,int> nodeIdMap,
            out ConnectionIdArrays connIdArrays,
            out T[] weightArr)
        {
            int count = connGeneArr.Length;
            int[] srcIdArr = new int[count];
            int[] tgtIdArr = new int[count];
            weightArr = new T[count];

            for(int i=0; i < count; i++) 
            {
                srcIdArr[i] = nodeIdMap(connGeneArr[i].SourceId);
                tgtIdArr[i] = nodeIdMap(connGeneArr[i].TargetId);
                weightArr[i] = connGeneArr[i].Weight;
            }

            connIdArrays = new ConnectionIdArrays(srcIdArr, tgtIdArr);
        }

        #endregion
    }
}
