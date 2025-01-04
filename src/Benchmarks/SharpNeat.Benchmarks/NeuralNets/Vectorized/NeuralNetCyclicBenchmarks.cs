using BenchmarkDotNet.Attributes;
using Redzen.Random;
using SharpNeat.IO;
using SharpNeat.IO.Models;
using SharpNeat.NeuralNets.IO;

#pragma warning disable CA1822 // Mark members as static

namespace SharpNeat.NeuralNets.Vectorized;

public class NeuralNetCyclicBenchmarks
{
    static readonly NeuralNetCyclic<float> __nn;

    static NeuralNetCyclicBenchmarks()
    {
        // Load neural net model from file, and convert into a neural net instance.
        NetFileModel netFileModel = NetFile.Load("data/genomes/preycapture.net");
        __nn = (NeuralNetCyclic<float>)NeuralNetConverter.ToNeuralNet<float>(
            netFileModel,
            enableHardwareAcceleratedNeuralNets: true,
            enableHardwareAcceleratedActivationFunctions: false);

        // Set some non-zero random input values.
        var rng = RandomDefaults.CreateRandomSource();
        var inputs = __nn.Inputs.Span;
        for(int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = rng.NextFloat();
        }
    }

    [Benchmark]
    public void Activate()
    {
        for(int i = 0; i < 1000; i++)
        {
            __nn.Activate();
        }
    }
}
