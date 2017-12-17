using SharpNeat.Network;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover
{
    internal struct ConnectionGene<T> where T : struct
    {
        public readonly int Id;
        public readonly DirectedConnection Endpoints;
        public readonly T Weight;

        public ConnectionGene(int id, DirectedConnection endpoints, T weight)
        {
            this.Id = id;
            this.Endpoints = endpoints;
            this.Weight = weight;
        }
    }   
}
