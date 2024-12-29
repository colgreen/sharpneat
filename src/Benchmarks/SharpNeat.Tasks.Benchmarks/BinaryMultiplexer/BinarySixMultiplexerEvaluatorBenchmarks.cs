using BenchmarkDotNet.Attributes;
using SharpNeat.Tasks.BinarySixMultiplexer;

#pragma warning disable CA1822 // Mark members as static

namespace SharpNeat.Tasks.BinaryMultiplexer;

public class BinarySixMultiplexerEvaluatorBenchmarks
{
    static readonly BinarySixMultiplexerEvaluatorDouble __evaluator = new();
    static readonly NullBlackBox __blackBox = new();

    [Benchmark]
    public void Evaluate()
    {
        __evaluator.Evaluate(__blackBox);
    }

    private sealed class NullBlackBox : IBlackBox<double>
    {
        readonly double[] _inputAndOutputs = new double[8];

        public NullBlackBox()
        {
            Inputs = _inputAndOutputs.AsMemory(0, 7);
            Outputs = _inputAndOutputs.AsMemory(7, 1);
        }

        public Memory<double> Inputs { get; }
        public Memory<double> Outputs { get; }
        public void Activate() {}
        public void Dispose() {}
        public void Reset() {}
    }
}
