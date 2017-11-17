using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.Neat.Network
{
    public static class NeatAcyclicDirectedGraphFactory<T>
        where T : struct
    {
        #region Public Static Methods

        public static WeightedAcyclicDirectedGraph<T> Create(ConnectionGene<T>[] connGeneArr, int inputCount, int outputCount)
        {
            // Convert the set of connections to a standardised graph representation.
            WeightedDirectedGraph<T> digraph = NeatDirectedGraphFactory<T>.Create(connGeneArr, inputCount, outputCount);

            // Invoke factory logic specific to acyclic graphs.
            return WeightedAcyclicDirectedGraphFactory<T>.Create(digraph, inputCount, outputCount);
        }

        #endregion
    }
}
