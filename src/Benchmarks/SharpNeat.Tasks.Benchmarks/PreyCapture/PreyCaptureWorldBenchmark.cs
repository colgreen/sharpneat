using BenchmarkDotNet.Attributes;
using SharpNeat.Tasks.PreyCapture;

namespace SharpNeat.Tasks.Benchmarks.PreyCapture;

public class PreyCaptureWorldBenchmark : IDisposable
{
    readonly PreyCaptureWorld _world;
    readonly MockPreyCaptureAgent _agent;

    public PreyCaptureWorldBenchmark()
    {
        _world = new PreyCaptureWorld(4, 1f, 4f, 1000);
        _agent = new MockPreyCaptureAgent();
    }

    [Benchmark]
    public void RunTrial()
    {
        _world.RunTrial(_agent);
    }

    public void RunTrials()
    {
        for(int i = 0; i < 30_000_000; i++)
        {
            _world.RunTrial(_agent);
        }
    }

    public void Dispose()
    {
        _agent.Dispose();
        GC.SuppressFinalize(this);
    }
}
