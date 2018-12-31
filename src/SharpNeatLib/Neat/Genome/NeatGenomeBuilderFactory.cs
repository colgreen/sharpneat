
namespace SharpNeat.Neat.Genome
{
    public static class NeatGenomeBuilderFactory<T>
        where T : struct
    {
        public static INeatGenomeBuilder<T> Create(
            MetaNeatGenome<T> metaNeatGenome)
        {
            if(metaNeatGenome.IsAcyclic)
            {
                return new NeatGenomeAcyclicBuilder<T>(metaNeatGenome);
            }
            // else
            return new NeatGenomeBuilder<T>(metaNeatGenome);
        }
    }
}
