using BenchmarkDotNet.Running;
using SharpNeatTasks.Benchmarks.FunctionRegression;
using SharpNeatTasks.Benchmarks.PreyCapture;

namespace SharpNeatTasks.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PreyCaptureWorldBenchmark>();

            //var benchmark = new PreyCaptureWorldBenchmark();
            //benchmark.RunTrials();
        }
    }
}
