using BenchmarkDotNet.Attributes;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.Double;
using SharpNeat.Neat.Genome.IO;
using SharpNeat.NeuralNets.Double.ActivationFunctions;

#pragma warning disable CA1822 // Mark members as static

namespace SharpNeat.NeuralNets.Double.Benchmarks;

public class NeuralNetCyclicBenchmarks
{
    static readonly NeuralNetCyclic __nn;

    static NeuralNetCyclicBenchmarks()
    {
        // TODO: Load neural nets directly, instead of loading a genome and decoding.
        var metaNeatGenome = MetaNeatGenome<double>.CreateCyclic(14, 4, 1, new LeakyReLU());
        var genome = NeatGenomeLoader.Load("data/genomes/preycapture.net", metaNeatGenome, 0);

        var genomeDecoder = new NeatGenomeDecoderCyclic();
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
