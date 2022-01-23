using BenchmarkDotNet.Attributes;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.Double.Vectorized;
using SharpNeat.Neat.Genome.IO;
using SharpNeat.NeuralNets.Double.ActivationFunctions;

#pragma warning disable CA1822 // Mark members as static

namespace SharpNeat.NeuralNets.Double.Vectorized.Benchmarks
{
    public class NeuralNetCyclicBenchmarks
    {
        static readonly NeuralNetCyclic __nn;

        static NeuralNetCyclicBenchmarks()
        {
            // TODO: Load neural nets directly, instead of loading a genome and decoding.
            var metaNeatGenome = new MetaNeatGenome<double>(14, 4, false, new LeakyReLU());
            var genomeLoader = NeatGenomeLoaderFactory.CreateLoaderDouble(metaNeatGenome);
            var genome = genomeLoader.Load("data/genomes/preycapture.genome");

            var genomeDecoder = new NeatGenomeDecoderCyclic(1);
            __nn = (NeuralNetCyclic)genomeDecoder.Decode(genome);

            // Set some non-zero random input values.
            var rng = RandomDefaults.CreateRandomSource();
            var inputs = __nn.Inputs.Span;
            for(int i=0; i < inputs.Length; i++)
            {
                inputs[i] = rng.NextDouble();
            }
        }

        [Benchmark]
        public void Activate()
        {
            for(int i=0; i < 1000; i++)
            {
                __nn.Activate();
            }
        }
    }
}
