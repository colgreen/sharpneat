using BenchmarkDotNet.Running;
using SharpNeat.Tasks.Benchmarks.FunctionRegression;
using SharpNeat.Tasks.Benchmarks.PreyCapture;

namespace SharpNeatTasks.Benchmarks;

class Program
{
    static void Main()
    {
        var summary = BenchmarkRunner.Run<PreyCaptureWorldBenchmark>();

        //var benchmark = new PreyCaptureWorldBenchmark();
        //benchmark.RunTrials();
    }
}
