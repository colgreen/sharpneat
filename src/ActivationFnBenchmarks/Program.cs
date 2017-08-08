using BenchmarkDotNet.Running;

namespace ActivationFnBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<BenchmarksDouble>();
            var summary = BenchmarkRunner.Run<BenchmarksVectorizedDouble>();
        }
    }
}
