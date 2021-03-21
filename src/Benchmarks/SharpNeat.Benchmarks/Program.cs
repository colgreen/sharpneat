using BenchmarkDotNet.Running;

namespace SharpNeat.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<NeuralNets.Double.ActivationFunctions.Benchmarks.BenchmarksDouble>();
            var summary = BenchmarkRunner.Run<NeuralNets.Double.ActivationFunctions.Vectorized.Benchmarks.BenchmarksVectorizedDouble>();
            //var summary = BenchmarkRunner.Run<ConnectionSorterBenchmarks>();
        }
    }
}
