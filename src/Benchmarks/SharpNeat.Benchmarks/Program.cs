using BenchmarkDotNet.Running;

namespace SharpNeat.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<NeuralNets.Double.ActivationFunctions.Benchmarks.ActivationFunctionsBenchmarks>();
            var summary = BenchmarkRunner.Run<NeuralNets.Double.ActivationFunctions.Vectorized.Benchmarks.ActivationFunctionsBenchmarks>();
            //var summary = BenchmarkRunner.Run<ConnectionSorterBenchmarks>();
        }
    }
}
