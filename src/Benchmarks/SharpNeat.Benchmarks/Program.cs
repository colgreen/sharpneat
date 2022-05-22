using BenchmarkDotNet.Running;
//using SharpNeat.NeuralNets.Double.Vectorized.Benchmarks;

namespace SharpNeat.Benchmarks;

class Program
{
    static void Main()
    {
        //BenchmarkRunner.Run<NeuralNets.Double.ActivationFunctions.Benchmarks.ActivationFunctionsBenchmarks>();
        //BenchmarkRunner.Run<NeuralNets.Double.ActivationFunctions.Vectorized.Benchmarks.ActivationFunctionsBenchmarks>();
        //BenchmarkRunner.Run<ConnectionSorterBenchmarks>();

        BenchmarkRunner.Run<NeuralNets.Double.Benchmarks.NeuralNetAcyclicBenchmarks>();
        //BenchmarkRunner.Run<NeuralNets.Double.Vectorized.Benchmarks.NeuralNetAcyclicBenchmarks>();

        //BenchmarkRunner.Run<NeuralNets.Double.Benchmarks.NeuralNetCyclicBenchmarks>();
        //BenchmarkRunner.Run<NeuralNets.Double.Vectorized.Benchmarks.NeuralNetCyclicBenchmarks>();

        //var benchmarks = new NeuralNets.Double.Benchmarks.NeuralNetAcyclicBenchmarks();
        //for(; ; )
        //{
        //    benchmarks.Activate();
        //}

        //var benchmarks = new NeuralNets.Double.ActivationFunctions.Vectorized.Benchmarks.ActivationFunctionsBenchmarks();
        //for(; ; )
        //{
        //    benchmarks.LeakyReLU();
        //}
    }
}
