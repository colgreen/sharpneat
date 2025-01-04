using BenchmarkDotNet.Running;
using SharpNeat.Tasks.BinaryMultiplexer;
using SharpNeat.Tasks.FunctionRegression;
using SharpNeat.Tasks.PreyCapture;

namespace SharpNeat.Tasks;

internal sealed class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<PreyCaptureWorldBenchmark>();

        //var benchmark = new PreyCaptureWorldBenchmark();
        //benchmark.RunTrials();

        //BenchmarkRunner.Run<BinarySixMultiplexerEvaluatorBenchmarks>();
        //BenchmarkRunner.Run<BinaryElevenMultiplexerEvaluatorBenchmarks>();
        //BenchmarkRunner.Run<BinaryTwentyMultiplexerEvaluatorBenchmarks>();
        BenchmarkRunner.Run<FuncRegressionUtilsBenchmarks>();
    }
}
