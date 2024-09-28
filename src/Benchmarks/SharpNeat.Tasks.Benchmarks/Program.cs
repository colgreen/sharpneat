using BenchmarkDotNet.Running;
using SharpNeat.Tasks.BinaryMultiplexer;
using SharpNeat.Tasks.PreyCapture;

namespace SharpNeat.Tasks;

sealed class Program
{
    static void Main()
    {
        //var summary = BenchmarkRunner.Run<PreyCaptureWorldBenchmark>();

        //var benchmark = new PreyCaptureWorldBenchmark();
        //benchmark.RunTrials();

        //BenchmarkRunner.Run<BinarySixMultiplexerEvaluatorBenchmarks>();
        //BenchmarkRunner.Run<BinaryElevenMultiplexerEvaluatorBenchmarks>();
        BenchmarkRunner.Run<BinaryTwentyMultiplexerEvaluatorBenchmarks>();
    }
}
