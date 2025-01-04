using BenchmarkDotNet.Attributes;
using SharpNeat.Tasks.BinaryElevenMultiplexer;

#pragma warning disable CA1822 // Mark members as static

namespace SharpNeat.Tasks.BinaryMultiplexer;

public class BinaryElevenMultiplexerEvaluatorBenchmarks
{
    static readonly BinaryElevenMultiplexerEvaluatorFloat __evaluator = new();
    static readonly NullBlackBox __blackBox = new();

    [Benchmark]
    public void Evaluate()
    {
        __evaluator.Evaluate(__blackBox);
    }

    private sealed class NullBlackBox : IBlackBox<float>
    {
        readonly float[] _inputAndOutputs = new float[13];

        public NullBlackBox()
        {
            Inputs = _inputAndOutputs.AsMemory(0, 12);
            Outputs = _inputAndOutputs.AsMemory(12, 1);
        }

        public Memory<float> Inputs { get; }
        public Memory<float> Outputs { get; }
        public void Activate() {}
        public void Dispose() {}
        public void Reset() {}
    }
}
