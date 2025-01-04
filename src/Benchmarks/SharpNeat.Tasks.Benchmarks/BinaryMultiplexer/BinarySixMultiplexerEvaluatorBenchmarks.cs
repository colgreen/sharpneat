using BenchmarkDotNet.Attributes;
using SharpNeat.Tasks.BinarySixMultiplexer;

#pragma warning disable CA1822 // Mark members as static

namespace SharpNeat.Tasks.BinaryMultiplexer;

public class BinarySixMultiplexerEvaluatorBenchmarks
{
    static readonly BinarySixMultiplexerEvaluatorFloat __evaluator = new();
    static readonly NullBlackBox __blackBox = new();

    [Benchmark]
    public void Evaluate()
    {
        __evaluator.Evaluate(__blackBox);
    }

    private sealed class NullBlackBox : IBlackBox<float>
    {
        readonly float[] _inputAndOutputs = new float[8];

        public NullBlackBox()
        {
            Inputs = _inputAndOutputs.AsMemory(0, 7);
            Outputs = _inputAndOutputs.AsMemory(7, 1);
        }

        public Memory<float> Inputs { get; }
        public Memory<float> Outputs { get; }
        public void Activate() {}
        public void Dispose() {}
        public void Reset() {}
    }
}
