using BenchmarkDotNet.Attributes;
using SharpNeat.Tasks.BinaryElevenMultiplexer;

#pragma warning disable CA1822 // Mark members as static

namespace SharpNeat.Tasks;

public class BinaryElevenMultiplexerEvaluatorBenchmarks
{
    static readonly BinaryElevenMultiplexerEvaluator __evaluator = new();
    static readonly NullBlackBox __blackBox = new();

    [Benchmark]
    public void Evaluate()
    {
        __evaluator.Evaluate(__blackBox);
    }

    private sealed class NullBlackBox : IBlackBox<double>
    {
        readonly double[] _inputAndOutputs = new double[13];

        public NullBlackBox()
        {
            Inputs = _inputAndOutputs.AsMemory(0, 12);
            Outputs = _inputAndOutputs.AsMemory(12, 1);
        }

        public Memory<double> Inputs { get; }
        public Memory<double> Outputs { get; }
        public void Activate() {}
        public void Dispose() {}
        public void Reset() {}
    }
}
