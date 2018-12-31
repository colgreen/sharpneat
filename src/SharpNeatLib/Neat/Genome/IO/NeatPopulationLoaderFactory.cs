
namespace SharpNeat.Neat.Genome.IO
{
    public static class NeatPopulationLoaderFactory
    {
        public static NeatPopulationLoader<double> CreateLoaderDouble(
            MetaNeatGenome<double> metaNeatGenome)
        {
            NeatGenomeLoader<double> genomeLoader = NeatGenomeLoaderFactory.CreateLoaderDouble(metaNeatGenome);
            return new NeatPopulationLoader<double>(genomeLoader);
        }
    }
}
