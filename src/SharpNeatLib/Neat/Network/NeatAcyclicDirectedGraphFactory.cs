using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.Neat.Network
{
    /// <summary>
    /// For creating acyclic directed graphs from NEAT genomes.
    /// </summary>
    /// <typeparam name="T">Connection weight type.</typeparam>
    public static class NeatAcyclicDirectedGraphFactory<T>
        where T : struct
    {
        #region Public Static Methods

        public static WeightedAcyclicDirectedGraph<T> Create(NeatGenome<T> genome)
        {
            // Convert the set of connections to a standardised graph representation.
            WeightedDirectedGraph<T> digraph = NeatDirectedGraphFactory<T>.Create(genome);

            // Invoke factory logic specific to acyclic graphs.
            return WeightedAcyclicDirectedGraphFactory<T>.Create(
                digraph,
                genome.MetaNeatGenome.InputNodeCount,
                genome.MetaNeatGenome.OutputNodeCount);
        }

        #endregion
    }
}
