using SharpNeat.Network;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover
{
    internal struct ConnectionGene<T> where T : struct
    {
        public readonly DirectedConnection Endpoints;
        public readonly T Weight;

        public ConnectionGene(DirectedConnection endpoints, T weight)
        {
            this.Endpoints = endpoints;
            this.Weight = weight;
        }
    }   
}
