using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeat.Neat.Network
{
    /// <summary>
    /// For creating directed graphs from NEAT genomes.
    /// </summary>
    /// <typeparam name="T">Connection weight type.</typeparam>
    public static class NeatDirectedGraphFactory<T> 
        where T : struct
    {
        #region Public Static Methods

        /// <summary>
        /// Create a directed graph based on the provided connections (between node IDs) and a predefined set of node IDs 
        /// represented by inputCount and outputCount, and occupying the ID range from 0 to (inputCount+outputCount)-1.
        /// I.e. this method allows for allocation of predefined input and output nodeIDs regardless of whether they are
        /// connected to or not; this is primarily to allow for the allocation of NeatGenome input and output nodes which
        /// are defined with fixed IDs but aren't necessarily connected to in any given genome.
        /// </summary>
        public static WeightedDirectedGraph<T> Create(ConnectionGenes<T> connGenes, int inputCount, int outputCount)
        {
            // Assert that the connections are sorted.
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));

            // Determine the full set of hidden node IDs.
            int inputOutputCount = inputCount + outputCount;
            var hiddenNodeIdSet = GetHiddenNodeIdSet(connGenes._connArr, inputOutputCount);

            // Compile a mapping from current nodeIDs to new IDs (i.e. removing gaps in the ID space).
            Func<int,int> nodeIdMapFn = DirectedGraphFactoryUtils.CompileNodeIdMap(hiddenNodeIdSet, inputOutputCount);

            // Extract/copy the neat genome connectivity graph into an array of DirectedConnection.
            // Notes. 
            // The array contents will be manipulated, so copying this avoids modification of the genome's
            // connection gene list.
            // The IDs are substituted for node indexes here.
            CopyAndMapIds(connGenes._connArr, nodeIdMapFn,
                out ConnectionIdArrays connIdArrays);

            // Construct and return a new WeightedDirectedGraph.
            int totalNodeCount =  inputOutputCount + hiddenNodeIdSet.Count;
            return new WeightedDirectedGraph<T>(connIdArrays, inputCount, outputCount, totalNodeCount, connGenes._weightArr);
        }

        #endregion

        #region Private Static Methods 

        private static HashSet<int> GetHiddenNodeIdSet(DirectedConnection[] connArr, int inputOutputCount)
        {
            // Build a hash set of all hidden nodes IDs referred to by the connections.
            // TODO: Re-use this hashset to avoid memory alloc and GC overhead.
            var hiddenNodeIdSet = new HashSet<int>();
            
            // Extract hidden node IDs from the connections, to build a complete set of hidden nodeIDs.
            for(int i=0; i<connArr.Length; i++)
            {
                if(connArr[i].SourceId >= inputOutputCount) { 
                    hiddenNodeIdSet.Add(connArr[i].SourceId); 
                }
                if(connArr[i].TargetId >= inputOutputCount) { 
                    hiddenNodeIdSet.Add(connArr[i].TargetId); 
                }
            }
            return hiddenNodeIdSet;
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
