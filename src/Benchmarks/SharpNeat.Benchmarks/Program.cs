using BenchmarkDotNet.Running;
using SharpNeat.Neat.DistanceMetrics;

namespace SharpNeat;

internal sealed class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<NeuralNets.ActivationFunctions.Vectorized.ActivationFunctionsBenchmarks>();
        //BenchmarkRunner.Run<ConnectionSorterBenchmarks>();

        //BenchmarkRunner.Run<NeuralNets.Double.NeuralNetAcyclicBenchmarks>();
        //BenchmarkRunner.Run<NeuralNets.Double.Vectorized.NeuralNetAcyclicBenchmarks>();

        //BenchmarkRunner.Run<NeuralNets.Double.NeuralNetCyclicBenchmarks>();
        //BenchmarkRunner.Run<NeuralNets.Double.Vectorized.NeuralNetCyclicBenchmarks>();

        //BenchmarkRunner.Run<DistanceMetricUtilsBenchmarks>();
        //BenchmarkRunner.Run<EuclideanDistanceMetricBenchmarks>();
        //BenchmarkRunner.Run<ManhattanDistanceMetricBenchmarks>();

        //var benchmarks = new NeuralNets.Double.NeuralNetAcyclicBenchmarks();
        //for(; ; )
        //{
        //    benchmarks.Activate();
        //}

        //var benchmarks = new NeuralNets.Double.ActivationFunctions.Vectorized.ActivationFunctionsBenchmarks();
        //for(; ; )
        //{
        //    benchmarks.LeakyReLU();
        //}
    }
}
