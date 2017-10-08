using BenchmarkDotNet.Running;

namespace SharpNeatLib.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<BenchmarksDouble>();
            //var summary = BenchmarkRunner.Run<BenchmarksVectorizedDouble>();
            var summary = BenchmarkRunner.Run<ConnectionSorterBenchmarks>();
        }
    }
}
