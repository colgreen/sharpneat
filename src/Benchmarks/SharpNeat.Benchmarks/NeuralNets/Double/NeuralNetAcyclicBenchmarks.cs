using BenchmarkDotNet.Attributes;
using Redzen.Random;
using SharpNeat.IO;
using SharpNeat.IO.Models;
using SharpNeat.NeuralNets.IO;

#pragma warning disable CA1822 // Mark members as static

namespace SharpNeat.NeuralNets.Double;

public class NeuralNetAcyclicBenchmarks
{
    static readonly NeuralNetAcyclic<double> __nn;

    static NeuralNetAcyclicBenchmarks()
    {
        // Load neural net model from file, and convert into a neural net instance.
        NetFileModel netFileModel = NetFile.Load("data/genomes/binary11.net");
        __nn = (NeuralNetAcyclic<double>)NeuralNetConverter.ToNeuralNet(netFileModel);

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
