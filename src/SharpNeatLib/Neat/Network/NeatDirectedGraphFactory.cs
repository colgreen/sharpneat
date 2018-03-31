using System;
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
        public static WeightedDirectedGraph<T> Create(NeatGenome<T> genome)
        {
            // Assert that the connections are sorted.
            Debug.Assert(DirectedConnectionUtils.IsSorted(genome.ConnectionGenes._connArr));

            // Construct and return a new WeightedDirectedGraph.
            int totalNodeCount = genome.MetaNeatGenome.InputOutputNodeCount + genome.HiddenNodeIdArray.Length;
            return new WeightedDirectedGraph<T>(
                genome.DirectedGraph.ConnectionIdArrays,
                genome.MetaNeatGenome.InputNodeCount,
                genome.MetaNeatGenome.OutputNodeCount,
                totalNodeCount,
                genome.ConnectionGenes._weightArr);
        }

        #endregion
    }
}
